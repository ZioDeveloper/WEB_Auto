using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_Auto.Models;
using ClosedXML.Excel;
using System.IO;

namespace WEB_Auto.Controllers
{
    public class ListaPerizieController : Controller
    {
        private wisedbEntities db = new wisedbEntities();

        // GET: ListaSpedizioni
        
        public ActionResult ListaSpedizioni(string Status = "APERTE")
        {
            Session["RTB"] = false; ;
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
            string aPerito = Session["IDPeritoVero"].ToString();

            if (Status == "TUTTE")
            {
                var lista = (from m in db.WEB_AUTO_ListaSpedizioni_2_vw
                             where m.IDPerito == aPerito
                             select m).ToList();
                model.WEB_AUTO_ListaSpedizioni_2_vw = lista;
               
            }
            else if (Status == "APERTE")
            {
                var lista = (from m in db.WEB_AUTO_ListaSpedizioni_2_vw
                             where m.IDPerito == aPerito
                             where m.IsClosed == false
                             select m).ToList();
                model.WEB_AUTO_ListaSpedizioni_2_vw = lista;
                
            }
            else if (Status == "CHIUSE")
            {
                var lista = (from m in db.WEB_AUTO_ListaSpedizioni_2_vw
                             where m.IDPerito == aPerito
                             where m.IsClosed == true
                             select m).ToList();
                model.WEB_AUTO_ListaSpedizioni_2_vw = lista;
                
            }

            return View(model);


            //using (var ctx = new wisedbEntities())
            //{
            //    //this will throw an exception
            //    var myForwardings = ctx.WEB_AUTO_ListaSpedizioni_vw.SqlQuery("SELECT  P.IDSpedizione, S.IDOriginale1 AS RifCliente,0 AS NumPerizie,0 AS Good, 0 As Damaged ,  P.IsClosed, P.IDPerito " +
            //" FROM            dbo.AGR_PERIZIE_TEMP_MVC AS P WITH(NOLOCK) INNER JOIN " +
            //                                                               " dbo.AGR_Spedizioni AS S ON P.IDSpedizione = S.ID LEFT OUTER JOIN " +
            //                                                               " dbo.AGR_PERIZIE_DETT_TEMP_MVC AS D WITH(NOLOCK) ON P.ID = D.IDPerizia " +
            //                                                               " --GROUP BY P.IDSpedizione, P.IsClosed, P.IDPerito, S.IDOriginale1").ToList();
            //return View(model.WEB_Auto_Test);
            //}

        }
        
        public ActionResult EditSpedizione(string IDSpedizione, string IDTP, string TipoMezzo = "TUTTE")
        {
            var model = new Models.HomeModel();
            string aPerito = Session["IDPeritoVero"].ToString();

            string myViaggio = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                            where m.IDSpedizione == IDSpedizione
                            select m.IDOriginale1).FirstOrDefault();

            if (TipoMezzo == "TUTTE")
            {
                var lista = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                             where m.IDSpedizione == IDSpedizione
                             where m.IDPerito == aPerito
                             where m.IDTipoPerizia == IDTP
                             select m).ToList();
                model.WEB_Auto_ListaPerizieXSpedizione_vw = lista;
            }
            else if (TipoMezzo == "RTB")
            {
                var lista = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                             where m.IDSpedizione == IDSpedizione
                             where m.IDPerito == aPerito
                             where m.IDTipoPerizia == IDTP
                             where m.IDModello.ToString() == "1240" || m.IDModello.ToString() == "1241"
                             select m).ToList();
                model.WEB_Auto_ListaPerizieXSpedizione_vw = lista;
            }
            if (TipoMezzo == "AUTO")
            {
                var lista = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                             where m.IDSpedizione == IDSpedizione
                             where m.IDPerito == aPerito
                             where m.IDTipoPerizia == IDTP
                             where m.IDModello.ToString() != "1240" && m.IDModello.ToString() != "1241"
                             select m).ToList();
                model.WEB_Auto_ListaPerizieXSpedizione_vw = lista;
            }
            
            
            ViewBag.IDSpedizione = IDSpedizione;
            ViewBag.IDTP = IDTP;
            ViewBag.myViaggio = myViaggio;
            return View(model);
        }

        public ActionResult ChiudiSpedizione(string IDSpedizione, string IDTP)
        {
            string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                            " SET ISClosed = 1  " +
                            " WHERE IDSpedizione = @IDSpedizione " +
                            " AND IDTipoPerizia = @IDTipoPerizia" ;


            int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDSpedizione", IDSpedizione),
                                                                 new SqlParameter("@IDTipoPerizia", IDTP));
            
            ViewBag.IDSpedizione = IDSpedizione;

            if(Inserted >0)
            {
                string aFile = @"C:\Test\";
                
                var datiFile = (from m in db.AGR_Spedizioni
                            where m.ID == IDSpedizione
                            select new { m.IDOriginale1 ,m.DataInizioImbarco}).FirstOrDefault();

                string aIDOrig = datiFile.IDOriginale1.Replace(@"\","-");
                aIDOrig = datiFile.IDOriginale1.Replace(@"/", "-");
                aFile += "GNV_" + aIDOrig + "_" + datiFile.DataInizioImbarco.Value.ToString("dd-MM-yyyy") + "_" + IDSpedizione + ".XLSX";
                Create(aFile, IDSpedizione);
            }
            return RedirectToAction("ListaSpedizioni");
        }

        public ActionResult CarouselFoto(string aIDPerizia)
        {
            var model = new Models.HomeModel();
            var foto = (from m in db.WEB_AUTO_FOTO
                        where m.IDPerizia == aIDPerizia
                        select m).ToList();
            model.WEB_AUTO_FOTO = foto;
            
            return View(model);
        }

        public ActionResult MostraPDF(string aIDPerizia)
        {
            var model = new Models.HomeModel();
            var pdf = (from m in db.WEB_AUTO_PDF
                        where m.IDPerizia == aIDPerizia
                        select m).ToList();
            model.WEB_AUTO_PDF = pdf;

            return View(model);
        }

        public void Create(string filePath,string IDSpedizione )
        {
            var model = new Models.HomeModel();

            string aViaggio = (from m in db.AGR_Spedizioni
                               where m.ID == IDSpedizione
                               select m.IDOriginale1).FirstOrDefault();

            int rtb = (from m in db.WEB_ListaPerizieFlat_MVC_vw
                       where m.IDSpedizione == IDSpedizione
                       where m.IDModelloCasa == "1240" || m.IDModelloCasa == "1241"
                       select m.IDPerizia).Distinct().Count();
            int rtbD = (from m in db.WEB_ListaPerizieFlat_MVC_vw
                        where m.IDSpedizione == IDSpedizione
                        where m.IDModelloCasa == "1240" || m.IDModelloCasa == "1241"
                        where m.Status == "DMG"
                        select m.IDPerizia).Distinct().Count();
            int auto = (from m in db.WEB_ListaPerizieFlat_MVC_vw
                        where m.IDSpedizione == IDSpedizione
                        where m.IDModelloCasa != "1240" &&  m.IDModelloCasa != "1241"
                        select m.IDPerizia).Distinct().Count();
            int autoD = (from m in db.WEB_ListaPerizieFlat_MVC_vw
                         where m.IDSpedizione == IDSpedizione
                         where m.IDModelloCasa != "1240" && m.IDModelloCasa != "1241"
                         where m.Status == "DMG"
                         select m.IDPerizia).Distinct().Count();

            var lista = (from m in db.WEB_ListaPerizieFlat_MVC_vw
                         where m.IDSpedizione == IDSpedizione
                         select m).OrderBy(s => s.Status).ToList();
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
            var rng5 = rngTable.Range("C3:C3");

            rng1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            rng1.Style.Font.Bold = true;
            rng2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            rng2.Style.Font.Bold = true;
            rng3.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            rng3.Style.Font.Bold = true;
            rng4.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rng4.Style.Font.Bold = true;
            rng5.Style.Font.Bold = true;
            //rngHeaders.Style.Fill.BackgroundColor = XLColor.LightGray;

            ws.Cell(1, 1).Value = "Astrea Claim Solutions S.r.l.";
            ws.Cell(3, 1).Value = "Spedizione :";
            ws.Cell(3, 2).Value = lista[0].IDSpedizione;
            ws.Cell(3, 3).Value = "Viaggio :";
            ws.Cell(3, 4).Value = lista[0].Viaggio;

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
            ws.Cell(7, 6).Value = rtb - rtbD;
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
    }
}