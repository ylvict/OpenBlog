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
            sbRebots.AppendLine($"Sitemap: {_requestSession.Host}/sitemap.xml");
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
                new SitemapModel() { loc = $"{_requestSession.Host}/{"generalsitemap"}.xml", lastmod = DateTime.UtcNow.Date },
                new SitemapModel() { loc = $"{_requestSession.Host}/{"blog/sitemap"}.xml", lastmod = DateTime.UtcNow.Date }
            };
            return View("SitemapIndex", sitemapList);
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