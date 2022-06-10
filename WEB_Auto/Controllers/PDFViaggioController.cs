using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WEB_Auto.Models;

namespace WEB_Auto.Controllers
{
    public class PDFViaggioController : Controller
    {
        // GET: PDFViaggio
        private wisedbEntities db = new wisedbEntities();
        

        public ActionResult InsertPDFViaggio(string IDSpedizione)
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

                var myFoto = (from f in db.WEB_AUTO_PDF
                              where f.IDPerizia == IDSpedizione
                              select f);
                model.WEB_AUTO_PDF = myFoto.ToList();
                UpdateModel(myFoto);





            return View(model);
        }


        [HttpPost]
        public ActionResult InsertPDFViaggio(string IDSpedizione, string IDTipoViaggio)
        {
            var model = new Models.HomeModel();
            var myFoto = (from f in db.WEB_AUTO_PDF
                          where f.IDPerizia == IDSpedizione
                          select f);
            model.WEB_AUTO_PDF = myFoto.ToList();
            UpdateModel(myFoto);

            ViewBag.IDSpedizione = IDSpedizione;

            //return View("ScattaPDFSpedizione", myFoto);
            return RedirectToAction("InsertPDFViaggio" , new { IDSpedizione});
        }

        public ActionResult ScattaPDFSpedizione(string myIDSpedizione)
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
            Elenco.Add(new SelectListItem() { Text = "Partenza", Value = "2" });
            Elenco.Add(new SelectListItem() { Text = "Arrivo", Value = "4" });


            var ElencoTP = new SelectList(Elenco, "Value", "Text");
            ViewData["ElencoSpedizioni"] = ElencoSpedizioni;


            ViewData["ElencoTP"] = ElencoTP;


            var myFoto = (from f in db.WEB_AUTO_PDF
                          where f.IDPerizia == myIDSpedizione
                          select f);
            model.WEB_AUTO_PDF = myFoto.ToList();
            UpdateModel(myFoto);

            ViewBag.IDSpedizione = myIDSpedizione;

            //if(String.IsNullOrEmpty(myIDSpedizione))
            //{
            //    ViewBag.Message = "Selezionare Viaggio e tipo PDF";
            //}
            //else
            //{
            //    ViewBag.Message = "";
            //}

            return View("ScattaPDFSpedizione", myFoto);



        }

        public ActionResult UploadPDFSpedizione(IEnumerable<HttpPostedFileBase> files, string IDSpedizione, string IDTipoViaggio)
        {
            string filename = "";
            string path = "";
            string pathtrimmed = "";

            if (!String.IsNullOrEmpty(IDSpedizione) && !String.IsNullOrEmpty(IDTipoViaggio))
            {
                ViewBag.Message = "";
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        filename = System.IO.Path.GetFileName(file.FileName);

                        path = System.IO.Path.Combine(Server.MapPath("~/DocumentiXTelai/PDF"), filename);
                        if (file != null)
                        {
                            pathtrimmed = Regex.Replace(path, @"\s", "");
                            file.SaveAs(pathtrimmed);
                        }
                        int cnt = 0;
                        try
                        {
                            cnt = (int)(from m in db.WEB_AUTO_PDF
                                        where m.IDPerizia == IDSpedizione
                                        select m.Prog).Max();
                        }
                        catch { }
                        cnt++;
                        filename = Regex.Replace(filename, @"\s", "");

                        var sql = @"INSERT INTO WEB_AUTO_PDF (IDPerizia, FileName,Prog,Flags,IsSent) Values (@IDPerizia, @FileName,@Prog,@Flags,0)";
                        int noOfRowInserted = db.Database.ExecuteSqlCommand(sql,
                            new SqlParameter("@IDPerizia", IDSpedizione),
                            new SqlParameter("@FileName", filename),
                            new SqlParameter("@Flags", IDTipoViaggio),
                            new SqlParameter("@Prog", cnt));
                        
                    }
                }
            }
            else 
            {
                if(!String.IsNullOrEmpty(IDSpedizione) && String.IsNullOrEmpty(IDTipoViaggio))
                    ViewBag.Message = "Selezionare partenza o arrivo !";
                if (String.IsNullOrEmpty(IDSpedizione) && !String.IsNullOrEmpty(IDTipoViaggio))
                    ViewBag.Message = "Selezionare Viaggio";
                if (String.IsNullOrEmpty(IDSpedizione) && String.IsNullOrEmpty(IDTipoViaggio))
                    ViewBag.Message = "Selezionare Viaggio e partenza o arrivo";
            }

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
            Elenco.Add(new SelectListItem() { Text = "Partenza", Value = "2" });
            Elenco.Add(new SelectListItem() { Text = "Arrivo", Value = "4" });


            var ElencoTP = new SelectList(Elenco, "Value", "Text");
            ViewData["ElencoSpedizioni"] = ElencoSpedizioni;


            ViewData["ElencoTP"] = ElencoTP;

            var myFoto = (from f in db.WEB_AUTO_PDF
                          where f.IDPerizia == IDSpedizione
                          select f);
            model.WEB_AUTO_PDF = myFoto.ToList();
            UpdateModel(myFoto);



            ViewBag.IDSpedizione = IDSpedizione;

            return View("ScattaPDFSpedizione", myFoto);
        }

        public ActionResult CancellaPDFSpedizione(string IDSpedizione, string nomefile)
        {
            var sql = @"DELETE FROM WEB_AUTO_PDF WHERE IDPerizia = @IDDocumento";
            int myRecordCounter = db.Database.ExecuteSqlCommand(sql, new SqlParameter("@IDDocumento", IDSpedizione));

            string fullPath = Request.MapPath("~/DocumentiXTelai/PDF/" + nomefile);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            var model = new Models.HomeModel();



            var myFoto = (from f in db.WEB_AUTO_PDF
                          where f.IDPerizia == IDSpedizione
                          select f);
            model.WEB_AUTO_PDF = myFoto.ToList();

            //ViewBag.IDTelaio = myIDPerizia;
            return RedirectToAction("ScattaPDFSpedizione", "PDFViaggio", new
            {

                IDSpedizione = IDSpedizione

            });

            //return View("ScattaFoto", myIDPerizia);
        }
    }
}