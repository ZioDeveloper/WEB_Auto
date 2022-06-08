using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_Auto.Models;

namespace WEB_Auto.Controllers
{
    public class PDFViaggioController : Controller
    {
        // GET: PDFViaggio
        private wisedbEntities db = new wisedbEntities();
        
        public ActionResult InsertPDFViaggio()
        {
            var model = new Models.HomeModel();
            DateTime ini = DateTime.Today.AddDays(-7);
            DateTime end = DateTime.Today.AddDays(+7);
            


            
                var Spedizioni = from m in db.AGR_SpedizioniWEB_vw
                                 where m.DataInizioImbarco >= ini
                                 where m.DataInizioImbarco <= end
                                
                                 where m.IDCliente == "51" || m.IDCliente == "GN"
                                 select m;
                // model.AGR_SpedizioniWEB_vw = Spedizioni.ToList().OrderBy(s => s.DataInizioImbarco).OrderBy(s => s.IDPortoImbarco).OrderBy(s => s.IDPortoSbarco).OrderBy(s => s.IDOriginale1);
                model.AGR_SpedizioniWEB_vw = Spedizioni.ToList().OrderBy(s => s.DataInizioImbarco).OrderBy(s => s.DataInizioImbarco);
                
                var ElencoSpedizioni = new SelectList(model.AGR_SpedizioniWEB_vw.ToList(), "ID", "DescrAlt");
               
                List<SelectListItem> Elenco = new List<SelectListItem>();
                    Elenco.Add(new SelectListItem() { Text = "Partenza", Value = "P" });
                    Elenco.Add(new SelectListItem() { Text = "Arrivo", Value = "A" });
                    

                var ElencoTP = new SelectList(Elenco, "Value", "Text");
            ViewData["ElencoSpedizioni"] = ElencoSpedizioni;


                ViewData["ElencoTP"] = ElencoTP;

            return View();
        }

        public ActionResult Exec (string IDSpedizione, string IDTipoViaggio)
        {

            return RedirectToAction("InsertPDFViaggio");
        }
    }
}