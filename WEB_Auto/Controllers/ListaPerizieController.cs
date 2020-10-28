using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_Auto.Models;
namespace WEB_Auto.Controllers
{
    public class ListaPerizieController : Controller
    {
        private wisedbEntities db = new wisedbEntities();

        // GET: ListaPerizie
        public ActionResult ListaPerizie()
        {

            var model = new Models.HomeModel();
            #region Codice Commentato
            //string usr = Session["USer"].ToString(); ;

            //var myIDPerito = (from s in db.AGR_Periti_WEB
            //                  where s.Name == usr
            //                  select s.IDPerito).FirstOrDefault();

            //var myIDPorto = (from s in db.AGR_Periti_WEB
            //                 where s.Name == usr
            //                 select s.IDPorto).FirstOrDefault();

            //var datiperito = from m in db.Periti
            //                 where m.IDModem == myIDPerito.ToString()
            //                 select m;
            //model.Periti = datiperito.ToList();

            //var datiporto = from m in db.AGR_Porti
            //                where m.ID == myIDPorto.ToString()
            //                select m;
            //model.AGR_Porti = datiporto.ToList();


            //var myPerito = from m in db.AGR_Periti_WEB
            //               where m.Name == usr
            //               select m;
            //model.AGR_Periti_WEB = myPerito.ToList();

            //// Dati per dropdown spedizioni
            //DateTime ini = DateTime.Today.AddDays(-1);
            //DateTime end = DateTime.Today.AddDays(+1);
            //var Spedizioni = from m in db.AGR_SpedizioniWEB_vw
            //                 where m.DataInizioImbarco >= ini
            //                 where m.DataInizioImbarco <= end
            //                 where (m.IDPortoImbarco == myIDPorto || m.IDPortoSbarco == myIDPorto)
            //                 where m.IDCliente == "51" || m.IDCliente == "GN"
            //                 select m;
            //model.AGR_SpedizioniWEB_vw = Spedizioni.ToList().OrderBy(s => s.ID);
            #endregion

            return View();
        }
    }
}