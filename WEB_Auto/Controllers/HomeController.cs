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

            Session["User"] = usr;

            if (String.IsNullOrEmpty(usr))
                usr = "Pierangeli";

            var myIDPerito = (from s in db.AGR_Periti_WEB
                              where s.Name == usr
                              select s.IDPerito).FirstOrDefault();

            var myIDPorto = (from s in db.AGR_Periti_WEB
                              where s.Name == usr
                              select s.IDPorto).FirstOrDefault();

            var datiperito = from m in db.Periti
                    where m.IDModem == myIDPerito.ToString()
                    select m;
            model.Periti = datiperito.ToList();

            var datiporto = from m in db.AGR_Porti
                             where m.ID == myIDPorto.ToString()
                             select m;
            model.AGR_Porti = datiporto.ToList();


            var myPerito = from m in db.AGR_Periti_WEB
                             where m.Name == usr
                             select m;
            model.AGR_Periti_WEB = myPerito.ToList();

            // Dati per dropdown spedizioni
            DateTime periodo = DateTime.Today.AddDays(-4);
            var Spedizioni = from m in db.AGR_SpedizioniWEB_vw
                             where m.DataInizioImbarco >= periodo
                             where ( m.IDPortoImbarco == myIDPorto || m.IDPortoSbarco == myIDPorto)

                             where m.IDCliente == "51"
                             select m;
            model.AGR_SpedizioniWEB_vw = Spedizioni.ToList();

            var ElencoSpedizioni = new SelectList(model.AGR_SpedizioniWEB_vw.ToList(), "ID", "Descr");

            // Dati per dropdown Meteo
            var Meteo = from m in db.AGR_Meteo
                             select m;
            model.AGR_Meteo = Meteo.ToList();

            var ElencoMeteo = new SelectList(model.AGR_Meteo.ToList(), "ID", "DescrITA");

            // Dati per dropdown TipoPErizia
            var TP = from m in db.AGR_TipiPerizia
                     where m.ID == "C" ||
                           m.ID == "D"
                     select m;
            model.AGR_TipiPerizia = TP.ToList();
            var ElencoTP = new SelectList(model.AGR_TipiPerizia.ToList(), "ID", "DescrITA");

            



            Session["User"] = usr;
            ViewData["ElencoSpedizioni"] = ElencoSpedizioni;
            ViewData["ElencoMeteo"] = ElencoMeteo;
            ViewData["ElencoTP"] = ElencoTP;

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Autocargo - Rilevamento danni";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contatti per assistenza.";

            return View();
        }

        [HttpPost]
        public ActionResult DatiPerizia(string IDSpedizione, string IDMeteo , string IDTP )
        {
            var model = new Models.HomeModel();

            // COME RECUPERARE CAMPI DA TABELLA/VISTA = select new{m. ecc ecc}

            var Casa = (from m in db.AGR_SpedizioniWEB_vw
                              where m.ID == IDSpedizione
                              select new { m.IDCliente, m.IDCasa }).FirstOrDefault();

            string aIDCliente = Casa.IDCliente;
            string aCasa = Casa.IDCasa;

            var Spedizioni = from m in db.AGR_SpedizioniWEB_Decoded_vw
                              where m.ID == IDSpedizione
                              select m;
            model.AGR_SpedizioniWEB_Decoded_vw = Spedizioni.ToList();

            var TP = from m in db.AGR_TipiPerizia
                     where m.ID == IDTP
                     select m;
            model.AGR_TipiPerizia = TP.ToList();

            // Dati per dropdown Modello
            var modello = from m in db.AGR_ModelliAuto
                          where m.IDCliente == "**"
                          where m.IDCasa == aCasa
                          select m;
            model.AGR_ModelliAuto = modello.ToList();
            var ElencoModelli = new SelectList(model.AGR_ModelliAuto.ToList(), "IDModelloCasa", "Descr");
            ViewData["ElencoModelli"] = ElencoModelli;

            // Dati per dropdown Trasportatore Grimaldi
            var TraspGrim = from m in db.AGR_TrasportatoriGrimaldi
                            where m.Descr.ToString().Substring(0,3) != "***"
                            select m;
            model.AGR_TrasportatoriGrimaldi = TraspGrim.ToList().OrderBy(m=>m.Descr);
            var ElencoTraspGrim = new SelectList(model.AGR_TrasportatoriGrimaldi.ToList(), "ID", "Descr");
            ViewData["ElencoTraspGrim"] = ElencoTraspGrim;

            // Dati per dropdown Tipo rotabile
            var TipoRotabile = from m in db.AGR_TipoRotabile
                               select m;
            model.AGR_TipoRotabile = TipoRotabile.ToList();
            var ElencoTipoRotabile = new SelectList(model.AGR_TipoRotabile.ToList(), "ID", "DescrITA");
            ViewData["ElencoTipoRotabile"] = ElencoTipoRotabile;


            return View(model);
        }

        [HttpPost]
        public ActionResult SalvaPeriziaTesta(string  Chassis, string DataPerizia, string IDModelloCasa, string IDTrasportatoreGrim, 
                                              string IDTipoRotabile, bool? isDamaged)
        {
            
            return View();
        }
    }


}