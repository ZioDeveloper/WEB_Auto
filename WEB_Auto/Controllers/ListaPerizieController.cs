using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_Auto.Models;
using ClosedXML.Excel;
using System.IO;
using System.Transactions;
using System.IO.Compression;
using System.Data.Entity;

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

            if (Session["Classe"].ToString() == "0")
            {
                if (Status == "TUTTE")
                {
                    var lista = (from m in db.WEB_AUTO_ListaSpedizioni_3_vw
                                 where m.IDPerito == aPerito
                                 select m).ToList();
                    model.WEB_AUTO_ListaSpedizioni_3_vw = lista;

                }
                else if (Status == "APERTE")
                {
                    var lista = (from m in db.WEB_AUTO_ListaSpedizioni_3_vw
                                 where m.IDPerito == aPerito
                                 where m.Aperte != 0
                                 where m.StandBy == 0
                                 select m).ToList();
                    model.WEB_AUTO_ListaSpedizioni_3_vw = lista;

                }
                else if (Status == "CHIUSE")
                {
                    var lista = (from m in db.WEB_AUTO_ListaSpedizioni_3_vw
                                 where m.IDPerito == aPerito
                                 where m.Aperte == 0
                                 select m).ToList();
                    model.WEB_AUTO_ListaSpedizioni_3_vw = lista;

                }

                else if (Status == "STANDBY")
                {
                    var lista = (from m in db.WEB_AUTO_ListaSpedizioni_3_vw
                                 where m.IDPerito == aPerito
                                 where m.Aperte != 0
                                 where m.StandBy != 0
                                 select m).ToList();
                    model.WEB_AUTO_ListaSpedizioni_3_vw = lista;

                }

                var stby = (from m in db.WEB_AUTO_ListaSpedizioni_3_vw
                            where m.IDPerito == aPerito
                            where m.StandBy > 0
                            select m).Count();


                ViewBag.Status = Status;
                if (stby > 0)
                    ViewBag.StabdBy = "Attenzione :  ci sono n° " + stby.ToString() + " perizie in standby !";
                else
                    ViewBag.StabdBy = "";

                return View(model);
            }
            else

            {
                if (Status == "TUTTE")
                {
                    var lista = (from m in db.WEB_AUTO_ListaSpedizioni_CMN_vw
                                 where m.IDPerito == aPerito
                                 where m.ID == "F2034275"
                                 select m).ToList();
                    model.WEB_AUTO_ListaSpedizioni_CMN_vw = lista;

                }
                else if (Status == "APERTE")
                {
                    var lista = (from m in db.WEB_AUTO_ListaSpedizioni_CMN_vw
                                 where m.IDPerito == aPerito
                                 where m.ID == "F2034275"
                                 where m.IsClosed == false
                                 select m).ToList();
                    model.WEB_AUTO_ListaSpedizioni_CMN_vw = lista;

                }
                else if (Status == "CHIUSE")
                {
                    var lista = (from m in db.WEB_AUTO_ListaSpedizioni_CMN_vw
                                 where m.IDPerito == aPerito
                                 where m.ID == "F2034275"
                                 where m.IsClosed == true
                                 select m).ToList();
                    model.WEB_AUTO_ListaSpedizioni_CMN_vw = lista;

                }


                ViewBag.Status = Status;
                return View(model);
            }

        }
        
        public ActionResult EditSpedizione(string IDSpedizione, string IDTP, string TipoMezzo = "TUTTE", string IDPerizia = "",
                                           string SDU_Viste = "TUTTE", string Status = "APERTE" )
        {
            var model = new Models.HomeModel();
            string aPerito = Session["IDPeritoVero"].ToString();

            string myViaggio = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                            where m.IDSpedizione == IDSpedizione
                            select m.IDOriginale1).FirstOrDefault();

            // START Ricalcolo numero foto...
            var listaFoto = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                         where m.IDSpedizione == IDSpedizione
                         where m.IDPerito == aPerito
                         where m.IDTipoPerizia == IDTP
                         select m).ToList();
            for (int i = 0; i < listaFoto.Count; i++)
            {
                string myIDPerizia = listaFoto[i].ID;
                string myCondition = listaFoto[i].Status;
                short myNumFoto = (short)listaFoto[i].NumFoto;
                if(myCondition == "Damaged" && myNumFoto == 0)
                    AggiornaContatoreFoto(myIDPerizia);

            }
            // END  Ricalcolo numero foto...


            if (TipoMezzo == "TUTTE" && SDU_Viste == "TUTTE")
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

            if (SDU_Viste == "DA VEDERE")
            {
                var lista = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                             where m.IDSpedizione == IDSpedizione
                             where m.IDPerito == aPerito
                             where m.IDTipoPerizia == IDTP
                             where m.IDOperatore == 12
                             select m).ToList();
                model.WEB_Auto_ListaPerizieXSpedizione_vw = lista;
            }

            if (SDU_Viste == "VISTE")
            {
                var lista = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                             where m.IDSpedizione == IDSpedizione
                             where m.IDPerito == aPerito
                             where m.IDTipoPerizia == IDTP
                             where m.IDOperatore != 12
                             where m.IDModello.ToString() != "1240" && m.IDModello.ToString() != "1241"
                             select m).ToList();
                model.WEB_Auto_ListaPerizieXSpedizione_vw = lista;
            }



            if (!String.IsNullOrEmpty(IDPerizia))
            {
                string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                            " SET IDOperatore = @IDOperatore  " +
                            " WHERE ID = @ID " ;


                int Updated = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDOperatore", (int)Session["IDOperatore"]),
                                                                     new SqlParameter("@ID", IDPerizia));
            }

            // Dati per dropdown Meteo
            var Meteo = from m in db.AGR_Meteo
                        where m.ID != "*"
                        where m.ID != "5"
                        where m.ID != "6"
                        where m.ID != "7"
                        select m;
            model.AGR_Meteo = Meteo.ToList();

            var stby = (from m in db.WEB_AUTO_ListaSpedizioni_3_vw
                        where m.IDPerito == aPerito
                        where m.ID == IDSpedizione
                        where m.StandBy > 0
                        select m).Count();



            if (stby > 0)
                ViewBag.StabdBy = "Attenzione :  ci sono n° " + stby.ToString() + " perizie in standby !";
            else
                ViewBag.StabdBy = "";

            var ElencoMeteo = new SelectList(model.AGR_Meteo.ToList(), "ID", "DescrITA");
            ViewData["ElencoMeteo"] = ElencoMeteo;

            ViewBag.IDSpedizione = IDSpedizione;
            ViewBag.IDTP = IDTP;
            ViewBag.myViaggio = myViaggio;
            ViewBag.TipoMezzo = TipoMezzo;
            ViewBag.Status = Status;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditSpedizione(FormCollection formCollection, string IDSpedizione, string IDTP, string IDPerito, string TipoMezzo = "TUTTE", string IDPerizia = "", string SDU_Viste = "TUTTE")
        {
            bool canDelete = true;
            try
            {
                

                if (canDelete)///idsStandby.Count. == 1)
                {
                    string[] ids = formCollection["ID"].Split(new char[] { ',' });
                    foreach (string id in ids)
                    {
                        var perizia = id;
                        string aPerizia = perizia.ToString();
                        if (aPerizia != "false")
                        {
                            EliminaPerizia(aPerizia, IDSpedizione, IDTP,IDPerito);
                        }

                    }
                }
            }
            catch { }

            //try
            //{
            //    string[] ids = formCollection["IDStandby"].Split(new char[] { ',' });
            //    foreach (string id in ids)
            //    {
            //        var perizia = id;
            //        string aPerizia = perizia.ToString();
            //        if (aPerizia != "false")
            //        {
            //            //SetStandbyPerizia(aPerizia);
            //        }

            //    }
            //}
            //catch { }


            var model = new Models.HomeModel();
            string aPerito = Session["IDPeritoVero"].ToString();

            string myViaggio = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                                where m.IDSpedizione == IDSpedizione
                                select m.IDOriginale1).FirstOrDefault();

            if (TipoMezzo == "TUTTE" && SDU_Viste == "TUTTE")
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

            if (SDU_Viste == "DA VEDERE")
            {
                var lista = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                             where m.IDSpedizione == IDSpedizione
                             where m.IDPerito == aPerito
                             where m.IDTipoPerizia == IDTP
                             where m.IDOperatore == 12
                             select m).ToList();
                model.WEB_Auto_ListaPerizieXSpedizione_vw = lista;
            }

            if (SDU_Viste == "VISTE")
            {
                var lista = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                             where m.IDSpedizione == IDSpedizione
                             where m.IDPerito == aPerito
                             where m.IDTipoPerizia == IDTP
                             where m.IDOperatore != 12
                             where m.IDModello.ToString() != "1240" && m.IDModello.ToString() != "1241"
                             select m).ToList();
                model.WEB_Auto_ListaPerizieXSpedizione_vw = lista;
            }



            if (!String.IsNullOrEmpty(IDPerizia))
            {
                string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                            " SET IDOperatore = @IDOperatore  " +
                            " WHERE ID = @ID ";


                int Updated = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDOperatore", (int)Session["IDOperatore"]),
                                                                     new SqlParameter("@ID", IDPerizia));
            }
            // Dati per dropdown Meteo
            var Meteo = from m in db.AGR_Meteo
                        where m.ID != "*"
                        select m;
            model.AGR_Meteo = Meteo.ToList();
            var ElencoMeteo = new SelectList(model.AGR_Meteo.ToList(), "ID", "DescrITA");
            ViewData["ElencoMeteo"] = ElencoMeteo;

            var stby = (from m in db.WEB_AUTO_ListaSpedizioni_3_vw
                        where m.IDPerito == aPerito
                        where m.ID == IDSpedizione
                        where m.StandBy > 0
                        select m).Count();


            
            if (stby > 0)
                ViewBag.StabdBy = "Attenzione :  ci sono n° " + stby.ToString() + " perizie in standby !";
            else
                ViewBag.StabdBy = "";



            ViewBag.IDSpedizione = IDSpedizione;
            ViewBag.IDTP = IDTP;
            ViewBag.myViaggio = myViaggio;
            ViewBag.TipoMezzo = TipoMezzo;
            return View(model);
        }

        

        public ActionResult ChiudiSpedizione(string IDSpedizione, string IDTP)
        {
            // Controllo non codificati
            string aPerito = Session["IDPeritoVero"].ToString();
            var cnt = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                         where m.IDSpedizione == IDSpedizione
                         where m.IDPerito == aPerito
                         where m.IDTipoPerizia == IDTP
                         where m.ID_TrasportatoreGrimaldi == "0"
                         select m).Count();
            
            if (cnt != 0)
                return View("CodificaNonInUso");


            // NON chiude le perizsie in standby
            string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                            " SET ISClosed = 1  " +
                            " WHERE IDSpedizione = @IDSpedizione " +
                            " AND IDPerito = @IDPerito " +
                            " AND Stato IS NULL OR Stato <> 'S' " +
                            " AND IDTipoPerizia = @IDTipoPerizia" ;


            int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDSpedizione", IDSpedizione),
                                                                 new SqlParameter("@IDPerito", aPerito),
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

        public ActionResult CarouselFoto(string aIDPerizia, string aTelaio, string CntrProvenienza = "" , int FiltroData = 0 , string FiltroStato = "")
        {
            var model = new Models.HomeModel();
            var foto = (from m in db.WEB_AUTO_FOTO
                        where m.IDPerizia == aIDPerizia
                        select m).ToList();
            model.WEB_AUTO_FOTO = foto;

            var myIDSpedizione = (from m in db.AGR_PERIZIE_TEMP_MVC
                        where m.ID == aIDPerizia
                        select new { m.IDSpedizione, m.IDTipoPerizia }).FirstOrDefault();
           
            ViewBag.IDSpedizione = myIDSpedizione.IDSpedizione;
            ViewBag.IDTP = myIDSpedizione.IDTipoPerizia;
            ViewBag.Telaio = aTelaio;
            ViewBag.CntrProvenienza = CntrProvenienza;
            ViewBag.FiltroData = FiltroData;
            ViewBag.FiltroStato = FiltroStato;
            return View(model);
        }

        public ActionResult CarouselFotoStoricizzate(string aIDPerizia, string aTelaio, string aViaggio, string CntrProvenienza = "")
        {
            var model = new Models.HomeModel();
            //var foto = (from m in db.WEB_AUTO_FOTO
            //            where m.IDPerizia == aIDPerizia
            //            select m).ToList();
            //model.WEB_AUTO_FOTO = foto;
            var foto = (from m in db.WEB_ListaPerizieFlat_DEF_vw
                        where m.IDPerizia == aIDPerizia
                        select m).ToList();
            model.WEB_ListaPerizieFlat_DEF_vw = foto;

            var myIDSpedizione = (from m in db.AGR_PERIZIE_TEMP_MVC
                                  where m.ID == aIDPerizia
                                  select new { m.IDSpedizione, m.IDTipoPerizia }).FirstOrDefault();

            //ViewBag.IDSpedizione = myIDSpedizione.IDSpedizione;
           // ViewBag.IDTP = myIDSpedizione.IDTipoPerizia;
            //ViewBag.Telaio = aTelaio;
            ViewBag.NumFoto = foto[0].NumFoto;
            ViewBag.IDPErizia = foto[0].IDPerizia;
            ViewBag.CntrProvenienza = CntrProvenienza;
            ViewBag.myViaggio = aViaggio;
            return View(model);
        }

        public ActionResult MostraPDF(string aIDPerizia, string IDSpedizione, string IDTP)
        {
            var model = new Models.HomeModel();
            var pdf = (from m in db.WEB_AUTO_PDF
                        where m.IDPerizia == aIDPerizia
                        select m).ToList();
            model.WEB_AUTO_PDF = pdf;
            ViewBag.IDSpedizione = IDSpedizione;
            ViewBag.IDTP = IDTP;

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
            ws.Cell(11, 6).Value = "N° foto";
            ws.Cell(11, 7).Value = "Componente";
            ws.Cell(11, 8).Value = "Danno";
            ws.Cell(11, 9).Value = "Note";


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
                ws.Cell(prog, 6).Value = item.NumFoto;
                ws.Cell(prog, 7).Value = item.Parte;
                ws.Cell(prog, 8).Value = item.Danno;
                ws.Cell(prog, 9).Value = item.Note;
                
                mynew = myold;
                prog++;
            }

            ws.Columns("A", "H").AdjustToContents();
            wb.SaveAs(path + "/" + Path.GetFileName(filePath));

            // Salvo dati file in tabella

           

            string aNomeFile = Path.GetFileName(filePath);
            string aContenuto = ""; // Deve essere blank
            string aIDGestione = "GN";
            string aPOL = "";
            if (lista[0].POL.Left(3) == "GOA")
            {
                aPOL = "GENOA";
            }
            if (lista[0].POL.Left(3) == "NAP")
            {
                aPOL = "NAPLES";
            }
            if (lista[0].POL.Left(3) == "PMO")
            {
                aPOL = "PALERMO";
            }
            if (lista[0].POL.Left(3) == "TRI")
            {
                aPOL = "TERMINI IMERESE";
            }
            if (lista[0].POL.Left(3) == "TUN")
            {
                aPOL = "TUNISI";
            }


            string aPOD = "";
            if (lista[0].POD.Left(3) == "GOA")
            {
                aPOD = "GENOA";
            }
            if (lista[0].POD.Left(3) == "NAP")
            {
                aPOD = "NAPLES";
            }
            if (lista[0].POD.Left(3) == "PMO")
            {
                aPOD = "PALERMO";
            }
            if (lista[0].POD.Left(3) == "TRI")
            {
                aPOD = "TERMINI IMERESE";
            }
            if (lista[0].POD.Left(3) == "TUN")
            {
                aPOD = "TUNISI";
            }



            string aIDSpedizione = lista[0].IDSpedizione.ToString();
            string aIDOriginale = lista[0].Viaggio.ToString();

            string sqlcmd = " INSERT INTO AGR_FilesTxt (NomeFile, Contenuto, IDGestione,  POL, POD, IDSpedizione, IDOriginale) " +
                           " VALUES(@NomeFile, @Contenuto, @IDGestione,  @POL, @POD, @IDSpedizione, @IDOriginale) ";

            int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@NomeFile", aNomeFile),
                                                               new SqlParameter("@Contenuto", aContenuto),
                                                               new SqlParameter("@IDGestione", aIDGestione),
                                                               new SqlParameter("@POL", aPOL),
                                                               new SqlParameter("@POD", aPOD),
                                                               new SqlParameter("@IDSpedizione", aIDSpedizione),
                                                               new SqlParameter("@IDOriginale", aIDOriginale));
        }

        public void EliminaPerizia(string IDPerizia, string IDSpedizione, string IDTP,string IDPErito, bool IsUpdate = false, string TipoMezzo = "TUTTE")
        {

            using ( wisedbEntities db = new wisedbEntities())
            {
                //using (DbContextTransaction transaction = db.Database.BeginTransaction())
                //{

                    try
                    {
                        string sqlcmd = " DELETE FROM  AGR_PerizieExpGrim_Temp_MVC  WHERE ID = @ID";
                        int deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@ID", IDPerizia));

                        sqlcmd = " DELETE FROM  AGR_PERIZIE_DETT_TEMP_MVC  WHERE IDPerizia = @IDPerizia";
                        deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", IDPerizia));

                        sqlcmd = " DELETE FROM  AGR_PERIZIE_TEMP_MVC WHERE ID = @ID";
                        deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@ID", IDPerizia));

                        string OS = Session["OS"].ToString();

                        sqlcmd = " INSERT INTO AGR_PERIZIE_TEMP_MVC_LOG (IDPerizia,InsertDate,IDPerito,IDOperatore , MachineName,TipoOperazione) " +
                                 " VALUES (@IDPerizia, @InsertDate, @IDPerito,@IDOperatore, @MachineName,@TipoOperazione)";

                        deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", IDPerizia),
                                                                             new SqlParameter("@InsertDate", DateTime.Now),
                                                                             new SqlParameter("@IDPerito", IDPErito),
                                                                             new SqlParameter("@IDOperatore", (int)Session["IDOperatore"]),
                                                                             new SqlParameter("@MachineName", OS),
                                                                             new SqlParameter("@TipoOperazione", "Delete"));

    

                        //transaction.Commit();
                    }
                    catch (SqlException exc)
                    {
                        string a = exc.Message;
                        ViewBag.Message = a;
                        RedirectToAction ("ErroreInCancellazioneMulti");

                    }
                //}
            }
            //else
            //    return RedirectToAction("EditSpedizione", "ListaPerizie", new { IDPerito, IDSpedizione, IDMeteo, IDTP, TipoMezzo });

        }

        public ActionResult ErroreInCancellazioneMulti(string aMsg)
        {
            ViewBag.Message = aMsg;
            return View();
        }


        public ActionResult SetStandbyPeriziaFromPerizia(string IDPerizia)
        {
            using (wisedbEntities db = new wisedbEntities())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {

                    try
                    {
                        string sqlcmd = " UPDATE  AGR_PERIZIE_TEMP_MVC SET Stato = NULL WHERE ID = @IDPerizia";
                        int deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", IDPerizia));



                        transaction.Commit();
                    }
                    catch (SqlException exc)
                    {
                        string a = exc.Message;
                        transaction.Rollback();
                    }
                }
            }

            return View();
        }


        public ActionResult SetStandbyPerizia(string IDPerizia, string IDPerito, string IDSpedizione, string IDMeteo, string IDTP, bool VengoDaListaPerito = false)
        {

            var myStatus = (from m in db.AGR_PERIZIE_TEMP_MVC
                        where m.ID == IDPerizia
                        where m.Stato == null
                        select m).Count();
            if (myStatus == 1)
            {
                using (wisedbEntities db = new wisedbEntities())
                {
                    using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    {

                        try
                        {
                            string sqlcmd = " UPDATE  AGR_PERIZIE_TEMP_MVC SET Stato = 'S' WHERE  ID = @IDPerizia ";
                            int deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", IDPerizia));



                            transaction.Commit();
                        }
                        catch (SqlException exc)
                        {
                            string a = exc.Message;
                            transaction.Rollback();
                        }
                    }
                }
            }
            else
            {
                using (wisedbEntities db = new wisedbEntities())
                {
                    using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    {

                        try
                        {
                            string sqlcmd = " UPDATE  AGR_PERIZIE_TEMP_MVC SET Stato = NULL WHERE ID = @IDPerizia";
                            int deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", IDPerizia));



                            transaction.Commit();
                        }
                        catch (SqlException exc)
                        {
                            string a = exc.Message;
                            transaction.Rollback();
                        }
                    }
                }
            }
            if (!VengoDaListaPerito)
            {
                return RedirectToAction("EditSpedizione", "ListaPerizie", new { IDPerito, IDSpedizione, IDMeteo, IDTP });
            }
            else
            {
                return RedirectToAction("ListaPerizieInStandByPErPerito", "ListaPerizie");
            }
        }

        public void AggiornaContatoreFoto(string aIDPerizia)
        {
            var myFoto = (from f in db.WEB_AUTO_FOTO
                          where f.IDPerizia == aIDPerizia
                          select f).Count();
            var myPDF = (from f in db.WEB_AUTO_PDF
                         where f.IDPerizia == aIDPerizia
                         select f).Count();

            string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                                " SET NumFoto = @NumFoto, " +
                                "     NumPDF = @NumPDF " +
                                " WHERE ID = @IDPerizia";


            int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@NumFoto", myFoto), new SqlParameter("@NumPDF", myPDF), new SqlParameter("@IDPerizia", aIDPerizia));
        }


        public ActionResult EsportaFotoSpedizione(string IDSpedizione)
        {


            System.IO.DirectoryInfo di = new DirectoryInfo(Path.Combine(Server.MapPath("~/ZIPPED").ToString()));

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            System.IO.DirectoryInfo di2 = new DirectoryInfo(Path.Combine(Server.MapPath("~/ZIPCreated").ToString()));

            foreach (FileInfo file in di2.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di2.GetDirectories())
            {
                dir.Delete(true);
            }

            var targa = (from f in db.WEB_AUTO_FOTO
                         join t in db.AGR_PERIZIE_TEMP_MVC on f.IDPerizia equals t.ID
                         where t.IDSpedizione == IDSpedizione
                         select new { t.Telaio }).ToList();

            string nomecartella = "";
            //string oldname = "";
            foreach (var item in targa)
            {
                if (!String.IsNullOrEmpty(item.Telaio))
                { nomecartella = item.Telaio.ToString(); }
                else
                { nomecartella = item.Telaio.ToString(); }

                bool exists = System.IO.Directory.Exists(Server.MapPath(@"~/ZIPPED/" + nomecartella).ToString());
                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(@"~/ZIPPED/" + nomecartella).ToString());
                    var foto = (from m in db.WEB_AUTO_FOTO
                                join t in db.AGR_PERIZIE_TEMP_MVC on m.IDPerizia equals t.ID
                                where t.IDSpedizione == IDSpedizione
                                where t.Telaio == nomecartella 
                                select m.FileName).ToList();
                    foreach (var itemfoto in foto)
                    {

                        string test = itemfoto.ToString();
                        string path2 = System.IO.Path.Combine(Server.MapPath("~/DocumentiXTelai/Foto"), itemfoto);
                        try
                        {
                            System.IO.File.Copy(path2, System.IO.Path.Combine(Server.MapPath("~/ZIPPED/" + nomecartella), itemfoto));
                        }
                        catch (Exception exc)
                        {
                            string a = exc.Message;
                        }
                    }
                }
                else
                {

                }


            }


            string nomecommessa = "TEST";
            string source = Path.Combine(Server.MapPath("~/ZIPPED").ToString());
            string dest = Path.Combine(Server.MapPath("~/ZIPCreated"), nomecommessa + ".zip");


            ZipFile.CreateFromDirectory(source, dest);



            ViewBag.NomeFile = nomecommessa + ".zip";



            return View();
        }

        public ActionResult ListaPerizieInStandBy()
        {
            var model = new Models.HomeModel();
            var myListStby = (from m in db.WEB_Auto_ListaPerizieXSpedizione_withInfo_vw
                              where m.Stato.ToUpper() == "S"
                                select m).ToList();
            model.WEB_Auto_ListaPerizieXSpedizione_withInfo_vw = myListStby;

            return View(model);
        }

        public ActionResult ListaPerizieInStandByPErPerito()
        {
            var model = new Models.HomeModel();
            string aPerito = Session["IDPeritoVero"].ToString();
            var myListStby = (from m in db.WEB_Auto_ListaPerizieXSpedizione_withInfo_vw
                              where m.Stato.ToUpper() == "S"
                              where m.IDPerito == aPerito
                              select m).ToList();
            model.WEB_Auto_ListaPerizieXSpedizione_withInfo_vw = myListStby;

            return View(model);
        }
    }
}