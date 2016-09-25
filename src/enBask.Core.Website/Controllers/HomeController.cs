using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace enBask.Core.Website.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            string data_str = "hello world";
            var data = System.Text.Encoding.UTF8.GetBytes(data_str);
            HttpContext.Session.Set("hello", data);

            HttpContext.Session.TryGetValue("hello", out data);
            if (data != null)
            {
                var new_str = System.Text.Encoding.UTF8.GetString(data);
            }

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
