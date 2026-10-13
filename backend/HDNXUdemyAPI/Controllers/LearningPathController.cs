using Asp.Versioning;
using HDNXUdemyData.Entities;
using HDNXUdemyData.IRepository;
using HDNXUdemyModel.Base;
using HDNXUdemyModel.Constant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HDNXUdemyAPI.Controllers
{
    /// <summary>
    /// Learning Paths — guided course sequences
    /// </summary>
    [ApiVersion(VersionApi.Version1)]
    [Route("api/v{version:apiVersion}/LearningPath")]
    public class LearningPathController : BaseController
    {
        private readonly ILearningPathRepository _pathRepository;
        private readonly ILearningPathCourseRepository _pathCourseRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ICoursePrerequisiteRepository _prerequisiteRepository;

        public LearningPathController(
            ILearningPathRepository pathRepository,
            ILearningPathCourseRepository pathCourseRepository,
            ICourseRepository courseRepository,
            ICoursePrerequisiteRepository prerequisiteRepository)
        {
            _pathRepository = pathRepository;
            _pathCourseRepository = pathCourseRepository;
            _courseRepository = courseRepository;
            _prerequisiteRepository = prerequisiteRepository;
        }

        /// <summary>
        /// Get all learning paths
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var paths = await _pathRepository.GetAsync(x => x.Status == (int)EStatus.Active);
            var result = paths.Select(p => new
            {
                p.Id, p.Title, p.Description, p.PictureUrl,
                p.LevelRequired, p.EstimatedHours, p.IdCategory
            }).ToList();
            return Ok(new RepositoryModel<object> { Data = result, StatusCode = (int)HttpStatusCode.OK });
        }

        /// <summary>
        /// Get learning path with its courses (ordered)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var path = await _pathRepository.GetByIdAsync(id);
            if (path == null) return NotFound();

            var pathCourses = await _pathCourseRepository.GetAsync(x => x.IdLearningPath == id);
            var courseDetails = new List<object>();
            foreach (var pc in pathCourses.OrderBy(x => x.OrderIndex))
            {
                var course = await _courseRepository.GetByIdAsync(pc.IdCourse);
                if (course != null)
                {
                    courseDetails.Add(new
                    {
                        pc.OrderIndex, pc.IsRequired,
                        courseId = course.Id, course.Title, course.PictureUrl,
                        course.Duration, course.PriceOfCourse, course.IsFree
                    });
                }
            }

            return Ok(new RepositoryModel<object>
            {
                Data = new
                {
                    path.Id, path.Title, path.Description, path.PictureUrl,
                    path.LevelRequired, path.EstimatedHours,
                    courses = courseDetails,
                    totalCourses = courseDetails.Count
                },
                StatusCode = (int)HttpStatusCode.OK
            });
        }

        /// <summary>
        /// Create learning path (admin)
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateLearningPathRequest request)
        {
            var path = new LearningPathEntities
            {
                Title = request.Title,
                Description = request.Description,
                IdCategory = request.IdCategory,
                PictureUrl = request.PictureUrl,
                LevelRequired = request.LevelRequired,
                EstimatedHours = request.EstimatedHours
            };
            await _pathRepository.AddAsync(path);
            return Ok(new RepositoryModel<long> { Data = path.Id, StatusCode = (int)HttpStatusCode.OK });
        }

        /// <summary>
        /// Add course to learning path
        /// </summary>
        [HttpPost("{id}/courses")]
        [Authorize]
        public async Task<IActionResult> AddCourse(long id, [FromBody] AddPathCourseRequest request)
        {
            await _pathCourseRepository.AddAsync(new LearningPathCourseEntities
            {
                IdLearningPath = id,
                IdCourse = request.IdCourse,
                OrderIndex = request.OrderIndex,
                IsRequired = request.IsRequired
            });
            return Ok(new RepositoryModel<bool> { Data = true, StatusCode = (int)HttpStatusCode.OK });
        }

        /// <summary>
        /// Get prerequisites for a course
        /// </summary>
        [HttpGet("prerequisites/{idCourse}")]
        public async Task<IActionResult> GetPrerequisites(long idCourse)
        {
            var prereqs = await _prerequisiteRepository.GetAsync(x => x.IdCourse == idCourse);
            var courses = new List<object>();
            foreach (var p in prereqs)
            {
                var course = await _courseRepository.GetByIdAsync(p.IdPrerequisiteCourse);
                if (course != null)
                    courses.Add(new { course.Id, course.Title, course.PictureUrl });
            }
            return Ok(new RepositoryModel<object> { Data = courses, StatusCode = (int)HttpStatusCode.OK });
        }

        /// <summary>
        /// Set prerequisites for a course (admin/instructor)
        /// </summary>
        [HttpPost("prerequisites")]
        [Authorize]
        public async Task<IActionResult> SetPrerequisite([FromBody] SetPrerequisiteRequest request)
        {
            var existing = await _prerequisiteRepository.GetObjectAsync(
                x => x.IdCourse == request.IdCourse && x.IdPrerequisiteCourse == request.IdPrerequisiteCourse);
            if (existing != null && existing.Id > 0)
                return Ok(new RepositoryModel<bool> { Data = true, PartnerCode = "Already set" });

            await _prerequisiteRepository.AddAsync(new CoursePrerequisiteEntities
            {
                IdCourse = request.IdCourse,
                IdPrerequisiteCourse = request.IdPrerequisiteCourse
            });
            return Ok(new RepositoryModel<bool> { Data = true, StatusCode = (int)HttpStatusCode.OK });
        }
    }

    public class CreateLearningPathRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long IdCategory { get; set; }
        public string? PictureUrl { get; set; }
        public string LevelRequired { get; set; } = "Beginner";
        public int EstimatedHours { get; set; }
    }

    public class AddPathCourseRequest
    {
        public long IdCourse { get; set; }
        public int OrderIndex { get; set; }
        public bool IsRequired { get; set; } = true;
    }

    public class SetPrerequisiteRequest
    {
        public long IdCourse { get; set; }
        public long IdPrerequisiteCourse { get; set; }
    }
}
