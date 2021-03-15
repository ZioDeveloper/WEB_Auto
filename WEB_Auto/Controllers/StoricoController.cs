using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_Auto.Models;

namespace WEB_Auto.Controllers
{
    public class StoricoController : Controller
    {
        // GET: Storico
        private wisedbEntities db = new wisedbEntities();
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
            var model = new Models.HomeModel();

            var Chassis = from m in db.WEB_ListaPerizieFlat_MVC_vw
                        where m.Telaio == myTelaio 
                        select m;
            model.WEB_ListaPerizieFlat_MVC_vw = Chassis.ToList();
            ViewBag.Chassis = myTelaio;


            return View(model);
        }
    }
}