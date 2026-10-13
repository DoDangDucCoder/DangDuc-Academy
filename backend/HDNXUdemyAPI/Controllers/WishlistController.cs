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
    /// Wishlist API — Save/remove courses to favorites
    /// </summary>
    [ApiVersion(VersionApi.Version1)]
    [Route("api/v{version:apiVersion}/Wishlist")]
    [Authorize]
    public class WishlistController : BaseController
    {
        private readonly IBookmarkCourseRepository _bookmarkRepository;
        private readonly ICourseRepository _courseRepository;

        public WishlistController(IBookmarkCourseRepository bookmarkRepository, ICourseRepository courseRepository)
        {
            _bookmarkRepository = bookmarkRepository;
            _courseRepository = courseRepository;
        }

        /// <summary>
        /// Get user's wishlist
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();

            var bookmarks = await _bookmarkRepository.GetAsync(x => x.IdStudent == userId && x.Status == (int)EStatus.Active);
            var result = new List<object>();

            foreach (var b in bookmarks)
            {
                var course = await _courseRepository.GetByIdAsync(b.IdCourse);
                if (course != null)
                {
                    result.Add(new
                    {
                        bookmarkId = b.Id,
                        courseId = course.Id,
                        title = course.Title,
                        pictureUrl = course.PictureUrl,
                        price = course.PriceOfCourse,
                        isDiscount = course.IsDiscount,
                        discountPrice = course.PriceOfDiscount,
                        isFree = course.IsFree,
                        totalStudents = course.TotalStudentRegister,
                        addedDate = b.CreateDate.ToString()
                    });
                }
            }

            return Ok(new RepositoryModel<List<object>> { Data = result, StatusCode = (int)HttpStatusCode.OK });
        }

        /// <summary>
        /// Add course to wishlist
        /// </summary>
        [HttpPost("{idCourse}")]
        public async Task<IActionResult> AddToWishlist(long idCourse)
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();

            var existing = await _bookmarkRepository.GetObjectAsync(
                x => x.IdStudent == userId && x.IdCourse == idCourse && x.Status == (int)EStatus.Active);

            if (existing != null && existing.Id > 0)
                return Ok(new RepositoryModel<bool> { Data = true, StatusCode = (int)HttpStatusCode.OK, PartnerCode = "Already in wishlist" });

            await _bookmarkRepository.AddAsync(new BookmarkCourseEntities
            {
                IdStudent = userId,
                IdCourse = idCourse
            });

            return Ok(new RepositoryModel<bool> { Data = true, StatusCode = (int)HttpStatusCode.OK });
        }

        /// <summary>
        /// Remove course from wishlist
        /// </summary>
        [HttpDelete("{idCourse}")]
        public async Task<IActionResult> RemoveFromWishlist(long idCourse)
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();

            var bookmark = await _bookmarkRepository.GetObjectAsync(
                x => x.IdStudent == userId && x.IdCourse == idCourse && x.Status == (int)EStatus.Active);

            if (bookmark != null && bookmark.Id > 0)
            {
                await _bookmarkRepository.DeleteByKey(bookmark.Id);
            }

            return Ok(new RepositoryModel<bool> { Data = true, StatusCode = (int)HttpStatusCode.OK });
        }

        /// <summary>
        /// Check if course is in wishlist
        /// </summary>
        [HttpGet("check/{idCourse}")]
        public async Task<IActionResult> IsInWishlist(long idCourse)
        {
            var userId = CurrentUserId();
            if (userId == 0) return Unauthorized();

            var exists = await _bookmarkRepository.GetObjectAsync(
                x => x.IdStudent == userId && x.IdCourse == idCourse && x.Status == (int)EStatus.Active);

            return Ok(new RepositoryModel<bool> { Data = exists != null && exists.Id > 0, StatusCode = (int)HttpStatusCode.OK });
        }
    }
}
