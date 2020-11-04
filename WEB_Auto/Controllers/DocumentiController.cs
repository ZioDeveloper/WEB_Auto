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

        public ActionResult ScattaFoto(string myIDPerizia)
        {

            //var model = new Models.HomeModel();

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

                    path = System.IO.Path.Combine(Server.MapPath("~/DocumentiXTelai"), filename);
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

            //var model = new Models.HomeModel();

            //// Lista tipidocumento
            //var tipidoc = from m in db.TipiDocumento
            //              select m;
            //model.TipiDocumento = tipidoc.ToList();
            //var elencotipidocumento = new SelectList(model.TipiDocumento.ToList().OrderBy(m => m.ID), "ID", "TipoDocumento");
            //ViewData["TipiDocumento"] = elencotipidocumento;

            //var myFoto = (from f in db.FotoXTelaio_vw
            //              where f.IDTelaio == IDTelaio
            //              select f);
            //model.FotoXTelaio_vw = myFoto.ToList();
            //UpdateModel(myFoto);

            //ViewBag.IDTelaio = IDTelaio;
            //ViewBag.IDTipoDocumento = IDTipoDocumento;
            ViewBag.myIDPerizia = myIDPerizia;
            return View("ScattaFoto");
        }


    }
}