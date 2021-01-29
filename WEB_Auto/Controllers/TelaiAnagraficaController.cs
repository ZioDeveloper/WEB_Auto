using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Mvc;
using WEB_Auto.Models;

namespace WEB_Auto.Controllers
{
    public class TelaiAnagraficaController : Controller
    {
        private wisedbEntities db = new wisedbEntities();
        // GET: TelaiAnagrafica
        public ActionResult InputTelaio(string IDPerito, string IDSpedizione,string IDMeteo, string IDTP, string Chassis)
        {
            // Se sto cercando telaio...
            if(!String.IsNullOrEmpty(Chassis))
            {
                string myTelaio = Chassis.ToUpper();

                int cnt = (from m in db.AGR_PERIZIE_TEMP_MVC
                           where m.Telaio == Chassis
                           where m.IDSpedizione == IDSpedizione
                           select m.ID).Count();

                if (cnt == 0)
                {

                   string myIDPerizia = CreaNuovaPerizia(IDPerito, IDSpedizione, IDMeteo, IDTP, myTelaio);
                   return RedirectToAction("Edit", "TelaiAnagrafica", new { IDPerito, IDSpedizione, IDMeteo, IDTP, Chassis, myIDPerizia });
                    //return View("Edit");
                }
                else
                {

                    string ID_Trasportatore = "";
                    string ID_TipoRotabile = "";
                    string flagNU = "";
                    string Annotazioni = "";
                    var dati = (from m in db.AGR_Perizie_MVC_Flat_vw
                              where m.Telaio == myTelaio
                              where m.IDSpedizione == IDSpedizione
                              select new { m.ID_TrasportatoreGrimaldi, m.ID_TipoRotabile,m.IDModello,m.ID,m.FlgNuovoUsato,m.Note}).FirstOrDefault();
                    try
                    {
                        ID_Trasportatore = dati.ID_TrasportatoreGrimaldi.ToString();
                    }
                    catch { ID_Trasportatore = ""; }
                    try { ID_TipoRotabile = dati.ID_TipoRotabile.ToString(); }
                    catch { ID_TipoRotabile = ""; }
                    string myIDPerizia = dati.ID.ToString();
                    try {  flagNU = dati.FlgNuovoUsato.ToString(); } catch { flagNU = ""; }
                    try { Annotazioni = dati.Note.ToString(); } catch { Annotazioni = ""; }
                    string IDModello = dati.IDModello.ToString();
                    return RedirectToAction("Edit", "TelaiAnagrafica", new { IDPerito = IDPerito, IDSpedizione = IDSpedizione,
                        IDMeteo= IDMeteo, IDTP= IDTP, aIDTrasportatore = ID_Trasportatore,
                        aIDTipoRotabile = ID_TipoRotabile,
                        aIDModelloCasa = IDModello, myIDPerizia = myIDPerizia,
                        Annotazioni = Annotazioni
                    });

                }
            }

            // Se invece devo ancora inputare il telaio...
            ViewBag.IDPerito = IDPerito;
            ViewBag.IDSpedizione = IDSpedizione;
            ViewBag.IDMeteo = IDMeteo;
            ViewBag.IDTP = IDTP;
            return View();
        }

        public string CreaNuovaPerizia(string IDPerito, string IDSpedizione, string IDMeteo, string IDTP, string Chassis)
        {
            DateTime DataPerizia = DateTime.Now;
            string myIDPerizia = GetNewCode_AUTO(IDPerito);
            //string myDataPerizia = DataPerizia.Substring(6, 4) + DataPerizia.Substring(3, 2) + DataPerizia.Substring(0, 2);
            string sqlcmd = " INSERT INTO AGR_PERIZIE_Temp_MVC (ID, IDSpedizione, IDPerito, IDTipoPerizia, DataPerizia, IDNazione, Telaio, NumFoto, " +
                            "  Flags, IRichiesta, IDefinizione, IContab, DataModPerito, FlagControllo, IDMeteo, NumPDF,Note) " +
                            "VALUES (@ID, @IDSpedizione, @IDPerito, @IDTipoPerizia, @DataPerizia, @IDNazione, @Telaio, @NumFoto, " +
                            "  @Flags, @IRichiesta, @IDefinizione, @IContab, @DataModPerito, @FlagControllo, @IDMeteo, @NumPDF, @Note)";
            int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@ID", myIDPerizia),
                                                                 new SqlParameter("@IDSpedizione", IDSpedizione),
                                                                 new SqlParameter("@IDPerito", IDPerito),
                                                                 new SqlParameter("@IDTipoPerizia", IDTP),
                                                                 new SqlParameter("@DataPerizia", DataPerizia),
                                                                 new SqlParameter("@IDNazione", "***"),
                                                                 new SqlParameter("@Telaio", Chassis),
                                                                 new SqlParameter("@NumFoto", "0"),
                                                                 new SqlParameter("@Flags", 16),
                                                                 new SqlParameter("@IRichiesta", "0"),
                                                                 new SqlParameter("@IDefinizione", "0"),
                                                                 new SqlParameter("@IContab", "0"),
                                                                 new SqlParameter("@DataModPerito", DateTime.Now),
                                                                 new SqlParameter("@FlagControllo", "0"),
                                                                 new SqlParameter("@IDMeteo", IDMeteo),
                                                                 new SqlParameter("@NumPDF", "0"),
                                                                 new SqlParameter("@Note", ""));
            if (Inserted > 0)
            {


                sqlcmd = " INSERT INTO dbo.AGR_PerizieExpGrim_Temp_MVC  (ID ) " +
                          "  VALUES  (@ID)";
                Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@ID", myIDPerizia));
            }

            return myIDPerizia;

        }

        public string GetNewCode_AUTO(string aIDPerito)
        {
            string aIDPerizia = "";
            var myIDModemPerito = (from s in db.Periti
                                   where s.ID == aIDPerito
                                   select s.IDModem).FirstOrDefault();

            DateTime ora = DateTime.Now;
            string now = ora.ToString("yyyyMMddhhmmss");

            int Totale = 0;



            //For iw = 1 To 17
            //    Totale = Totale + Asc(Mid(wTemp, iw, 1))
            //Next


            aIDPerizia = myIDModemPerito + now + Totale;
            return aIDPerizia;
        }

        public ActionResult Edit(string IDPerito, string IDSpedizione, string IDMeteo, string IDTP, string aIDTrasportatore,
                                         string aIDTipoRotabile, string aIDModelloCasa, string myIDPerizia,string flagNU, string Annotazioni)
        {
            if (myIDPerizia == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AGR_PERIZIE_TEMP_MVC aGR_PERIZIE_TEMP_MVC = db.AGR_PERIZIE_TEMP_MVC.Find(myIDPerizia);
            if (aGR_PERIZIE_TEMP_MVC == null)
            {
                return HttpNotFound();
            }

            var dati = (from m in db.AGR_Perizie_MVC_Flat_vw
                          where m.ID == myIDPerizia
                          select m).FirstOrDefault();


            AggiornaFlagGoodDamaged(myIDPerizia);

            var model = new Models.HomeModel();
            // COME RECUPERARE CAMPI DA TABELLA/VISTA = select new{m. ecc ecc}

            var Casa = (from m in db.AGR_SpedizioniWEB_vw
                        where m.ID == dati.IDSpedizione
                        select new { m.IDCliente, m.IDCasa }).FirstOrDefault();

            string aIDCliente = Casa.IDCliente;
            string aCasa = Casa.IDCasa;
            // Dati spedizione
            var Spedizioni = from m in db.AGR_SpedizioniWEB_Decoded_vw
                             where m.ID == dati.IDSpedizione
                             select m;
            model.AGR_SpedizioniWEB_Decoded_vw = Spedizioni.ToList();

            // Dadti meteo
            var Meteo = from m in db.AGR_Meteo
                        where m.ID == dati.IDMeteo
                        select m;
            model.AGR_Meteo = Meteo.ToList();


            var TP = from m in db.AGR_TipiPerizia
                     where m.ID == dati.IDTipoPerizia
                     select m;
            model.AGR_TipiPerizia = TP.ToList();

            // Dati per dropdown Modello
            var modello = from m in db.AGR_ModelliAuto
                          where m.IDCliente == "**"
                          where m.IDCasa == aCasa
                          select m;
            model.AGR_ModelliAuto = modello.ToList();
            var ElencoModelli = new SelectList(model.AGR_ModelliAuto.ToList(), "ID", "Descr");
            ViewData["ElencoModelli"] = ElencoModelli;
            ViewBag.aIDModelloCasa = dati.IDModello;

            // Dati per dropdown Trasportatore Grimaldi
            var TraspGrim = from m in db.AGR_TrasportatoriGrimaldi
                            where m.Descr.ToString().Substring(0, 3) != "***"
                            select m;
            model.AGR_TrasportatoriGrimaldi = TraspGrim.ToList().OrderBy(m => m.Descr);
            var ElencoTraspGrim = new SelectList(model.AGR_TrasportatoriGrimaldi.ToList(), "ID", "Descr");
            ViewData["ElencoTraspGrim"] = ElencoTraspGrim;
            ViewBag.aIDTrasportatore = aIDTrasportatore;

            // Dati per dropdown Tipo rotabile
            var TipoRotabile = from m in db.AGR_TipoRotabile
                               select m;
            model.AGR_TipoRotabile = TipoRotabile.ToList();
            var ElencoTipoRotabile = new SelectList(model.AGR_TipoRotabile.ToList(), "ID", "DescrITA");


            ViewData["ElencoTipoRotabile"] = ElencoTipoRotabile;
            ViewBag.aIDTipoRotabile = aIDTipoRotabile;

            ViewBag.IDPerito = IDPerito;
            ViewBag.IDSpedizione = IDSpedizione;
            ViewBag.IDMeteo = IDMeteo;
            ViewBag.IDTP = IDTP;

            var perizie = from m in db.AGR_Perizie_MVC_Flat_vw
                            where m.ID == myIDPerizia
                          select m;
            model.AGR_Perizie_MVC_Flat_vw = perizie.ToList();

            return View(model);
        }

        public ActionResult SalvaPeriziaTesta(string IDPerito, string IDSpedizione, string IDMeteo, string IDTP, string Chassis, string DataPerizia, string IDModelloCasa, string IDTrasportatoreGrim,
                                              string IDTipoRotabile, bool? isDamaged, string Condizione, string Annotazioni, string myIDPerizia)
        {


            // Verifico Sia tutto ok.. to do !!!!!
            //bool isOK = CheckAll();

            // Aggiorno dati perizia
            string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                            " SET IDModello = @IDModello, Telaio = @Telaio, NumFoto = @NumFoto , FileNumber = 0 , Note = @Note " +
                            " WHERE ID = @IDPerizia";

        
            int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", myIDPerizia),
                                                                 new SqlParameter("@IDModello", IDModelloCasa),
                                                                 new SqlParameter("@Telaio", Chassis),
                                                                 new SqlParameter("@NumFoto", "0"),
                                                                 new SqlParameter("@Note", Annotazioni));
            if (Inserted > 0)
            {
                if (String.IsNullOrEmpty(Condizione))
                    Condizione = "";
                if (String.IsNullOrEmpty(IDTipoRotabile))
                    IDTipoRotabile = "";
                if (String.IsNullOrEmpty(IDTrasportatoreGrim))
                    IDTrasportatoreGrim = "";

                sqlcmd = " UPDATE AGR_PerizieExpGrim_Temp_MVC   " +
                          "  SET ID_TrasportatoreGrimaldi = @ID_TrasportatoreGrimaldi, " +
                          "  ID_TipoRotabile = @ID_TipoRotabile, " +
                          "   FlgNuovoUsato = @FlgNuovoUsato " +
                          " WHERE ID = @ID ";

                
                if (IDTrasportatoreGrim == "")
                    IDTrasportatoreGrim = null;

                if (IDTipoRotabile == "")
                    IDTipoRotabile = null;
                if (Condizione == "")
                    Condizione = null;

                Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@ID", myIDPerizia),
                                                                 new SqlParameter("@ID_TrasportatoreGrimaldi", (object)IDTrasportatoreGrim ?? DBNull.Value),
                                                                 new SqlParameter("@ID_TipoRotabile", (object)IDTipoRotabile ?? DBNull.Value),
                                                                 new SqlParameter("@FlgNuovoUsato", (object)Condizione ?? DBNull.Value));
            }


            if (isDamaged == false)
            {
                return RedirectToAction("Edit", "TelaiAnagrafica", new { IDPerito = IDPerito, IDSpedizione = IDSpedizione,
                    IDMeteo = IDMeteo, IDTP = IDTP, aIDTrasportatore = IDTrasportatoreGrim, aIDTipoRotabile = IDTipoRotabile,
                    aIDModelloCasa = IDModelloCasa,myIDPerizia = myIDPerizia });
            }
            else
            {
                return RedirectToAction("SalvaPeriziaDettagli", "TelaiAnagrafica", new { myIDPerizia });
            }
        }

        public ActionResult SalvaPeriziaDettagli(string myIDPerizia, string myIDParte)
        {
            var model = new Models.HomeModel();

            // Carichiamo UN PO' DI DATI...

            // Dati per dropdown AGR_Parti
            var parti = from m in db.WEB_AGR_Parti_vw
                        where m.IDCliente == "**"
                        where m.IDCasa == "RTB"
                        select m;
            model.WEB_AGR_Parti_vw = parti.ToList();
            var ElencoParti = new SelectList(model.WEB_AGR_Parti_vw.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
            ViewData["ElencoParti"] = ElencoParti;

            // Dati per dropdown AGR_Parti
            var danni = from m in db.WEB_AGR_Danni_vw
                        where m.IDCliente == "**"

                        select m;
            model.WEB_AGR_Danni_vw = danni.ToList();
            var ElencoDanni = new SelectList(model.WEB_AGR_Danni_vw.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
            ViewData["ElencoDanni"] = ElencoDanni;


            // Dati per dropdown AGR_Parti
            var gravita = from m in db.WEB_AGR_Gravita_vw
                          where m.IDCliente == "FI"

                          select m;
            model.WEB_AGR_Gravita_vw = gravita.ToList();
            var ElencoGravita = new SelectList(model.WEB_AGR_Gravita_vw.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
            ViewData["ElencoGravita"] = ElencoGravita;

            var Dettagli = from m in db.AGR_PERIZIE_DETT_TEMP_MVC_vw
                           where m.IDPerizia == myIDPerizia
                           select m;
            model.AGR_PERIZIE_DETT_TEMP_MVC_vw = Dettagli.ToList();
            ViewBag.IDPerizia = myIDPerizia;

            var test = (from m in db.AGR_PERIZIE_TEMP_MVC
                       where m.ID.ToString() == myIDPerizia
                       select new { m.IDPerito, m.IDSpedizione, m.IDMeteo, m.IDTipoPerizia, m.IDModello,m.ID }).FirstOrDefault();

            var test2 = (from m in db.AGR_PerizieExpGrim_Temp_MVC
                        where m.ID.ToString() == myIDPerizia
                        select new { m.ID_TipoRotabile,m.ID_TrasportatoreGrimaldi}).FirstOrDefault();

            ViewBag.IDPerito = test.IDPerito;
            ViewBag.IDSpedizione = test.IDSpedizione;
            ViewBag.IDMeteo = test.IDMeteo;
            ViewBag.IDTP = test.IDTipoPerizia;
            ViewBag.aIDTrasportatore = test2.ID_TrasportatoreGrimaldi;
            ViewBag.aIDTipoRotabile = test2.ID_TipoRotabile;
            ViewBag.aIDModelloCasa = test.IDModello;
            ViewBag.IDPeriz = test.ID;
            return View(model);
        }

        [HttpPost]
        public ActionResult SalvaPeriziaDettagli(string myIDPerizia, string IDParte, string IDDanno, string Qta, string Note, string Flags,
            string IDGravita, string IDResponsabilita, string IDAttribuzione)
        {

            // Inserisco dettagli danno perizia
            if (String.IsNullOrEmpty(Flags))
                Flags = "";
            if (String.IsNullOrEmpty(IDResponsabilita))
                IDResponsabilita = "";
            if (String.IsNullOrEmpty(Flags))
                Flags = "";
            if (String.IsNullOrEmpty(IDAttribuzione))
                IDAttribuzione = "";



            string sqlcmd = " INSERT INTO AGR_PERIZIE_DETT_TEMP_MVC (IDPerizia, IDParte, IDDanno, Qta, Note, Flags, IDGravita, IDResponsabilita, IDAttribuzione) " +
                           "VALUES (@IDPerizia, @IDParte, @IDDanno, @Qta, @Note, @Flags, @IDGravita, @IDResponsabilita, @IDAttribuzione)";
            int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", myIDPerizia),
                                                                 new SqlParameter("@IDParte", IDParte),
                                                                 new SqlParameter("@IDDanno", IDDanno),
                                                                 new SqlParameter("@Qta", Qta),
                                                                 new SqlParameter("@Note", Note),
                                                                 new SqlParameter("@Flags", 16),
                                                                 new SqlParameter("@IDGravita", IDGravita),
                                                                 new SqlParameter("@IDResponsabilita", IDResponsabilita),
                                                                 new SqlParameter("@IDAttribuzione", IDAttribuzione));

            // Aggiorno dati perizia
            sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                            " SET Flags = 32 " +
                            " WHERE ID = @IDPerizia";


            Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", myIDPerizia));

            //var model = new Models.HomeModel();
            //var Detatgli = from m in db.AGR_PERIZIE_DETT_TEMP_MVC_vw
            //               where m.IDPerizia == aIDPerizia
            //               select m;
            //model.AGR_PERIZIE_DETT_TEMP_MVC_vw = Detatgli.ToList();
            return RedirectToAction("SalvaPeriziaDettagli", "TelaiAnagrafica", new { myIDPerizia });
            
            //return View(model);
        }

        public ActionResult DeleteDettaglio(string aIDDett, string myIDPerizia)
        {
            string sqlcmd = " DELETE FROM  AGR_PERIZIE_DETT_TEMP_MVC WHERE ID = @ID";
            int deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@ID", aIDDett));

            return RedirectToAction("SalvaPeriziaDettagli", "TelaiAnagrafica", new { myIDPerizia });
        }

        public void AggiornaFlagGoodDamaged(string aIDPerizia)
        {
            var cnt = (from m in db.AGR_PERIZIE_DETT_TEMP_MVC_vw
                       select m.ID).Count();

            if (cnt > 0)
            {
                string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                                " SET Flags = 32 " +
                                " WHERE ID = @IDPerizia";


                int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", aIDPerizia));
            }
            else
            {
                string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                                " SET Flags = 16 " +
                                " WHERE ID = @IDPerizia";


                int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", aIDPerizia));
            }

        }
    }
}