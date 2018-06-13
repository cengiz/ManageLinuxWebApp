using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace dotnetCoreManageLinux.Web.Controllers
{
    public class DockerController : Controller
    {
        public IActionResult Index()
        {
            string cmdRes = Helper.RunShellCommand("docker ps -a");

            ViewData["Message"] = cmdRes;

            return View();
        }

        public IActionResult GetDockerImages()
        {

            return View();
        }
    }
}