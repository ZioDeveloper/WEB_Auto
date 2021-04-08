using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using WEB_Auto.Models;
using System.Text;

namespace WEB_Auto.Controllers
{
    public class AreaTestController : Controller
    {
        private wisedbEntities db = new wisedbEntities();
        // GET: AreaTest
        public ActionResult Index()
        {
            CreateFile();
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

        public void CreateFile()
        {
            ////todo: add some data from your database into that string:
            //var string_with_your_data = "TEST !";

            //var byteArray = Encoding.ASCII.GetBytes(string_with_your_data);
            //var stream = new MemoryStream(byteArray);

            //return File(stream, "text/plain", "C:\\test\\your_file_name.txt");
            
            string fileName = @"ZioTest.txt";
            string path = System.IO.Path.Combine(Server.MapPath("~/DocumentiXTelai/TXT"), fileName);
            FileInfo fi = new FileInfo(fileName);
            using (StreamWriter sw = fi.CreateText())
            {
                sw.WriteLine("New file created: {0}", DateTime.Now.ToString());
                sw.WriteLine("Author: Mahesh Chand");
                sw.WriteLine("Add one more line ");
                sw.WriteLine("Add one more line ");
                sw.WriteLine("Done! ");
            }
        }
    }
}