﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WEB_Auto.Models;

namespace WEB_Auto.Controllers
{
    public class DocumentiController : Controller
    {
        private wisedbEntities db = new wisedbEntities();

        // GET: Documenti
        public ActionResult Index(string test)
        {
            return View();
        }

        public ActionResult ScattaFoto(string myIDPerizia, string IDPerito, string IDSpedizione, string IDMeteo, string IDTP, string aIDTrasportatore,
                                         string aIDTipoRotabile, string aIDModelloCasa,string ErrMess="", bool IsUpdate = false)
        {

            var model = new Models.HomeModel();

            

            var myFoto = (from f in db.WEB_AUTO_FOTO
                          where f.IDPerizia == myIDPerizia
                          select f);
            model.WEB_AUTO_FOTO = myFoto.ToList();
            UpdateModel(myFoto);

            ViewBag.IDPerito = IDPerito;
            ViewBag.IDSpedizione = IDSpedizione;
            ViewBag.IDMeteo = IDMeteo;
            ViewBag.IDTP = IDTP;
            ViewBag.aIDTrasportatore = aIDTrasportatore;
            ViewBag.aIDTipoRotabile = aIDTipoRotabile;
            ViewBag.aIDModelloCasa = aIDModelloCasa;
            ViewBag.myIDPerizia = myIDPerizia;
            ViewBag.ErrMess = ErrMess;
            ViewBag.IsUpdate = IsUpdate;
            ViewBag.NumFoto = myFoto.Count();
            return View("ScattaFoto", myFoto);

            //ViewBag.myIDPerizia = myIDPerizia;
            //return View();

        }
        

        public ActionResult UploadFoto(IEnumerable<HttpPostedFileBase> files, string myIDPerizia,string IDPerito, string IDSpedizione , string IDMeteo,
               string IDTP, string aIDTrasportatore, string aIDTipoRotabile, string aIDModelloCasa, string ErrMess = "", bool IsUpdate = false)
        {
            string filename = "";
            string path = "";
            string pathtrimmed = "";


            foreach (var file in files)
            {
                if (file != null)
                {
                    filename = System.IO.Path.GetFileName(file.FileName);

                    path = System.IO.Path.Combine(Server.MapPath("~/DocumentiXTelai/Foto"), filename);
                    if (file != null)
                    {
                        pathtrimmed = Regex.Replace(path, @"\s", "");
                        file.SaveAs(pathtrimmed);
                    }
                    int cnt = 0;
                    try
                    {
                        cnt = (int)(from m in db.WEB_AUTO_FOTO
                                        where m.IDPerizia == myIDPerizia
                                        select m.Prog).Max();
                    }
                    catch {   }
                    cnt++;
                    filename = Regex.Replace(filename, @"\s", "");
                    var sql = @"INSERT INTO WEB_AUTO_FOTO (IDPerizia, FileName,Prog) Values (@IDPerizia, @FileName,@Prog)";
                    int noOfRowInserted = db.Database.ExecuteSqlCommand(sql,
                        new SqlParameter("@IDPerizia", myIDPerizia),
                        new SqlParameter("@FileName", filename),
                        new SqlParameter("@Prog", cnt));
                }
            }

            var model = new Models.HomeModel();

            

            var myFoto = (from f in db.WEB_AUTO_FOTO
                          where f.IDPerizia == myIDPerizia
                          select f);
            model.WEB_AUTO_FOTO = myFoto.ToList();
            UpdateModel(myFoto);

            
            ViewBag.myIDPerizia = myIDPerizia;
            ViewBag.IDPerito = IDPerito;
            ViewBag.IDSpedizione = IDSpedizione;
            ViewBag.IDMeteo = IDMeteo;
            ViewBag.IDTP = IDTP;
            ViewBag.aIDTrasportatore = aIDTrasportatore;
            ViewBag.aIDTipoRotabile = aIDTipoRotabile;
            ViewBag.aIDModelloCasa = aIDModelloCasa;
            ViewBag.ErrMess = ErrMess;
            ViewBag.IsUpdate = IsUpdate;
            ViewBag.NumFoto = myFoto.Count();
            return View("ScattaFoto", myFoto);
        }


        public ActionResult CancellaDocumento(int? IDDocumento ,string myIDPerizia, string nomefile, string IDPerito, string IDSpedizione, string IDMeteo, string IDTP, 
                                             string aIDTrasportatore , string aIDTipoRotabile , string aIDModelloCasa , bool IsUpdate = false)
        {
            var sql = @"DELETE FROM WEB_AUTO_FOTO WHERE ID = @IDDocumento";
            int myRecordCounter = db.Database.ExecuteSqlCommand(sql, new SqlParameter("@IDDocumento", IDDocumento));

            string fullPath = Request.MapPath("~/DocumentiXTelai/Foto/" + nomefile);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            var model = new Models.HomeModel();

            

            var myFoto = (from f in db.WEB_AUTO_FOTO
                          where f.IDPerizia == myIDPerizia
                          select f);
            model.WEB_AUTO_FOTO = myFoto.ToList();

            //ViewBag.IDTelaio = myIDPerizia;
            return RedirectToAction("ScattaFoto", "Documenti", new { myIDPerizia= myIDPerizia, IDPerito= IDPerito, IDSpedizione = IDSpedizione,
                                                                     IDMeteo = IDMeteo, IDTP= IDTP, aIDTrasportatore = aIDTrasportatore ,
                                                                    aIDTipoRotabile = aIDTipoRotabile ,  aIDModelloCasa = aIDModelloCasa ,IsUpdate = IsUpdate
            });

            //return View("ScattaFoto", myIDPerizia);
        }


        public ActionResult ScattaPDF(string myIDPerizia, string IDPerito, string IDSpedizione, string IDMeteo, string IDTP, string aIDTrasportatore,
                                         string aIDTipoRotabile, string aIDModelloCasa, string ErrMess = "" ,bool IsUpdate = false)
        {

            var model = new Models.HomeModel();



            var myFoto = (from f in db.WEB_AUTO_PDF
                          where f.IDPerizia == myIDPerizia
                          select f);
            model.WEB_AUTO_PDF = myFoto.ToList();
            UpdateModel(myFoto);

            ViewBag.IDPerito = IDPerito;
            ViewBag.IDSpedizione = IDSpedizione;
            ViewBag.IDMeteo = IDMeteo;
            ViewBag.IDTP = IDTP;
            ViewBag.aIDTrasportatore = aIDTrasportatore;
            ViewBag.aIDTipoRotabile = aIDTipoRotabile;
            ViewBag.aIDModelloCasa = aIDModelloCasa;
            ViewBag.myIDPerizia = myIDPerizia;
            ViewBag.ErrMess = ErrMess;
            ViewBag.IsUpdate = IsUpdate;
            return View("ScattaPDF", myFoto);

            //ViewBag.myIDPerizia = myIDPerizia;
           // return View();

        }


        public ActionResult ScattaPDFSpedizione(string myIDSpedizione)
        {

            var model = new Models.HomeModel();



            var myFoto = (from f in db.WEB_AUTO_PDF
                          where f.IDPerizia == myIDSpedizione
                          select f);
            model.WEB_AUTO_PDF = myFoto.ToList();
            UpdateModel(myFoto);

            ViewBag.IDSpedizione = myIDSpedizione;
            
            return View("ScattaPDFSpedizione", myFoto);

        }


        public ActionResult UploadPDF(IEnumerable<HttpPostedFileBase> files, string myIDPerizia, string IDPerito, string IDSpedizione, string IDMeteo,
               string IDTP, string aIDTrasportatore, string aIDTipoRotabile, string aIDModelloCasa, string ErrMess = "", bool IsUpdate = false)
        {
            string filename = "";
            string path = "";
            string pathtrimmed = "";


            foreach (var file in files)
            {
                if (file != null)
                {
                    filename = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                    filename += "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".pdf";

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
                                    where m.IDPerizia == myIDPerizia
                                    select m.Prog).Max();
                    }
                    catch { }
                    cnt++;
                    filename = Regex.Replace(filename, @"\s", "");

                    //filename += DateTime.Now.ToString("yyyyMMdd_hhmmss");

                    string fullPath = Request.MapPath("~/DocumentiXTelai/PDF/" + filename);
                    if (System.IO.File.Exists(fullPath))
                    {
                        var sql = @"INSERT INTO WEB_AUTO_PDF (IDPerizia, FileName,Prog,Flags, IsSent ) Values (@IDPerizia, @FileName,@Prog,@Flags, 0 )";
                        int noOfRowInserted = db.Database.ExecuteSqlCommand(sql,
                            new SqlParameter("@IDPerizia", myIDPerizia),
                            new SqlParameter("@FileName", filename),
                            new SqlParameter("@Flags", 1),
                            new SqlParameter("@Prog", cnt));
                    }

                   
                }
            }

            var model = new Models.HomeModel();



            var myFoto = (from f in db.WEB_AUTO_PDF
                          where f.IDPerizia == myIDPerizia
                          select f);
            model.WEB_AUTO_PDF = myFoto.ToList();
            UpdateModel(myFoto);


            ViewBag.myIDPerizia = myIDPerizia;
            ViewBag.IDPerito = IDPerito;
            ViewBag.IDSpedizione = IDSpedizione;
            ViewBag.IDMeteo = IDMeteo;
            ViewBag.IDTP = IDTP;
            ViewBag.aIDTrasportatore = aIDTrasportatore;
            ViewBag.aIDTipoRotabile = aIDTipoRotabile;
            ViewBag.aIDModelloCasa = aIDModelloCasa;
            ViewBag.ErrMess = ErrMess;
            ViewBag.IsUpdate = IsUpdate;
            return View("ScattaPDF", myFoto);
        }

        public ActionResult UploadPDFSpedizione(IEnumerable<HttpPostedFileBase> files,  string IDSpedizione)
        {
            string filename = "";
            string path = "";
            string pathtrimmed = "";


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

                    string fullPath = Request.MapPath("~/DocumentiXTelai/PDF/" + filename);
                    if (System.IO.File.Exists(fullPath))
                    {
                        var sql = @"INSERT INTO WEB_AUTO_PDF (IDPerizia, FileName,Prog,Flags,IsSent) Values (@IDPerizia, @FileName,@Prog,@Flags,0)";
                        int noOfRowInserted = db.Database.ExecuteSqlCommand(sql,
                            new SqlParameter("@IDPerizia", IDSpedizione),
                            new SqlParameter("@FileName", filename),
                            new SqlParameter("@Flags", 4),
                            new SqlParameter("@Prog", cnt));
                    }

                    
                }
            }

            var model = new Models.HomeModel();



            var myFoto = (from f in db.WEB_AUTO_PDF
                          where f.IDPerizia == IDSpedizione
                          select f);
            model.WEB_AUTO_PDF = myFoto.ToList();
            UpdateModel(myFoto);


            
            ViewBag.IDSpedizione = IDSpedizione;
            
            return View("ScattaPDFSpedizione", myFoto);
        }
        public ActionResult CancellaPDF(int? IDDocumento, string myIDPerizia, string nomefile, string IDPerito, string IDSpedizione, string IDMeteo, string IDTP, 
                                        string aIDTrasportatore, string aIDTipoRotabile, string aIDModelloCasa ,  bool IsUpdate = false)
        {
            var sql = @"DELETE FROM WEB_AUTO_PDF WHERE ID = @IDDocumento";
            int myRecordCounter = db.Database.ExecuteSqlCommand(sql, new SqlParameter("@IDDocumento", IDDocumento));

            string fullPath = Request.MapPath("~/DocumentiXTelai/PDF/" + nomefile);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            var model = new Models.HomeModel();



            var myFoto = (from f in db.WEB_AUTO_PDF
                          where f.IDPerizia == myIDPerizia
                          select f);
            model.WEB_AUTO_PDF = myFoto.ToList();

            //ViewBag.IDTelaio = myIDPerizia;
            return RedirectToAction("ScattaPDF", "Documenti", new { myIDPerizia = myIDPerizia, IDPerito = IDPerito, IDSpedizione = IDSpedizione,
                                                                    IDMeteo = IDMeteo, IDTP = IDTP,  aIDTrasportatore = aIDTrasportatore,    aIDTipoRotabile = aIDTipoRotabile,
                                                                    aIDModelloCasa = aIDModelloCasa, IsUpdate = IsUpdate  });

            //return View("ScattaFoto", myIDPerizia);
        }

        public ActionResult CancellaPDFSpedizione(string IDSpedizione , string nomefile)
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
            return RedirectToAction("ScattaPDFSpedizione", "Documenti", new
            {
                
                IDSpedizione = IDSpedizione
                
            });

            //return View("ScattaFoto", myIDPerizia);
        }
    }
}