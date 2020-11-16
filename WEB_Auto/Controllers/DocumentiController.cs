using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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
                                         string aIDTipoRotabile, string aIDModelloCasa)
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
            //ViewBag.aIDTrasportatore = aIDTrasportatore;
            //ViewBag.aIDTipoRotabile = aIDTipoRotabile;
            //ViewBag.aIDModelloCasa = aIDModelloCasa;
            ViewBag.myIDPerizia = myIDPerizia;
            return View("ScattaFoto", myFoto);

            ViewBag.myIDPerizia = myIDPerizia;
            return View();

        }

        public ActionResult UploadFoto(IEnumerable<HttpPostedFileBase> files, string myIDPerizia)
        {
            string filename = "";
            string path = "";


            foreach (var file in files)
            {
                if (file != null)
                {
                    filename = System.IO.Path.GetFileName(file.FileName);

                    path = System.IO.Path.Combine(Server.MapPath("~/DocumentiXTelai/Foto"), filename);
                    if (file != null)
                    {
                        file.SaveAs(path);
                    }

                    int cnt = (from m in db.WEB_AUTO_FOTO
                               where m.IDPerizia == myIDPerizia
                               select m.ID).Count();

                    cnt++;

                    var sql = @"Insert Into WEB_AUTO_FOTO (IDPerizia, FileName,Prog) Values (@IDPerizia, @FileName,@Prog)";
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
            return View("ScattaFoto", myFoto);
        }

        public ActionResult CancellaDocumento(int? IDDocumento ,string myIDPerizia, string nomefile, string IDPerito, string IDSpedizione, string IDMeteo, string IDTP)
        {
            var sql = @"DELETE FROM WEB_AUTO_FOTO WHERE ID = @IDDocumento";
            int myRecordCounter = db.Database.ExecuteSqlCommand(sql, new SqlParameter("@IDDocumento", IDDocumento));

            string fullPath = Request.MapPath("~/DocumentiXTelai/" + nomefile);
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
            return RedirectToAction("ScattaFoto", "Documenti", new { myIDPerizia= myIDPerizia, IDPerito= IDPerito, IDSpedizione = IDSpedizione, IDMeteo = IDMeteo, IDTP= IDTP });

            //return View("ScattaFoto", myIDPerizia);
        }

    }
}