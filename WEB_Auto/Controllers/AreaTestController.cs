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
           

            string fileName = @"D:\TFS\MVC\Web_Auto\WEB_Auto\DocumentiXTelai\TXT\ZioTest.txt";
            string aFileName = "ZioTest.txt";
            
            FileInfo fi = new FileInfo(fileName);
            using (StreamWriter sw = fi.CreateText())
            {
                sw.WriteLine("New file created: {0}", DateTime.Now.ToString());
                sw.WriteLine("Author: Mahesh Chand");
                sw.WriteLine("Add one more line ");
                sw.WriteLine("Add one more line ");
                sw.WriteLine("Done! ");
            }

             aFileName = "ZioTWOTest.txt";
            string path = Server.MapPath("~/DocumentiXTelai/TXT");
            path = path + "\\" + aFileName;
            fi = new FileInfo(path);
            using (StreamWriter sw = fi.CreateText())
            {
                sw.WriteLine("New file created: {0}", DateTime.Now.ToString());
                sw.WriteLine("Author: Zietto!");
                sw.WriteLine("Add one more line ");
                sw.WriteLine("Add one more line ");
                sw.WriteLine("Done! ");
            }
        }
    }
}