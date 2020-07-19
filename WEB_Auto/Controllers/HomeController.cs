using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_Auto.Models;
namespace WEB_Auto.Controllers
{
    public class HomeController : Controller
    {

        private wisedbEntities db  = new wisedbEntities();

        public ActionResult Index(string usr)
        {
            var model = new Models.HomeModel();


            if (String.IsNullOrEmpty(usr))
                usr = "Pierangeli";

            var myIDPerito = (from s in db.AGR_Periti_WEB
                              where s.Name == usr
                              select s.IDPerito).FirstOrDefault();

            var datiperito = from m in db.Periti
                    where m.IDModem == myIDPerito.ToString()
                    select m;
            model.Periti = datiperito.ToList();


            var myPerito = from m in db.AGR_Periti_WEB
                             where m.Name == usr
                             select m;
            model.AGR_Periti_WEB = myPerito.ToList();

            DateTime periodo = DateTime.Today.AddDays(-4);
            var Spedizioni = from m in db.AGR_SpedizioniWEB_vw
                             where m.DataInizioImbarco > periodo
                             where m.IDCliente == "51"
                             select m;
            model.AGR_SpedizioniWEB_vw = Spedizioni.ToList();

            var ElencoSpedizioni = new SelectList(model.AGR_SpedizioniWEB_vw.ToList(), "ID", "Descr");

            Session["User"] = usr;
            ViewData["ElencoSpedizioni"] = ElencoSpedizioni;

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}