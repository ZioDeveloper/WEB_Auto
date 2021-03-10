using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WEB_Auto.Controllers
{
    public class StoricoController : Controller
    {
        // GET: Storico
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CercaTelaio()
        {
            return View();
        }

        public ActionResult CercaTelaioSingolo(string aTelaio)
        {
            string myTelaio = aTelaio.ToUpper();

            return View();
        }
    }
}