using System;
using System.Collections.Generic;
using System.IO;
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

        public ActionResult CercaStoriaTelaio(string aTelaio)
        {
            string myTelaio = aTelaio.ToUpper();
            var model = new Models.HomeModel();

            var Chassis1 = from m in db.WEB_ListaPerizieFlat_MVC_vw
                           where m.Telaio == myTelaio
                           where m.IsClosed == false
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
            model.WEB_ListaPerizieFlat_DEF_vw = Chassis3.ToList().OrderByDescending(m => m.DataPerizia);

            return View(model);
        }

        public ActionResult VisualizzaPreload(string aViaggio)
        {
            var model = new Models.HomeModel();

            if (!String.IsNullOrEmpty(aViaggio))
            {
                var L1 = from m in db.WEB_ListaPerizieFlat_MVC_vw
                         where m.Viaggio == aViaggio
                         where m.IDTipoPerizia == "C"
                         where m.IsClosed == false
                         select m;
                model.WEB_ListaPerizieFlat_MVC_vw = L1.ToList().OrderBy(s => s.Telaio);

                var L2 = from m in db.WEB_ListaPerizieFlat_DEF_ALL_vw
                         where m.Viaggio == aViaggio
                         where m.IDTipoPerizia == "C"
                         select m;
                model.WEB_ListaPerizieFlat_DEF_ALL_vw = L2.ToList().OrderBy(s => s.Telaio);

                var L3 = from m in db.WEB_ListaPerizieFlat_TMP_vw
                         where m.Viaggio == aViaggio
                         where m.IDTipoPerizia == "C"
                         select m;
                model.WEB_ListaPerizieFlat_TMP_vw = L3.ToList().OrderBy(s => s.Telaio);
            }
            else
            {
                var L1 = from m in db.WEB_ListaPerizieFlat_MVC_vw
                         where 0==1
                         where m.Viaggio == aViaggio
                         where m.IDTipoPerizia == "C"
                         select m;
                model.WEB_ListaPerizieFlat_MVC_vw = L1.ToList().OrderBy(s => s.Status);

                var L2 = from m in db.WEB_ListaPerizieFlat_DEF_ALL_vw
                         where 0 == 1
                         where m.Viaggio == aViaggio
                         where m.IDTipoPerizia == "C"
                         select m;
                model.WEB_ListaPerizieFlat_DEF_ALL_vw = L2.ToList().OrderBy(s => s.Status);

                var L3 = from m in db.WEB_ListaPerizieFlat_TMP_vw
                         where 0==1
                         where m.Viaggio == aViaggio
                         where m.IDTipoPerizia == "C"
                         select m;
                model.WEB_ListaPerizieFlat_TMP_vw = L3.ToList().OrderBy(s => s.Status);

            }

            ViewBag.Viaggio = aViaggio;
            return View(model);
        }

        public ActionResult CarouselFotoStoriche(string aIDPerizia, string aTelaio)
        {
            


            var model = new Models.HomeModel();

            var foto = (from m in db.WEB_ListaPerizieFlat_DEF_vw
                        where m.IDPerizia == aIDPerizia
                        select m).ToList();
            model.WEB_ListaPerizieFlat_DEF_vw = foto;
            ViewBag.NumFoto = foto[0].NumFoto;
            ViewBag.IDPErizia = foto[0].IDPerizia;
            ViewBag.Telaio = aTelaio;
            return View(model);
        }
    }
}