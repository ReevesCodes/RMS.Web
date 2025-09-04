using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EcommerceRMS.Controllers
{
    public class SitemapController : Controller
    {
        [Route("sitemap.xml")]
        public IActionResult SitemapXml()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}"; // ← dynamic base URL

            var urls = new List<string>
            {
                $"{baseUrl}/",
                $"{baseUrl}/shop",
                $"{baseUrl}/cart"
            };

            var xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
            xml += "<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">\n";

            foreach (var url in urls)
            {
                xml += "  <url>\n";
                xml += $"    <loc>{url}</loc>\n";
                xml += "    <priority>0.8</priority>\n";
                xml += "  </url>\n";
            }

            xml += "</urlset>";

            return Content(xml, "application/xml");
        }
    }
}
