using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using WEB_Auto.Models;
using System.Text;
using ClosedXML.Excel;


namespace WEB_Auto.Controllers
{
    public class AreaTestController : Controller
    {
        private wisedbEntities db = new wisedbEntities();
        // GET: AreaTest
        public ActionResult Index()
        {
            //CreateFile();
            string aFile = @"C:\Test\MioExcel.xlsx";
            Create(aFile);
            var model = new Models.HomeModel();
            var lista = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                         where m.IDSpedizione == "G0327042"

                         select m).ToList();
            model.WEB_Auto_ListaPerizieXSpedizione_vw = lista;

            return View(model);
        }

        public void Create(String filePath)
        {
            var model = new Models.HomeModel();

            int rtb = (from m in db.WEB_ListaPerizieFlat_MVC_vw
                       where m.Viaggio == "54207"
                       where m.Modello == "1240-ROTABILI" 
                       select m.IDPerizia).Distinct().Count();
            int rtbD = (from m in db.WEB_ListaPerizieFlat_MVC_vw
                       where m.Viaggio == "54207"
                       where m.Modello == "1240-ROTABILI"
                       where m.Status == "DMG"
                       select m.IDPerizia).Distinct().Count();
            int auto = (from m in db.WEB_ListaPerizieFlat_MVC_vw
                       where m.Viaggio == "54207"
                       where m.Modello != "1240-ROTABILI" 
                       select m.IDPerizia).Distinct().Count();
            int autoD = (from m in db.WEB_ListaPerizieFlat_MVC_vw
                        where m.Viaggio == "54207"
                        where m.Modello != "1240-ROTABILI"
                        where m.Status == "DMG"
                        select m.IDPerizia).Distinct().Count();

            var lista = (from m in db.WEB_ListaPerizieFlat_MVC_vw
                          where m.Viaggio == "54207"
                         select m).OrderBy  (s => s.Status).ToList();
            model.WEB_ListaPerizieFlat_MVC_vw = lista;


            string path = Server.MapPath("~/DocumentiXTelai/TXT");
            IXLWorkbook wb = new XLWorkbook();
            IXLWorksheet ws = wb.Worksheets.Add("Sample Sheet");
            var rngTable = ws.Range("A1:M1000");
            rngTable.Style.Border.InsideBorder = XLBorderStyleValues.None;


            var rng1 = rngTable.Range("A1:A9"); // The address is relative to rngTable (NOT the worksheet)
            var rng2 = rngTable.Range("C7:C9");
            var rng3 = rngTable.Range("E7:E9");
            var rng4 = rngTable.Range("A11:M11");

            rng1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            rng1.Style.Font.Bold = true;
            rng2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            rng2.Style.Font.Bold = true;
            rng3.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            rng3.Style.Font.Bold = true;
            rng4.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rng4.Style.Font.Bold = true;
            //rngHeaders.Style.Fill.BackgroundColor = XLColor.LightGray;

            ws.Cell(1, 1).Value = "Astrea Claim Solutions S.r.l.";
            ws.Cell(3, 1).Value = "Spedizione :";
            ws.Cell(3, 2).Value = lista[0].IDSpedizione;
            
            ws.Cell(4, 1).Value = "Porto Imbarco :";
            ws.Cell(4, 2).Value = lista[0].POL;
            ws.Cell(5, 1).Value = "Porto Sbarco :";
            ws.Cell(5, 2).Value = lista[0].POD;
            
            ws.Cell(6, 1).Value = "Nave :";
            ws.Cell(6, 2).Value = lista[0].Nave;

            
            ws.Cell(7, 1).Value = "Rotabili :";
            ws.Cell(7, 2).Value = rtb;
            ws.Cell(7, 3).Value = "DAMAGED :";
            ws.Cell(7, 3).Style.Font.FontColor = XLColor.Red;
            ws.Cell(7, 4).Value = rtbD;
            ws.Cell(7, 4).Style.Font.FontColor = XLColor.Red;
            ws.Cell(7, 5).Value = "GOOD :";
            ws.Cell(7, 6).Value = rtb-rtbD;
            ws.Cell(7, 5).Style.Font.FontColor = XLColor.ForestGreen;
            ws.Cell(7, 6).Style.Font.FontColor = XLColor.ForestGreen;
            ws.Cell(8, 1).Value = "Veicoli :";
            ws.Cell(8, 2).Value = auto;
            ws.Cell(8, 3).Value = "DAMAGED :";
            ws.Cell(8, 3).Style.Font.FontColor = XLColor.Red;
            ws.Cell(8, 4).Value = autoD;
            ws.Cell(8, 4).Style.Font.FontColor = XLColor.Red;
            ws.Cell(8, 5).Value = "GOOD :";
            ws.Cell(8, 6).Value = auto - autoD;
            ws.Cell(8, 5).Style.Font.FontColor = XLColor.ForestGreen;
            ws.Cell(8, 6).Style.Font.FontColor = XLColor.ForestGreen;

            ws.Cell(9, 1).Value = "Totale perizie :";
            ws.Cell(9, 2).Value = rtb + auto;
            ws.Cell(9, 3).Value = "DAMAGED:";
            ws.Cell(9, 4).Value = rtbD + autoD;
            ws.Cell(9, 3).Style.Font.FontColor = XLColor.Red;
            ws.Cell(9, 4).Style.Font.FontColor = XLColor.Red;
            ws.Cell(9, 5).Value = "GOOD :";
            ws.Cell(9, 6).Value = (rtb + auto) - (rtbD + autoD);
            ws.Cell(9, 5).Style.Font.FontColor = XLColor.ForestGreen;
            ws.Cell(9, 6).Style.Font.FontColor = XLColor.ForestGreen;

            ws.Cell(11, 1).Value = "Telaio";
            ws.Cell(11, 2).Value = "Modello";
            ws.Cell(11, 3).Value = "Data perizia";
            ws.Cell(11, 4).Value = "Trasportatore";
            ws.Cell(11, 5).Value = "Status";
            ws.Cell(11, 6).Value = "Componente";
            ws.Cell(11, 7).Value = "Danno";



            int prog = 12;
            string myold = "";
            string mynew = "";
            foreach (var item in lista)
            {
                myold = item.Telaio;
                if (myold != mynew)
                {
                    ws.Cell(prog, 1).Value = item.Telaio;
                    ws.Cell(prog, 2).Value = item.Modello;
                    ws.Cell(prog, 3).Value = item.DataPerizia;
                    ws.Cell(prog, 3).Style.NumberFormat.Format = "mm/dd/yyyy";
                    ws.Cell(prog, 4).Value = item.Trasportatore;
                    ws.Cell(prog, 5).Value = item.Status;
                }
                
                ws.Cell(prog, 6).Value = item.Parte;
                ws.Cell(prog, 7).Value = item.Danno;
                mynew = myold;
                prog++;
            }

            ws.Columns("A", "H").AdjustToContents();
            wb.SaveAs(path + "/" + Path.GetFileName(filePath));
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