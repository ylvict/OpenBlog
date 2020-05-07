using Microsoft.AspNetCore.Mvc;
using OpenBlog.Web.Models;
using OpenBlog.Web.WebFramework.Sessions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenBlog.Web.Controllers
{
    public class SeoController : BaseMvcController
    {
        private readonly IRequestSession _requestSession;

        public SeoController(IRequestSession requestSession)
        {
            _requestSession = requestSession ?? throw new System.ArgumentNullException(nameof(requestSession));
        }

        [HttpGet]
        public ContentResult RobotsPolicy()
        {
            StringBuilder sbRebots = new StringBuilder();
            sbRebots.AppendLine("User-Agent: *");
            sbRebots.Append(@"
Disallow: /account/*
Disallow: /cdn-cgi/*
");
            sbRebots.AppendLine(string.Format("Sitemap: {0}/sitemap.xml", _requestSession.Host));
            return Content(sbRebots.ToString(), "text/plain");
        }

        /// <summary>
        /// General Sitemap
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Sitemap()
        {
            Response.ContentType = "text/xml";
            var sitemapList = new List<SitemapModel>
            {
                new SitemapModel() { loc = string.Format("{0}/{1}.xml", _requestSession.Host, "generalsitemap"), lastmod = DateTime.UtcNow.Date },
                new SitemapModel() { loc = string.Format("{0}/{1}.xml", _requestSession.Host, "chinese/simplified/sitemap"), lastmod = DateTime.UtcNow.Date },
                new SitemapModel() { loc = string.Format("{0}/{1}.xml", _requestSession.Host, "chinese/traditional/sitemap"), lastmod = DateTime.UtcNow.Date }
            };
            return View("SitemapIndex", sitemapList);
        }

        public ActionResult Links()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GeneralSitemap()
        {
            Response.ContentType = "text/xml";

            var sitemapModel = new List<SitemapUrlModel>
            {
                new SitemapUrlModel() { loc = _requestSession.Host }// Home Page
            };
            return View("Sitemap", sitemapModel);
        }
    }
}