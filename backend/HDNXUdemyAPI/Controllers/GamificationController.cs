using Asp.Versioning;
using HDNXUdemyData.Entities;
using HDNXUdemyData.IRepository;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using System.Net;

namespace HDNXUdemyAPI.Controllers
{
    /// <summary>
    /// Gamification: Streaks, XP Points, Achievement Badges
    /// </summary>
    [ApiVersion(VersionApi.Version1)]
    [Route("api/v{version:apiVersion}/Gamification")]
    [Authorize]
    public class GamificationController : BaseController
    {
        private readonly ILearningStreakRepository _streakRepository;
        private readonly IAchievementBadgeRepository _badgeRepository;
        private readonly IUserBadgeRepository _userBadgeRepository;
        private readonly IXPTransactionRepository _xpRepository;

        // XP values
        private const int XP_LESSON_COMPLETE = 10;
        private const int XP_QUIZ_PASS = 25;
        private const int XP_REVIEW_GIVEN = 15;
        private const int XP_STREAK_BONUS_7 = 50;
        private const int XP_STREAK_BONUS_30 = 200;
        private const int XP_FIRST_PURCHASE = 100;

        public GamificationController(
            ILearningStreakRepository streakRepository,
            IAchievementBadgeRepository badgeRepository,
            IUserBadgeRepository userBadgeRepository,
            IXPTransactionRepository xpRepository)
        {
            _streakRepository = streakRepository;
            _badgeRepository = badgeRepository;
            _userBadgeRepository = userBadgeRepository;
            _xpRepository = xpRepository;
        }

        /// <summary>
        /// Record daily activity (call when student completes a lesson, watches video, etc.)
        /// Returns updated streak info + any new XP earned
        /// </summary>
        [HttpPost("record-activity")]
        public async Task<IActionResult> RecordActivity()
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();

            var now = LocalDateTime.FromDateTime(DateTime.UtcNow);
            var today = new LocalDate(now.Year, now.Month, now.Day);

            var streak = await _streakRepository.GetObjectAsync(x => x.IdStudent == userId);
            int xpEarned = 0;
            bool isNewDay = false;

            if (streak == null || streak.Id == 0)
            {
                // First ever activity
                streak = new LearningStreakEntities
                {
                    IdStudent = userId,
                    CurrentStreak = 1,
                    LongestStreak = 1,
                    LastActivityDate = now,
                    TotalDaysLearned = 1
                };
                await _streakRepository.AddAsync(streak);
                isNewDay = true;
            }
            else
            {
                var lastDate = new LocalDate(streak.LastActivityDate.Year, streak.LastActivityDate.Month, streak.LastActivityDate.Day);
                var daysDiff = Period.Between(lastDate, today, PeriodUnits.Days).Days;

                if (daysDiff == 0)
                {
                    // Same day — no streak change
                    isNewDay = false;
                }
                else if (daysDiff == 1)
                {
                    // Consecutive day — streak continues!
                    streak.CurrentStreak++;
                    streak.TotalDaysLearned++;
                    streak.LastActivityDate = now;
                    if (streak.CurrentStreak > streak.LongestStreak)
                        streak.LongestStreak = streak.CurrentStreak;
                    await _streakRepository.UpdateAsync(streak);
                    isNewDay = true;

                    // Streak bonuses
                    if (streak.CurrentStreak == 7) xpEarned += XP_STREAK_BONUS_7;
                    if (streak.CurrentStreak == 30) xpEarned += XP_STREAK_BONUS_30;
                }
                else
                {
                    // Streak broken
                    streak.CurrentStreak = 1;
                    streak.TotalDaysLearned++;
                    streak.LastActivityDate = now;
                    await _streakRepository.UpdateAsync(streak);
                    isNewDay = true;
                }
            }

            // Award daily XP
            if (isNewDay)
            {
                xpEarned += XP_LESSON_COMPLETE;
                await _xpRepository.AddAsync(new XPTransactionEntities
                {
                    IdStudent = userId,
                    Points = xpEarned,
                    Reason = "daily_activity"
                });
            }

            return Ok(new RepositoryModel<object>
            {
                Data = new
                {
                    currentStreak = streak.CurrentStreak,
                    longestStreak = streak.LongestStreak,
                    totalDaysLearned = streak.TotalDaysLearned,
                    xpEarned,
                    isNewDay
                },
                StatusCode = (int)HttpStatusCode.OK
            });
        }

        /// <summary>
        /// Award XP for specific actions
        /// </summary>
        [HttpPost("award-xp")]
        public async Task<IActionResult> AwardXP([FromBody] AwardXPRequest request)
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();

            int points = request.Reason switch
            {
                "lesson_complete" => XP_LESSON_COMPLETE,
                "quiz_pass" => XP_QUIZ_PASS,
                "review_given" => XP_REVIEW_GIVEN,
                "first_purchase" => XP_FIRST_PURCHASE,
                _ => 5
            };

            await _xpRepository.AddAsync(new XPTransactionEntities
            {
                IdStudent = userId,
                Points = points,
                Reason = request.Reason,
                IdCourse = request.IdCourse
            });

            return Ok(new RepositoryModel<int> { Data = points, StatusCode = (int)HttpStatusCode.OK });
        }

        /// <summary>
        /// Get student's gamification profile
        /// </summary>
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();

            var streak = await _streakRepository.GetObjectAsync(x => x.IdStudent == userId);
            var xpTransactions = await _xpRepository.GetAsync(x => x.IdStudent == userId);
            var totalXP = xpTransactions.Sum(x => x.Points);
            var level = CalculateLevel(totalXP);

            var userBadges = await _userBadgeRepository.GetAsync(x => x.IdStudent == userId);
            var allBadges = await _badgeRepository.GetAsync(x => x.Status == (int)EStatus.Active);

            var earnedBadgeIds = userBadges.Select(b => b.IdBadge).ToHashSet();
            var badgeList = allBadges.Select(b => new
            {
                b.Id,
                b.Name,
                b.Description,
                b.IconEmoji,
                isEarned = earnedBadgeIds.Contains(b.Id),
                earnedDate = userBadges.FirstOrDefault(ub => ub.IdBadge == b.Id)?.EarnedDate.ToString()
            }).ToList();

            return Ok(new RepositoryModel<object>
            {
                Data = new
                {
                    totalXP,
                    level,
                    xpToNextLevel = (level + 1) * 500 - totalXP,
                    streak = streak != null ? new
                    {
                        streak.CurrentStreak,
                        streak.LongestStreak,
                        streak.TotalDaysLearned
                    } : null,
                    badges = badgeList
                },
                StatusCode = (int)HttpStatusCode.OK
            });
        }

        /// <summary>
        /// Get XP leaderboard (top 20)
        /// </summary>
        [HttpGet("leaderboard")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLeaderboard()
        {
            var allXP = await _xpRepository.GetAsync(x => x.Status == (int)EStatus.Active);

            var leaderboard = allXP
                .GroupBy(x => x.IdStudent)
                .Select(g => new { IdStudent = g.Key, TotalXP = g.Sum(x => x.Points) })
                .OrderByDescending(x => x.TotalXP)
                .Take(20)
                .Select((x, i) => new { rank = i + 1, x.IdStudent, x.TotalXP, level = CalculateLevel(x.TotalXP) })
                .ToList();

            return Ok(new RepositoryModel<object> { Data = leaderboard, StatusCode = (int)HttpStatusCode.OK });
        }

        private static int CalculateLevel(int totalXP)
        {
            // Level formula: every 500 XP = 1 level
            return Math.Max(1, totalXP / 500 + 1);
        }
    }

    public class AwardXPRequest
    {
        public string Reason { get; set; } = string.Empty;
        public long? IdCourse { get; set; }
    }
}
