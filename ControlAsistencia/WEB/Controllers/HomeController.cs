using DTO.Empleados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WEB.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            proxy.ControlAsistenciaClient sr = new proxy.ControlAsistenciaClient();
            List<dtoDepartamento> dtos = new List<dtoDepartamento>();
            var lst = sr.getDepartamento(string.Empty).ToList();
            foreach (var item in lst)
            {
                dtoDepartamento dto = new dtoDepartamento();
                dto.DEPTID = item.DEPTID;
                dto.DEPTNAME = item.DEPTNAME;
                dto.SUPDEPTID = item.SUPDEPTID;
                dtos.Add(dto);
            }
            return View(dtos);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}