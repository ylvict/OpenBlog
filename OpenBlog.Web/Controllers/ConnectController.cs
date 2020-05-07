using System.Text;
using Microsoft.AspNetCore.Mvc;
using OpenBlog.Web.WebFramework.Sessions;

namespace OpenBlog.Web.Controllers
{
    public class ConnectController : BaseMvcController
    {
        private readonly IRequestSession _requestSession;

        public ConnectController(IRequestSession requestSession)
        {
            _requestSession = requestSession;
        }

        [HttpGet]
        public IActionResult WlwManifest()
        {
            Response.ContentType = "text/xml";
            return View();
        }


        [HttpGet]
        public IActionResult Rsd()
        {
            Response.ContentType = "text/xml";
            return View();
        }

        [HttpGet]
        public IActionResult Rss()
        {
            Response.ContentType = "text/xml";
            return View();
        }
    }
}