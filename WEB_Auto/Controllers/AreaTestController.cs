using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_Auto.Models;

namespace WEB_Auto.Controllers
{
    public class AreaTestController : Controller
    {
        private wisedbEntities db = new wisedbEntities();
        // GET: AreaTest
        public ActionResult Index()
        {
            var model = new Models.HomeModel();
            var lista = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                         where m.IDSpedizione == "G0327042"

                         select m).ToList();
            model.WEB_Auto_ListaPerizieXSpedizione_vw = lista;
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(FormCollection formCollection,string Test)
        {
            string[] ids = formCollection["ID"].Split(new char[] { ',' });
            foreach (string id in ids)
            {
                try
                {
                    var perizia = id;
                    string aPerizia = perizia.ToString();
                    if (aPerizia != "false")
                        aPerizia = aPerizia;
                }
                catch { }
            }
            return RedirectToAction("Index");
        }
    }
}