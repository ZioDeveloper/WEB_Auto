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

            var Chassis1 = from m in db.WEB_ListaPerizieFlat_MVC_vw
                        where m.Telaio == myTelaio 
                        select m;
            model.WEB_ListaPerizieFlat_MVC_vw = Chassis1.ToList();
            ViewBag.Chassis1 = myTelaio;

            var Chassis2 = from m in db.WEB_ListaPerizieFlat_TMP_vw
                      where m.Telaio == myTelaio
                      select m;
            model.WEB_ListaPerizieFlat_TMP_vw = Chassis2.ToList();

            var Chassis3 = from m in db.WEB_ListaPerizieFlat_DEF_vw
                           where m.Telaio == myTelaio
                           select m;
            model.WEB_ListaPerizieFlat_DEF_vw = Chassis3.ToList().OrderByDescending(m=>m.DataPerizia);

            return View(model);
        }
    }
}