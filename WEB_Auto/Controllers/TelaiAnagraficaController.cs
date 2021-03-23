using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Mvc;
using WEB_Auto.Models;
using System.Text.RegularExpressions;

namespace WEB_Auto.Controllers
{
    public class TelaiAnagraficaController : Controller
    {
        private wisedbEntities db = new wisedbEntities();
        // GET: TelaiAnagrafica
        public ActionResult InputTelaio(string IDPerito, string IDSpedizione,string IDMeteo, string IDTP, string Chassis, bool IsRTB = false)
        {
            if(!String.IsNullOrEmpty(IDPerito))
                EliminaTelaiSenzaModello(IDPerito);


            if (String.IsNullOrEmpty(IDSpedizione))
            {
                string usr = Session["User"].ToString();
                return RedirectToAction("Index", "Home", new { usr = usr, errMess = "Selezionare una spedizione" });
            }

            string Test = Session["RTB"].ToString();

            if (Session["RTB"].ToString().ToUpper() == "TRUE" && IsRTB)
                IsRTB = true;



            if (String.IsNullOrEmpty(Session["RTB"].ToString()))
                Session["RTB"] = IsRTB;
            else
                Session["RTB"] = IsRTB;

            var Casa = (from m in db.AGR_SpedizioniWEB_vw
                        where m.ID == IDSpedizione
                        select new { m.IDCliente, m.IDCasa }).FirstOrDefault();

            
            string aCasa = Casa.IDCasa;


            if (! String.IsNullOrEmpty(Chassis))
                Chassis = Regex.Replace(Chassis, @"\s+", "");


            if (String.IsNullOrEmpty(IDMeteo))
            {
                string usr = Session["User"].ToString();
                return RedirectToAction("Index", "Home", new { usr = usr, errMess = "Selezionare un tipo meteo" });
            }

            if (String.IsNullOrEmpty(IDTP))
            {
                string usr = Session["User"].ToString();
                return RedirectToAction("Index", "Home", new { usr = usr, errMess = "Selezionare un tipo perizia " });
            }
            // Se sto cercando telaio...
            if (!String.IsNullOrEmpty(Chassis))
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
                        IDMeteo = IDMeteo, IDTP = IDTP, aIDTrasportatore = ID_Trasportatore,
                        aIDTipoRotabile = ID_TipoRotabile,
                        aIDModelloCasa = IDModello, myIDPerizia = myIDPerizia,
                        Annotazioni = Annotazioni,
                        errMess = "In modifica"

                    });

                }
            }

            // Se invece devo ancora inputare il telaio...
            ViewBag.IDPerito = IDPerito;
            ViewBag.Casa = aCasa;
            ViewBag.IDSpedizione = IDSpedizione;
            ViewBag.IDMeteo = IDMeteo;
            ViewBag.IDTP = IDTP;
            return View();
        }


        public string CreaNuovaPerizia(string IDPerito, string IDSpedizione, string IDMeteo, string IDTP, string Chassis)
        {
            DateTime DataPerizia = DateTime.Now;
            string myIDPerizia = GetNewCode_AUTO(IDPerito,IDSpedizione);
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

        public string GetNewCode_AUTO(string aIDPerito, string aIDSpedizione)
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
                                         string aIDTipoRotabile, string aIDModelloCasa, string myIDPerizia,string flagNU, string Annotazioni, bool Filtrati = true ,
                                         string errMess = " ", bool IsUpdate = false) // errMess = " " per eludere primo controllo in View Edit
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
            AggiornaContatoreFoto(myIDPerizia);

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

            // Dati meteo
            var Meteo = from m in db.AGR_Meteo
                        where m.ID == dati.IDMeteo
                        select m;
            model.AGR_Meteo = Meteo.ToList();


            var TP = from m in db.AGR_TipiPerizia
                     where m.ID == dati.IDTipoPerizia
                     select m;
            model.AGR_TipiPerizia = TP.ToList();

            // Dati per dropdown Modello
            if (Session["RTB"].ToString().ToUpper() == "TRUE" && aCasa != "RTB")
            {
                var modello = from m in db.AGR_ModelliAuto
                              where m.IDCliente == "**"
                              where m.IDCasa == aCasa
                              where m.IDModelloCasa == "1240"
                              select m;
                model.AGR_ModelliAuto = modello.ToList().OrderBy(m => m.Descr);
                var ElencoModelli = new SelectList(model.AGR_ModelliAuto.ToList(), "ID", "Descr");
                ViewData["ElencoModelli"] = ElencoModelli;
                aIDModelloCasa = "1240";
                ViewBag.aIDModelloCasa = "1240";
            }
            if (Session["RTB"].ToString().ToUpper() == "TRUE" && aCasa == "RTB")
            {
                var modello = from m in db.AGR_ModelliAuto
                              where m.IDCliente == "**"
                              where m.IDCasa == aCasa
                              where m.IDModelloCasa == "1241"
                              select m;
                model.AGR_ModelliAuto = modello.ToList().OrderBy(m => m.Descr);
                var ElencoModelli = new SelectList(model.AGR_ModelliAuto.ToList(), "ID", "Descr");
                ViewData["ElencoModelli"] = ElencoModelli;
                aIDModelloCasa = "1241";
                ViewBag.aIDModelloCasa = "1240";
            }
            else
            {
                //if (aCasa == "CAB" && Filtrati )
                //{
                //    var modello = from m in db.AGR_ModelliAuto_CAB_vw
                //                  where m.IDCliente == "**"
                //                  where m.IDCasa == aCasa
                //                  where m.IDModelloCasa == "2055" || m.IDModelloCasa == "2006" || m.IDModelloCasa == "1922"
                //                  select m;
                //    model.AGR_ModelliAuto_CAB_vw = modello.ToList().OrderBy(m => m.Descr);
                //    var ElencoModelli = new SelectList(model.AGR_ModelliAuto_CAB_vw.ToList(), "ID", "Descr");
                //    ViewData["ElencoModelli"] = ElencoModelli;
                //}
                //else
                //{
                //    var modello = from m in db.AGR_ModelliAuto_CAB_vw
                //                  where m.IDCliente == "**"
                //                  where m.IDCasa == aCasa
                //                  select m;
                //    model.AGR_ModelliAuto_CAB_vw = modello.ToList().OrderBy(m => m.Descr);
                //    var ElencoModelli = new SelectList(model.AGR_ModelliAuto_CAB_vw.ToList(), "ID", "Descr");
                //    ViewData["ElencoModelli"] = ElencoModelli;
                //}
                if (aCasa == "CAB" && Filtrati && Session["RTB"].ToString().ToUpper() != "TRUE")
                {
                    var modello = from m in db.AGR_ModelliAuto
                                  where m.IDCliente == "**"
                                  where m.IDCasa == aCasa
                                  where m.IDModelloCasa == "2055" || m.IDModelloCasa == "2006" || m.IDModelloCasa == "1922" || m.IDModelloCasa == "1923"
                                                                  || m.IDModelloCasa == "1523" || m.IDModelloCasa == "1915" || m.IDModelloCasa == "1920"
                                                                  || m.IDModelloCasa == "1802" || m.IDModelloCasa == "1924" || m.IDModelloCasa == "1583"
                                                                  || m.IDModelloCasa == "1916" || m.IDModelloCasa == "1419" || m.IDModelloCasa == "1917"
                                                                  || m.IDModelloCasa == "1914" || m.IDModelloCasa == "2050" || m.IDModelloCasa == "1905"
                                                                  || m.IDModelloCasa == "2054" || m.IDModelloCasa == "1897" || m.IDModelloCasa == "1898"
                                                                  || m.IDModelloCasa == "1896" || m.IDModelloCasa == "1479" || m.IDModelloCasa == "2014"
                                                                  || m.IDModelloCasa == "2053" || m.IDModelloCasa == "219"
                                  select m;
                    model.AGR_ModelliAuto = modello.ToList().OrderBy(m => m.Descr);
                    var ElencoModelli = new SelectList(model.AGR_ModelliAuto.ToList(), "ID", "Descr");
                    ViewData["ElencoModelli"] = ElencoModelli;

                    //var modello = from m in db.AGR_ModelliAuto
                    //              where m.IDCliente == "**"
                    //              where m.IDCasa == aCasa
                    //              select m;
                    //model.AGR_ModelliAuto = modello.ToList().OrderBy(m => m.Descr);
                    //var ElencoModelli = new SelectList(model.AGR_ModelliAuto.ToList(), "ID", "Descr");
                    //ViewData["ElencoModelli"] = ElencoModelli;
                }
                else if (aCasa == "CAB" && Filtrati && Session["RTB"].ToString().ToUpper() == "TRUE")
                {
                    var modello = from m in db.AGR_ModelliAuto
                                  where m.IDCliente == "**"
                                  where m.IDCasa == aCasa
                                  where m.IDModelloCasa == "1240"
                                  select m;
                    model.AGR_ModelliAuto = modello.ToList().OrderBy(m => m.Descr);
                    var ElencoModelli = new SelectList(model.AGR_ModelliAuto.ToList(), "ID", "Descr");
                    ViewData["ElencoModelli"] = ElencoModelli;

                    //var modello = from m in db.AGR_ModelliAuto
                    //              where m.IDCliente == "**"
                    //              where m.IDCasa == aCasa
                    //              select m;
                    //model.AGR_ModelliAuto = modello.ToList().OrderBy(m => m.Descr);
                    //var ElencoModelli = new SelectList(model.AGR_ModelliAuto.ToList(), "ID", "Descr");
                    //ViewData["ElencoModelli"] = ElencoModelli;
                }
                else
                {
                    var modello = from m in db.AGR_ModelliAuto
                                  where m.IDCliente == "**"
                                  where m.IDCasa == aCasa
                                  select m;
                    model.AGR_ModelliAuto = modello.ToList().OrderBy(m => m.Descr);
                    var ElencoModelli = new SelectList(model.AGR_ModelliAuto.ToList(), "ID", "Descr");
                    ViewData["ElencoModelli"] = ElencoModelli;
                }

            }
            if (!String.IsNullOrEmpty(dati.IDModello.ToString()))
            {
                ViewBag.aIDModelloCasa = aIDModelloCasa; //dati.IDModello;
            }
            else
            {
                ViewBag.aIDModelloCasa = aIDModelloCasa;
            }
            if(aCasa == "RTB")
            {
                aIDModelloCasa = "1241";
                ViewBag.aIDModelloCasa = "1241";
            }

            // Dati per dropdown Trasportatore Grimaldi
            var TraspGrim = from m in db.AGR_TrasportatoriGrimaldi_vw
                            where m.Descr.ToString().Substring(0, 3) != "***"
                            select m;
            model.AGR_TrasportatoriGrimaldi_vw = TraspGrim.ToList().OrderBy(m => m.Descr);
            var ElencoTraspGrim = new SelectList(model.AGR_TrasportatoriGrimaldi_vw.ToList(), "ID", "Descr");
            ViewData["ElencoTraspGrim"] = ElencoTraspGrim;
            ViewBag.aIDTrasportatore = aIDTrasportatore;

            // Dati per dropdown Tipo rotabile
            var TipoRotabile = from m in db.AGR_TipoRotabile
                               select m;
            model.AGR_TipoRotabile = TipoRotabile.ToList();
            var ElencoTipoRotabile = new SelectList(model.AGR_TipoRotabile.ToList(), "ID", "DescrITA");


            ViewData["ElencoTipoRotabile"] = ElencoTipoRotabile;
            ViewBag.aIDTipoRotabile = aIDTipoRotabile;


            // Cerco Trasportatore Grimaldi e Tipo rotabile pregressi e li uso...
            if((aIDModelloCasa == "1240" || aIDModelloCasa == "1241" )&& (String.IsNullOrEmpty(aIDTrasportatore)&& String.IsNullOrEmpty(aIDTipoRotabile)))
            {
                var myTelaio = (from m in db.AGR_Perizie_MVC_Flat_vw
                            where m.ID == myIDPerizia
                            select m.Telaio).FirstOrDefault();
                var datiPregressi = (from m in db.AGR_DatiRotabiliInUSo_vw
                                     where m.Telaio == myTelaio
                                     orderby m.DataPerizia descending
                                     select new { m.ID_TipoRotabile, m.ID_TrasportatoreGrimaldi }).FirstOrDefault();
                try
                {
                    string myIDTipoRotabile = datiPregressi.ID_TipoRotabile;
                    string myIDTrasportatore = datiPregressi.ID_TrasportatoreGrimaldi;
                    ViewBag.aIDTipoRotabile = myIDTipoRotabile;
                    ViewBag.aIDTrasportatore = myIDTrasportatore;
                }
                catch { }// bruciamo eccezione

            }

            // Dati per dropdown Spedizione
            DateTime ini = DateTime.Today.AddDays(-30);
            DateTime end = DateTime.Today.AddDays(+30);
            var Spedizione = from m in db.AGR_SpedizioniWEB_vw
                             where m.DataInizioImbarco >= ini
                             where m.DataInizioImbarco <= end
                             select m;
            model.AGR_SpedizioniWEB_vw = Spedizione.ToList();
            var ElencoSpedizioni = new SelectList(model.AGR_SpedizioniWEB_vw.ToList(), "ID", "DescrMin");


            ViewData["ElencoSpedizioni"] = ElencoSpedizioni;
            ViewBag.IDSpedizione = IDSpedizione;

            ViewBag.IDPerito = IDPerito;
            
            ViewBag.IDMeteo = IDMeteo;
            ViewBag.IDTP = IDTP;

            var hasdanni = (from m in db.AGR_PERIZIE_DETT_TEMP_MVC_vw
                            where m.IDPerizia == myIDPerizia
                            select m).Count();
            if (hasdanni > 0)
                ViewBag.HasDanni = "Perizia con danni";
            else
                ViewBag.HasDanni = "Perizia GOOD";

            var perizie = from m in db.AGR_Perizie_MVC_Flat_vw
                            where m.ID == myIDPerizia
                          select m;
            model.AGR_Perizie_MVC_Flat_vw = perizie.ToList();
            ViewBag.ErrMess = errMess;

            // num foto
            //var NumFoto = (from m in db.WEB_AUTO_FOTO
            //                 where m.IDPerizia == myIDPerizia
            //                 select m.ID).Count();
            ViewBag.NumFoto = ContaFoto(myIDPerizia);
            ViewBag.NumPDF = ContaPDF(myIDPerizia);
            ViewBag.IsUpdate = IsUpdate;

            return View(model);
        }

        public ActionResult SalvaPeriziaTesta(string IDPerito, string IDSpedizione, string IDMeteo, string IDTP, string Chassis, string DataPerizia, string IDModelloCasa, string IDTrasportatoreGrim,
                                              string IDTipoRotabile, bool? isDamaged, string Condizione, string Annotazioni, string myIDPerizia, 
                                              bool IsUpdate = false,bool Filtrati=true, bool isRapid = false )
        {
            if (isRapid == true)
                isDamaged = false;

            if (isDamaged == true)
                isRapid = false;

            // Verifico Sia tutto ok.. to do !!!!!
            bool isOK = CheckAll(IDSpedizione, Chassis, IDModelloCasa, IDTrasportatoreGrim , IDTipoRotabile, Condizione , Annotazioni, out string myerrMess);
            if (isOK)
            {


                // Aggiorno dati perizia
                string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                                " SET IDSpedizione = @IDSpedizione , IDModello = @IDModello, Telaio = @Telaio, NumFoto = @NumFoto , FileNumber = 0 , Note = @Note " +
                                " WHERE ID = @IDPerizia";


                int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", myIDPerizia),
                                                                     new SqlParameter("@IDSpedizione", IDSpedizione),
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

                    if(Condizione == "U")
                    {

                        var hasdanni = (from m in db.AGR_PERIZIE_DETT_TEMP_MVC_vw
                                        where m.IDPerizia == myIDPerizia
                                        where m.IDParte == "045"
                                        where m.IDDanno == "Y"
                                        select m).Count();
                        if (hasdanni == 0)
                        {

                            sqlcmd = " INSERT INTO AGR_PERIZIE_DETT_TEMP_MVC (IDPerizia,IDParte, IDDanno, QTA, Note)" +
                                     " VALUES(@IDPerizia,@IDParte, @IDDanno, @QTA, @Note)";
                            try
                            {
                                Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", myIDPerizia),
                                                                             new SqlParameter("@IDParte", "045"),
                                                                             new SqlParameter("@IDDanno", "Y"),
                                                                             new SqlParameter("@QTA", 1),
                                                                             new SqlParameter("@Note", "Danni da utilizzo")
                                                                             );
                            }
                            catch (Exception exc)
                            {
                                string a = exc.Message;
                            }
                        }
                        
                    }
                }


                if (isDamaged == false && isRapid == false )
                {
                    return RedirectToAction("Edit", "TelaiAnagrafica", new
                    {
                        IDPerito = IDPerito,
                        IDSpedizione = IDSpedizione,
                        IDMeteo = IDMeteo,
                        IDTP = IDTP,
                        aIDTrasportatore = IDTrasportatoreGrim,
                        aIDTipoRotabile = IDTipoRotabile,
                        aIDModelloCasa = IDModelloCasa,
                        myIDPerizia = myIDPerizia,
                        errMess = "Sbloccato",
                        IsUpdate = IsUpdate,
                        Filtrati = Filtrati
                    });
                }
                else
                {
                    if (isDamaged == true && isRapid == false)
                        return RedirectToAction("SalvaPeriziaDettagli", "TelaiAnagrafica", new { myIDPerizia, IsUpdate= IsUpdate });
                    else if (isDamaged == false && isRapid == true)
                        return RedirectToAction("SalvaPeriziaDettagliRapid", "TelaiAnagrafica", new { myIDPerizia, IsUpdate = IsUpdate });
                    else // finire: non deve mai accadere
                        return RedirectToAction("SalvaPeriziaDettagli", "TelaiAnagrafica", new { myIDPerizia, IsUpdate = IsUpdate });
                }
            }
            else
            {
                return RedirectToAction("Edit", "TelaiAnagrafica", new
                {
                    IDPerito = IDPerito,
                    IDSpedizione = IDSpedizione,
                    IDMeteo = IDMeteo,
                    IDTP = IDTP,
                    aIDTrasportatore = IDTrasportatoreGrim,
                    aIDTipoRotabile = IDTipoRotabile,
                    aIDModelloCasa = IDModelloCasa,
                    myIDPerizia = myIDPerizia,
                    errMess = myerrMess,
                    IsUpdate = IsUpdate,
                    Filtrati = Filtrati
                });
            }
        }

        public ActionResult SalvaPeriziaDettagli(string myIDPerizia, string myIDParte, bool IsUpdate= false)
        {
            var model = new Models.HomeModel();
            bool ISGravitaEnabled = true;

            // Carichiamo UN PO' DI DATI...
            // *******************************
            bool ISGEFCO_GN_51 = pISGEFCO_GN_51(myIDPerizia);

            // Dati per dropdown AGR_Parti
            if (ISGEFCO_GN_51)
            {
                var parti = from m in db.WEB_AGR_Parti_vw
                            where m.IDCliente == "**"
                            where m.IDCasa == "RTB"
                            select m;
                model.WEB_AGR_Parti_vw = parti.ToList();
                var ElencoParti = new SelectList(model.WEB_AGR_Parti_vw.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
                ViewData["ElencoParti"] = ElencoParti;
            }
            else
            {
                var parti = from m in db.WEB_AGR_Parti_vw
                            where m.IDCliente == "**"
                            where m.IDCasa != "RTB"
                            select m;
                model.WEB_AGR_Parti_vw = parti.ToList();
                var ElencoParti = new SelectList(model.WEB_AGR_Parti_vw.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
                ViewData["ElencoParti"] = ElencoParti;
            }

           
            

            // Dati per dropdown AGR_Danni
            var danni = from m in db.WEB_AGR_Danni_vw
                        where m.IDCliente == "**"

                        select m;
            model.WEB_AGR_Danni_vw = danni.ToList();
            var ElencoDanni = new SelectList(model.WEB_AGR_Danni_vw.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
            ViewData["ElencoDanni"] = ElencoDanni;


            // Dati per dropdown AGR_Gravita
            // *******************************
            if (ISGEFCO_GN_51)
            {
                var gravita = from m in db.WEB_AGR_Gravita_vw
                              where m.IDCliente == "FI"

                              select m;
                model.WEB_AGR_Gravita_vw = gravita.ToList();
                var ElencoGravita = new SelectList(model.WEB_AGR_Gravita_vw.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
                ViewData["ElencoGravita"] = ElencoGravita;
            }
            else
            {
                var gravita = from m in db.WEB_AGR_Gravita_vw
                              where m.IDCliente == "NULL"

                              select m;
                model.WEB_AGR_Gravita_vw = gravita.ToList();
                var ElencoGravita = new SelectList(model.WEB_AGR_Gravita_vw.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
                ViewData["ElencoGravita"] = ElencoGravita;
                ISGravitaEnabled = false;

            }

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
            ViewBag.ISGravitaEnabled = ISGravitaEnabled;
            ViewBag.IsUpdate = IsUpdate;
            return View(model);
        }

        [HttpPost]
        public ActionResult SalvaPeriziaDettagli(string myIDPerizia, string IDParte, string IDDanno, string Qta, string Note, string Flags,
            string IDGravita, string IDResponsabilita, string IDAttribuzione , bool IsUpdate = false)
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
            if (String.IsNullOrEmpty(IDGravita))
                IDGravita = "";

            if (!String.IsNullOrEmpty(myIDPerizia) && !String.IsNullOrEmpty(IDParte) && !String.IsNullOrEmpty(IDDanno) && !String.IsNullOrEmpty(myIDPerizia))
            {

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
            }

            //var model = new Models.HomeModel();
            //var Detatgli = from m in db.AGR_PERIZIE_DETT_TEMP_MVC_vw
            //               where m.IDPerizia == aIDPerizia
            //               select m;
            //model.AGR_PERIZIE_DETT_TEMP_MVC_vw = Detatgli.ToList();
            return RedirectToAction("SalvaPeriziaDettagli", "TelaiAnagrafica", new { myIDPerizia , IsUpdate = IsUpdate });
            
            //return View(model);
        }

        public ActionResult DeleteDettaglio(string aIDDett, string myIDPerizia, bool IsUpdate = false)
        {
            string sqlcmd = " DELETE FROM  AGR_PERIZIE_DETT_TEMP_MVC WHERE ID = @ID";
            int deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@ID", aIDDett));

            return RedirectToAction("SalvaPeriziaDettagli", "TelaiAnagrafica", new { myIDPerizia , IsUpdate = IsUpdate });
        }

        public ActionResult DeleteDettaglioRapid(string aIDDett, string myIDPerizia, bool IsUpdate = false)
        {
            string sqlcmd = " DELETE FROM  AGR_PERIZIE_DETT_TEMP_MVC WHERE ID = @ID";
            int deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@ID", aIDDett));

            return RedirectToAction("SalvaPeriziaDettagliRapid", "TelaiAnagrafica", new { myIDPerizia, IsUpdate = IsUpdate });
        }

        public ActionResult EliminaPerizia(string IDPerizia , string IDPerito, string IDSpedizione, string IDMeteo, string IDTP, bool IsUpdate = false)
        {


            string sqlcmd =  " DELETE FROM  AGR_PerizieExpGrim_Temp_MVC  WHERE ID = @ID";
            int deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@ID", IDPerizia));

            sqlcmd = " DELETE FROM  AGR_PERIZIE_DETT_TEMP_MVC  WHERE IDPerizia = @IDPerizia";
            deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", IDPerizia));

            sqlcmd = " DELETE FROM  AGR_PERIZIE_TEMP_MVC WHERE ID = @ID";
            deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@ID", IDPerizia));


            return RedirectToAction("InputTelaio", "TelaiAnagrafica" ,new { IDPerito, IDSpedizione, IDMeteo, IDTP });
        }

        public void AggiornaFlagGoodDamaged(string aIDPerizia)
        {
            var cnt = (from m in db.AGR_PERIZIE_DETT_TEMP_MVC_vw
                       where m.IDPerizia == aIDPerizia
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

        public void AggiornaContatoreFoto(string aIDPerizia)
        {
            var myFoto = (from f in db.WEB_AUTO_FOTO
                          where f.IDPerizia == aIDPerizia
                          select f).Count();
            string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                                " SET NumFoto = @NumFoto " +
                                " WHERE ID = @IDPerizia";


            int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@NumFoto", myFoto), new SqlParameter("@IDPerizia", aIDPerizia));
        }

        public bool pISGEFCO_GN_51(string aIDPerizia)
        {
            var model = new Models.HomeModel();

            var perizia = (from m in db.AGR_PERIZIE_TEMP_MVC
                        where m.ID == aIDPerizia
                        select new { m.IDSpedizione,m.IDModello }).FirstOrDefault();
            string myIDSpedizione = perizia.IDSpedizione;
            string myIDModello = perizia.IDModello.ToString();

            var spediz = (from m in db.AGR_SpedizioniWEB_Decoded_vw
                          where m.ID == myIDSpedizione
                          select new { m.IDCasa}).FirstOrDefault();
            string myIDCasa = spediz.IDCasa;

            if((myIDCasa == "CAB" && myIDModello == "1240") || (myIDCasa == "RTB" && myIDModello == "1241"))
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public int ContaFoto(string aIdPerizia)
        {
            int cnt = 0;
            cnt = (from m in db.WEB_AUTO_FOTO
                           where m.IDPerizia == aIdPerizia
                           select m.ID).Count();
            return cnt;
        }

        public int ContaPDF(string aIdPerizia)
        {
            int cnt = 0;
            cnt = (from m in db.WEB_AUTO_PDF
                           where m.IDPerizia == aIdPerizia
                           select m.ID).Count();
            return cnt;
        }

        public bool CheckAll(string aIDSpedizione ,string aTelaio, string IDModelloCasa, string IDTrasportatoreGrim,  string IDTipoRotabile , string Condizione,string Annotazioni, out string errMEss)
        {
            // Dati spedizione
            errMEss = "";

            // Controllo modello
            if(String.IsNullOrEmpty(IDModelloCasa))
            {
                errMEss = "Modello obbligatorio"; return false;
            }
        
            // Usato NUovo
            if(!String.IsNullOrEmpty(IDModelloCasa))
            {
                if(IDModelloCasa != "1240" && IDModelloCasa !="1241" && String.IsNullOrEmpty(Condizione))
                { errMEss = "Nuovo / Usato info obbligatoria"; return false; }

            }
            
            // Trasportatore
            if((IDModelloCasa == "1240" || IDModelloCasa == "1241") && String.IsNullOrEmpty(IDTrasportatoreGrim))
            {
                errMEss += "Trasportatore obbligatorio " ;
                return false;
            }

            // Tipo rotabile
            if ((IDModelloCasa == "1240" || IDModelloCasa == "1241") && String.IsNullOrEmpty(IDTipoRotabile))
            {
                errMEss += "Tipo rotabile obbligatorio ";
                return false;
            }

            // Lunghezza telaio x rotabili
            //if(IDModelloCasa == "1240" || IDModelloCasa =="1241")
            //{
            //    if(aTelaio.Length != 7)
            //    {
            //        errMEss += "Lunghezza telaio errata, (7 caratteri per i rotabili) ";
            //        return false;
            //    }
            //}
            //else
            //{
            //    if (aTelaio.Length != 8)
            //    {
            //        errMEss += "Lunghezza telaio errata, (8 caratteri per i NON rotabili) ";
            //        return false;
            //    }
            //}

            if(IDTrasportatoreGrim == "0" && String.IsNullOrEmpty(Annotazioni))
            {
                errMEss += "Note obbligatorie ";
                return false;
            }

            if (IDModelloCasa == "219" && String.IsNullOrEmpty(Annotazioni))
            {
                errMEss += "Note obbligatorie ";
                return false;
            }

            if (!String.IsNullOrEmpty(errMEss))
            {
                return false;
            }

            errMEss = "";
            return true;
        }

        public bool CheckAllDetails(string IDParte, string IDDanno , out string errMEss)
        {
            if (String.IsNullOrEmpty(IDParte))
            {
                errMEss = "Inserire codice parte...";
                return false;
            }
            if (String.IsNullOrEmpty(IDDanno))
            {
                errMEss = "Inserire codice danno...";
                return false;
            }

            else
            {
                errMEss = "";
                return true;
            }
        }

        public void EliminaTelaiSenzaModello(string IDPerito)
        {
            string sqlcmd = " DELETE FROM AGR_PerizieExpGrim_Temp_MVC WHERE ID IN(SELECT ID FROM AGR_PERIZIE_TEMP_MVC WHERE IDPErito = @IDPErito  AND IDModello IS NULL)";
            int deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPErito", IDPerito));
            sqlcmd = " DELETE FROM AGR_PERIZIE_TEMP_MVC WHERE IDPErito =  @IDPErito AND IDModello IS NULL";
            deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPErito", IDPerito));
        }

        public ActionResult SalvaPeriziaDettagliRapid(string myIDPerizia, string myIDParte, bool IsUpdate = false, string ErrMess="")
        {
            var model = new Models.HomeModel();
            bool ISGravitaEnabled = true;

            // Carichiamo UN PO' DI DATI...
            // *******************************
            bool ISGEFCO_GN_51 = pISGEFCO_GN_51(myIDPerizia);

            // Dati per dropdown AGR_Parti
            if (ISGEFCO_GN_51)
            {
                var parti = from m in db.WEB_AGR_Parti_vw
                            where m.IDCliente == "**"
                            where m.IDCasa == "RTB"
                            select m;
                model.WEB_AGR_Parti_vw = parti.ToList();
                var ElencoParti = new SelectList(model.WEB_AGR_Parti_vw.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
                ViewData["ElencoParti"] = ElencoParti;
            }
            else
            {
                var parti = from m in db.WEB_AGR_Parti_vw
                            where m.IDCliente == "**"
                            where m.IDCasa != "RTB"
                            select m;
                model.WEB_AGR_Parti_vw = parti.ToList();
                var ElencoParti = new SelectList(model.WEB_AGR_Parti_vw.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
                ViewData["ElencoParti"] = ElencoParti;
            }




            // Dati per dropdown AGR_Danni
            var danni = from m in db.WEB_AGR_Danni_vw
                        where m.IDCliente == "**"

                        select m;
            model.WEB_AGR_Danni_vw = danni.ToList();
            var ElencoDanni = new SelectList(model.WEB_AGR_Danni_vw.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
            ViewData["ElencoDanni"] = ElencoDanni;


            // Dati per dropdown AGR_Gravita
            // *******************************
            if (ISGEFCO_GN_51)
            {
                var gravita = from m in db.WEB_AGR_Gravita_vw
                              where m.IDCliente == "FI"

                              select m;
                model.WEB_AGR_Gravita_vw = gravita.ToList();
                var ElencoGravita = new SelectList(model.WEB_AGR_Gravita_vw.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
                ViewData["ElencoGravita"] = ElencoGravita;
            }
            else
            {
                var gravita = from m in db.WEB_AGR_Gravita_vw
                              where m.IDCliente == "NULL"

                              select m;
                model.WEB_AGR_Gravita_vw = gravita.ToList();
                var ElencoGravita = new SelectList(model.WEB_AGR_Gravita_vw.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
                ViewData["ElencoGravita"] = ElencoGravita;
                ISGravitaEnabled = false;

            }

            var Dettagli = from m in db.AGR_PERIZIE_DETT_TEMP_MVC_vw
                           where m.IDPerizia == myIDPerizia
                           select m;
            model.AGR_PERIZIE_DETT_TEMP_MVC_vw = Dettagli.ToList();
            ViewBag.IDPerizia = myIDPerizia;

            var test = (from m in db.AGR_PERIZIE_TEMP_MVC
                        where m.ID.ToString() == myIDPerizia
                        select new { m.IDPerito, m.IDSpedizione, m.IDMeteo, m.IDTipoPerizia, m.IDModello, m.ID }).FirstOrDefault();

            var test2 = (from m in db.AGR_PerizieExpGrim_Temp_MVC
                         where m.ID.ToString() == myIDPerizia
                         select new { m.ID_TipoRotabile, m.ID_TrasportatoreGrimaldi }).FirstOrDefault();

            ViewBag.IDPerito = test.IDPerito;
            ViewBag.IDSpedizione = test.IDSpedizione;
            ViewBag.IDMeteo = test.IDMeteo;
            ViewBag.IDTP = test.IDTipoPerizia;
            ViewBag.aIDTrasportatore = test2.ID_TrasportatoreGrimaldi;
            ViewBag.aIDTipoRotabile = test2.ID_TipoRotabile;
            ViewBag.aIDModelloCasa = test.IDModello;
            ViewBag.IDPeriz = test.ID;
            ViewBag.ISGravitaEnabled = ISGravitaEnabled;
            ViewBag.IsUpdate = IsUpdate;
            ViewBag.ErrMess = ErrMess;
            return View(model);
        }

        [HttpPost]
        public ActionResult SalvaPeriziaDettagliRapid(string myIDPerizia, string IDParte, string IDDanno, string Qta, string Note, string Flags,
            string IDGravita, string IDResponsabilita, string IDAttribuzione, bool IsUpdate = false)
        {
            string myMessDett = "";
            bool isOK = CheckAllDetails(IDParte, IDDanno , out myMessDett);

            if (isOK)
            {
                myMessDett = "";
                // Inserisco dettagli danno perizia
                if (String.IsNullOrEmpty(Flags))
                    Flags = "";
                if (String.IsNullOrEmpty(IDResponsabilita))
                    IDResponsabilita = "";
                if (String.IsNullOrEmpty(Flags))
                    Flags = "";
                if (String.IsNullOrEmpty(IDAttribuzione))
                    IDAttribuzione = "";
                if (String.IsNullOrEmpty(IDGravita))
                    IDGravita = "";

                if (!String.IsNullOrEmpty(myIDPerizia) && !String.IsNullOrEmpty(IDParte) && !String.IsNullOrEmpty(IDDanno) && !String.IsNullOrEmpty(myIDPerizia))
                {

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
                }

                //var model = new Models.HomeModel();
                //var Detatgli = from m in db.AGR_PERIZIE_DETT_TEMP_MVC_vw
                //               where m.IDPerizia == aIDPerizia
                //               select m;
                //model.AGR_PERIZIE_DETT_TEMP_MVC_vw = Detatgli.ToList();
               
                return RedirectToAction("SalvaPeriziaDettagliRapid", "TelaiAnagrafica", new { myIDPerizia, IsUpdate = IsUpdate, ErrMess = myMessDett });
            }
            else
            {
               
                return RedirectToAction("SalvaPeriziaDettagliRapid", "TelaiAnagrafica", new { myIDPerizia, IsUpdate = IsUpdate , myIDParte = IDParte, ErrMess = myMessDett });
            }

            //return View(model);
        }
    }
}