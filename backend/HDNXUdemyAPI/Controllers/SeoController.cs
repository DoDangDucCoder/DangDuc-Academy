using Asp.Versioning;
using HDNXUdemyData.IRepository;
using HDNXUdemyModel.Constant;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace HDNXUdemyAPI.Controllers
{
    /// <summary>
    /// SEO utilities: Sitemap.xml and robots.txt generation
    /// </summary>
    [ApiVersion(VersionApi.Version1)]
    [Route("api/v{version:apiVersion}/Seo")]
    public class SeoController : BaseController
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICategoryRepository _categoryRepository;

        public SeoController(ICourseRepository courseRepository, ICategoryRepository categoryRepository)
        {
            _courseRepository = courseRepository;
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Generate sitemap.xml for search engine crawlers
        /// </summary>
        [HttpGet("sitemap.xml")]
        [Produces("application/xml")]
        public async Task<IActionResult> Sitemap()
        {
            var baseUrl = "https://web-hdnx.devproinsights.com";
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            // Homepage
            sb.AppendLine($"  <url><loc>{baseUrl}/</loc><changefreq>daily</changefreq><priority>1.0</priority></url>");

            // Categories
            var categories = await _categoryRepository.GetAsync(x => x.Status == (int)EStatus.Active);
            foreach (var cat in categories)
            {
                sb.AppendLine($"  <url><loc>{baseUrl}/courses?category={cat.Id}</loc><changefreq>weekly</changefreq><priority>0.8</priority></url>");
            }

            // Courses
            var courses = await _courseRepository.GetAsync(x => x.Status == (int)EStatus.Active);
            foreach (var course in courses)
            {
                sb.AppendLine($"  <url><loc>{baseUrl}/course/{course.Id}</loc><changefreq>weekly</changefreq><priority>0.7</priority></url>");
            }

            sb.AppendLine("</urlset>");
            return Content(sb.ToString(), "application/xml", Encoding.UTF8);
        }

        /// <summary>
        /// robots.txt for search engine directives
        /// </summary>
        [HttpGet("robots.txt")]
        [Produces("text/plain")]
        public IActionResult Robots()
        {
            var baseUrl = "https://web-hdnx.devproinsights.com";
            var content = $@"User-agent: *
Allow: /
Disallow: /admin/
Disallow: /api/
Disallow: /checkout/
Disallow: /account/

Sitemap: {baseUrl}/api/v1/Seo/sitemap.xml";

            return Content(content, "text/plain");
        }

        /// <summary>
        /// Get SEO metadata for a specific course (Open Graph + JSON-LD)
        /// </summary>
        [HttpGet("course-meta/{idCourse}")]
        public async Task<IActionResult> GetCourseMeta(long idCourse)
        {
            var course = await _courseRepository.GetByIdAsync(idCourse);
            if (course == null)
                return NotFound();

            var meta = new
            {
                openGraph = new
                {
                    og_title = course.Title,
                    og_description = course.ShortDescription,
                    og_image = course.PictureUrl,
                    og_type = "website",
                    og_url = $"https://web-hdnx.devproinsights.com/course/{course.Id}"
                },
                jsonLd = new
                {
                    context = "https://schema.org",
                    type = "Course",
                    name = course.Title,
                    description = course.ShortDescription,
                    provider = new { type = "Organization", name = "HDNX Academy" },
                    image = course.PictureUrl,
                    offers = new
                    {
                        type = "Offer",
                        price = course.IsFree ? 0 : (course.IsDiscount ? course.PriceOfDiscount : course.PriceOfCourse),
                        priceCurrency = "USD",
                        availability = "https://schema.org/InStock"
                    },
                    aggregateRating = new
                    {
                        type = "AggregateRating",
                        ratingValue = 4.5,
                        reviewCount = course.TotalStudentRegister
                    }
                },
                twitter = new
                {
                    card = "summary_large_image",
                    title = course.Title,
                    description = course.ShortDescription,
                    image = course.PictureUrl
                }
            };

            return Ok(meta);
        }
    }
}
