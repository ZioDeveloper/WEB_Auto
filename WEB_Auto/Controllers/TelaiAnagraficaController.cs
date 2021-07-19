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

        public bool EsisteStessoOggetto(string Telaio , string IDSpedizione , string DataPerizia)
        {

            var Attuale = (from m in db.AGR_Spedizioni
                           where m.ID == IDSpedizione
                           select new { m.DataPartenzaImbarco,m.IDPortoImbarco,m.IDPortoSbarco }).FirstOrDefault();
            DateTime DataAttuale = (DateTime)Attuale.DataPartenzaImbarco;
            string POL = Attuale.IDPortoImbarco.ToString();
            string POD = Attuale.IDPortoSbarco.ToString();

            var Precedente = (from m in db.AGR_Spedizioni
                              where m.IDPortoImbarco == POL
                              where m.IDPortoSbarco == POD
                              where m.DataPartenzaImbarco < DataAttuale
                              select new { m.DataInizioImbarco, m.IDPortoImbarco, m.IDPortoSbarco }).OrderByDescending(s=>s.DataInizioImbarco).FirstOrDefault();



            return false;
        }
        public ActionResult InputTelaio(string IDPerito, string IDSpedizione,string IDMeteo, string IDTP, string Chassis,string aIDModello,string DataPerizia, bool IsRTB = false)
        {
            if(!String.IsNullOrEmpty(IDPerito))
                EliminaTelaiSenzaModello(IDPerito);

            // verifica TP
            var Spedizioni = (from m in db.AGR_SpedizioniWEB_vw
                             where m.ID == IDSpedizione
                             select m).FirstOrDefault();
           if(Spedizioni.IDPortoImbarco == Session["IDPortoPerito"].ToString())
            {
                if(IDTP=="D")
                {
                    return RedirectToAction("TipoPeriziaErrato","TelaiAnagrafica", new { Message = "Non puoi effettuare Post Discharge su questa spedizione : " + IDSpedizione });
                }
            }
           else
            {
                if (IDTP == "C")
                {
                    return RedirectToAction("TipoPeriziaErrato", "TelaiAnagrafica", new { Message = "Non puoi effettuare Pre Load su questa spedizione : " + IDSpedizione });
                }
            }

            int chiuse = (from m in db.AGR_PERIZIE_TEMP_MVC
                      
                       where m.IDSpedizione == IDSpedizione
                       where m.IsClosed == true
                       select m.ID).Count();
            if (chiuse > 0)
                return View("SpedizioneChiusa");

            if(EsisteStessoOggetto(Chassis, IDSpedizione, DataPerizia))
            { return View("SpedizioneChiusa"); }

            if (String.IsNullOrEmpty(IDSpedizione))
            {
                string usr = Session["User"].ToString();
                return RedirectToAction("Index", "Home", new { usr = usr, errMess = "Selezionare una spedizione" });
            }

            string Test = Session["RTB"].ToString();

            if (Session["RTB"].ToString().ToUpper() == "TRUE" && IsRTB)
                IsRTB = true;

            if(!String.IsNullOrEmpty(DataPerizia))
                Session["DataRicorda"] = DataPerizia;
           

            if (!String.IsNullOrEmpty(Session["DataRicorda"].ToString()))
            {
                ViewBag.DataRipetuta = Session["DataRicorda"].ToString();
            }
            else
            {
                ViewBag.DataRipetuta = "";
            }


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

            if (!String.IsNullOrEmpty(Chassis))
            {
                if (Chassis.Length > 8)
                    Chassis = Chassis.Right(8);
            }

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
            if (!String.IsNullOrEmpty(Chassis) && Session["Classe"].ToString() == "0")
            {
                string myTelaio = Chassis.ToUpper();

                if (StessoTelaioDifferenteViaggio(Chassis, IDSpedizione, IDTP, DataPerizia))
                {
                    // Se esiste il telaio per un viaggio differente, deve fare UPDATE dei dati prima perizia ed assegnarli a seconda
                    
               
                    DateTime myDate = DateTime.Now;
                    var model = new Models.HomeModel();

                    var datixmodel = from m in db.WEB_ListaPerizieFlat_MVC_vw
                                     where m.Telaio == Chassis
                                              where m.IDSpedizione != IDSpedizione
                                              where m.IDTipoPerizia == IDTP
                                              where m.DataPerizia < myDate
                                              where m.IsClosed == false
                                              select m;
                    model.WEB_ListaPerizieFlat_MVC_vw = datixmodel.ToList();
                    ViewBag.IDPerito = IDPerito;
                    ViewBag.IDSpedizione = IDSpedizione;
                    ViewBag.IDMeteo = IDMeteo;
                    ViewBag.IDTP = IDTP;
                    ViewBag.IDModello = aIDModello;
                    ViewBag.IsModelActive = false;
                    return View("TelaioEsistente",model);

                }


                //string ID_Trasportatore = "";
                //string ID_TipoRotabile = "";
                //string flagNU = "";
                //string Annotazioni = "";
                //var dati = (from m in db.AGR_Perizie_MVC_Flat_vw
                //            where m.Telaio == myTelaio
                //            where m.IDSpedizione != IDSpedizione

                //            select new { m.ID_TrasportatoreGrimaldi, m.ID_TipoRotabile, m.IDModello, m.ID, m.FlgNuovoUsato, m.Note,m.IDSpedizione }).FirstOrDefault();
                //try
                //{
                //    ID_Trasportatore = dati.ID_TrasportatoreGrimaldi.ToString();
                //}
                //catch { ID_Trasportatore = ""; }
                //try { ID_TipoRotabile = dati.ID_TipoRotabile.ToString(); }
                //catch { ID_TipoRotabile = ""; }
                //string myIDPerizia = dati.ID.ToString();
                //try { flagNU = dati.FlgNuovoUsato.ToString(); } catch { flagNU = ""; }
                //try { Annotazioni = dati.Note.ToString(); } catch { Annotazioni = ""; }
                //string IDModello = dati.IDModello.ToString();
                //return RedirectToAction("Edit", "TelaiAnagrafica", new
                //{
                //    IDPerito = IDPerito,
                //    IDSpedizione = dati.IDSpedizione,
                //    IDMeteo = IDMeteo,
                //    IDTP = IDTP,
                //    aIDTrasportatore = ID_Trasportatore,
                //    aIDTipoRotabile = ID_TipoRotabile,
                //    aIDModelloCasa = IDModello,
                //    myIDPerizia = myIDPerizia,
                //    Annotazioni = Annotazioni,
                //    errMess = "Sovrascrivo !",
                //    OldIDSpedizione = IDSpedizione

                //});



                int cnt = (from m in db.AGR_PERIZIE_TEMP_MVC
                           where m.Telaio == Chassis
                           where m.IDSpedizione == IDSpedizione
                           where m.IDTipoPerizia == IDTP
                           select m.ID).Count();

                if (cnt == 0)
                {

                   string myIDPerizia = CreaNuovaPerizia(IDPerito, IDSpedizione, IDMeteo, IDTP, myTelaio,DataPerizia);
                   return RedirectToAction("Edit", "TelaiAnagrafica", new { IDPerito = IDPerito, IDSpedizione=  IDSpedizione, IDMeteo= IDMeteo,
                       IDTP = IDTP,  Chassis=Chassis, myIDPerizia= myIDPerizia,  aIDModelloCasa = aIDModello });
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

            else if (!String.IsNullOrEmpty(Chassis) && Session["Classe"].ToString() == "1")
            {
                string myTelaio = Chassis.ToUpper();

                int cnt = (from m in db.AGR_PERIZIE_TEMP_MVC
                           where m.Telaio == Chassis
                           where m.IDSpedizione == IDSpedizione
                           where m.IDTipoPerizia == IDTP
                           select m.ID).Count();

                if (cnt == 0)
                {

                    string myIDPerizia = CreaNuovaPerizia(IDPerito, IDSpedizione, IDMeteo, IDTP, myTelaio,null);
                    return RedirectToAction("Edit", "TelaiAnagrafica", new
                    {
                        IDPerito = IDPerito,
                        IDSpedizione = IDSpedizione,
                        IDMeteo = IDMeteo,
                        IDTP = IDTP,
                        Chassis = Chassis,
                        myIDPerizia = myIDPerizia,
                        aIDModelloCasa = aIDModello,
                        ToDoRefresh = true
                    });
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
                                select new { m.ID_TrasportatoreGrimaldi, m.ID_TipoRotabile, m.IDModello, m.ID, m.FlgNuovoUsato, m.Note }).FirstOrDefault();
                    try
                    {
                        ID_Trasportatore = dati.ID_TrasportatoreGrimaldi.ToString();
                    }
                    catch { ID_Trasportatore = ""; }
                    try { ID_TipoRotabile = dati.ID_TipoRotabile.ToString(); }
                    catch { ID_TipoRotabile = ""; }
                    string myIDPerizia = dati.ID.ToString();
                    try { flagNU = dati.FlgNuovoUsato.ToString(); } catch { flagNU = ""; }
                    try { Annotazioni = dati.Note.ToString(); } catch { Annotazioni = ""; }
                    string IDModello = dati.IDModello.ToString();
                    return RedirectToAction("Edit", "TelaiAnagrafica", new
                    {
                        IDPerito = IDPerito,
                        IDSpedizione = IDSpedizione,
                        IDMeteo = IDMeteo,
                        IDTP = IDTP,
                        aIDTrasportatore = ID_Trasportatore,
                        aIDTipoRotabile = ID_TipoRotabile,
                        aIDModelloCasa = IDModello,
                        myIDPerizia = myIDPerizia,
                        Annotazioni = Annotazioni,
                        errMess = "In modifica"

                    });

                }
            }


            // Se invece devo ancora inputare il telaio...
            if (Session["RTB"].ToString().ToUpper() == "TRUE" || IsRTB)
                aIDModello = null;
            ViewBag.IDPerito = IDPerito;
            ViewBag.Casa = aCasa;
            ViewBag.IDSpedizione = IDSpedizione;
            ViewBag.IDMeteo = IDMeteo;
            ViewBag.IDTP = IDTP;
            ViewBag.aIDModello = aIDModello;
            return View();
        }


        public string CreaNuovaPerizia(string IDPerito, string IDSpedizione, string IDMeteo, string IDTP, string Chassis, string aDataPerizia)
        {

            var charsToRemove = new string[] { "@", ",", ";", "-", "_", "'" ,"?","^","|" ,"!",":",".","#","[", "]"};
            foreach (var c in charsToRemove)
            {
                Chassis = Chassis.Replace(c, string.Empty);
            }
            DateTime DataPerizia = DateTime.Now;
            DateTime myDate;
            if (!String.IsNullOrEmpty(aDataPerizia))
            {
                //DateTime myDate = DateTime.ParseExact("2009-05-08 14:40:52,531", "yyyy-MM-dd HH:mm:ss,fff",
                //                       System.Globalization.CultureInfo.InvariantCulture);
                DateTime ora = DateTime.Now;
                string ore = ora.Hour.ToString("00");
                string minuti = ora.Minute.ToString("00"); ;
                string secondi = ora.Second.ToString("00"); ;

                string myISoDate = aDataPerizia.Right(4) + aDataPerizia.Substring(3, 2) + aDataPerizia.Left(2) + " " + ore + ":" + minuti + ":" + secondi;
                myDate = DateTime.ParseExact(myISoDate, "yyyyMMdd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            }
            else
            {
                myDate = DataPerizia;
            }


            string myIDPerizia = GetNewCode_AUTO(IDPerito,IDSpedizione);
            string OS = Session["OS"].ToString();
            string myNote = "";
            // togliere note automatiche meteo 19/05/2021
            //if (IDMeteo == "3" || IDMeteo == "4")
            //    myNote = "No foto causa pioggia ";

            //string myDataPerizia = DataPerizia.Substring(6, 4) + DataPerizia.Substring(3, 2) + DataPerizia.Substring(0, 2);
            string sqlcmd = " INSERT INTO AGR_PERIZIE_Temp_MVC (ID, IDSpedizione, IDPerito, IDTipoPerizia, DataPerizia, IDNazione, Telaio, NumFoto, " +
                            "  Flags, IRichiesta, IDefinizione, IContab, DataModPerito, FlagControllo, IDMeteo, NumPDF,Note,IDOperatore,MachineName) " +
                            "VALUES (@ID, @IDSpedizione, @IDPerito, @IDTipoPerizia, @DataPerizia, @IDNazione, @Telaio, @NumFoto, " +
                            "  @Flags, @IRichiesta, @IDefinizione, @IContab, @DataModPerito, @FlagControllo, @IDMeteo, @NumPDF, @Note,@IDOperatore,@MachineName)";
            int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@ID", myIDPerizia),
                                                                 new SqlParameter("@IDSpedizione", IDSpedizione),
                                                                 new SqlParameter("@IDPerito", IDPerito),
                                                                 new SqlParameter("@IDTipoPerizia", IDTP),
                                                                 new SqlParameter("@DataPerizia", myDate),
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
                                                                 new SqlParameter("@Note", myNote),
                                                                 new SqlParameter("@IDOperatore", (int)Session["IDOperatore"]),
                                                                 new SqlParameter("@MachineName", OS));
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
                                         string errMess = " ", bool IsUpdate = false,bool ToDoRefresh = false, string OldIDSpedizione = "") // errMess = " " per eludere primo controllo in View Edit
        {
            // Default = modello, diventa trasportatore per CAB non rotabili
            ViewBag.IsTrasportatore = false;
            ViewBag.ToDoRefresh = ToDoRefresh;
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
                var modello = from m in db.AGR_ModelliAuto_vw
                              where m.IDCliente == "**"
                              where m.IDCasa == aCasa
                              where m.IDModelloCasa == "1240"
                              select m;
                model.AGR_ModelliAuto_vw = modello.ToList().OrderBy(m => m.Descr);
                var ElencoModelli = new SelectList(model.AGR_ModelliAuto_vw.ToList(), "ID", "Descr");
                ViewData["ElencoModelli"] = ElencoModelli;
                aIDModelloCasa = "1240";
                ViewBag.aIDModelloCasa = "1240";
            }
            if (Session["RTB"].ToString().ToUpper() == "TRUE" && aCasa == "RTB")
            {
                var modello = from m in db.AGR_ModelliAuto_vw
                              where m.IDCliente == "**"
                              where m.IDCasa == aCasa
                              where m.IDModelloCasa == "1241"
                              select m;
                model.AGR_ModelliAuto_vw = modello.ToList().OrderBy(m => m.Descr);
                var ElencoModelli = new SelectList(model.AGR_ModelliAuto_vw.ToList(), "ID", "Descr");
                ViewData["ElencoModelli"] = ElencoModelli;
                aIDModelloCasa = "1241";
                ViewBag.aIDModelloCasa = "1240";
            }
            else
            {

                
                if (aCasa == "CAB" && Filtrati && Session["RTB"].ToString().ToUpper() != "TRUE" && String.IsNullOrEmpty(aIDModelloCasa))
                {
                    var modello = from m in db.AGR_ModelliAuto_vw
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
                    model.AGR_ModelliAuto_vw = modello.ToList().OrderBy(m => m.Descrizione);
                    var ElencoModelli = new SelectList(model.AGR_ModelliAuto_vw.ToList(), "ID", "Descr");
                    ViewData["ElencoModelli"] = ElencoModelli;
                    ViewBag.IsTrasportatore = false;
                }
                else if (aCasa == "CAB" && Filtrati && Session["RTB"].ToString().ToUpper() == "TRUE" && String.IsNullOrEmpty(aIDModelloCasa))
                {
                    var modello = from m in db.AGR_ModelliAuto_vw
                                  where m.IDCliente == "**"
                                  where m.IDCasa == aCasa
                                  where m.IDModelloCasa == "1240"
                                  select m;
                    model.AGR_ModelliAuto_vw = modello.ToList().OrderBy(m => m.Descr);
                    var ElencoModelli = new SelectList(model.AGR_ModelliAuto_vw.ToList(), "ID", "Descr");
                    ViewData["ElencoModelli"] = ElencoModelli;
                    ViewBag.IsTrasportatore = false;


                }

                else
                {
                    var myTelaio = (from m in db.AGR_Perizie_MVC_Flat_vw
                                  where m.ID == myIDPerizia
                                  select m.Telaio).FirstOrDefault();

                    var cnt = (from m in db.AGR_Perizie_MVC_Flat_vw
                                    where m.Telaio == myTelaio
                                    select m.Telaio).Count();

                    var myModello = (from m in db.AGR_Perizie_MVC_Flat_vw
                                     where m.Telaio == myTelaio
                                     select  new { m.IDModello }).FirstOrDefault();
                   


                    if (cnt == 1)
                    {
                        var modello = from m in db.AGR_ModelliAuto_vw
                                      where m.IDCliente == "**"
                                      where m.IDCasa == aCasa
                                      select m;
                        model.AGR_ModelliAuto_vw = modello.ToList().OrderBy(m => m.Descr);
                        var ElencoModelli = new SelectList(model.AGR_ModelliAuto_vw.ToList(), "ID", "Descr");
                        ViewBag.IsTrasportatore = false;
                        ViewData["ElencoModelli"] = ElencoModelli;
                    }
                    else
                    {
                        var modello = from m in db.AGR_ModelliAuto_vw
                                      where m.IDCliente == "**"
                                      where m.IDCasa == aCasa
                                      //where m.IDModelloCasa == myModello.IDModello.ToString()
                                      select m;
                        model.AGR_ModelliAuto_vw = modello.ToList().OrderBy(m => m.Descr);
                        var ElencoModelli = new SelectList(model.AGR_ModelliAuto_vw.ToList(), "ID", "Descr");
                        ViewBag.IsTrasportatore = false;
                        ViewBag.aIDModelloCasa = myModello.IDModello.ToString();
                        ViewData["ElencoModelli"] = ElencoModelli;

                    }
                }

            }
            if (!String.IsNullOrEmpty(dati.IDModello.ToString()))
            {
                ViewBag.aIDModelloCasa = aIDModelloCasa; //dati.IDModello;
            }
            else
            {
                if (String.IsNullOrEmpty(ViewBag.aIDModelloCasa))
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


            // Aggiorna campo note x pioggia
            if (HasDamages(myIDPerizia))
            {
                if (IDMeteo == "3" || IDMeteo == "4")
                {
                    string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                                    " SET  Note = @Note " +
                                    " WHERE ID = @IDPerizia";

                    int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", myIDPerizia),
                                                                         new SqlParameter("@Note", "No foto causa pioggia"));
                }

            }
            // togliere note automatiche meteo 19/05/2021
            //if (IDMeteo == "3" || IDMeteo == "4")
            //    myNote = "No foto causa pioggia ";


            // Cerco Trasportatore Grimaldi e Tipo rotabile pregressi e li uso...
            if ((aIDModelloCasa == "1240" || aIDModelloCasa == "1241" )&& (String.IsNullOrEmpty(aIDTrasportatore)&& String.IsNullOrEmpty(aIDTipoRotabile)))
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
                    if (HasDamages(myIDPerizia))
                    {

                    }

                    // Aggiorno dati perizia
                        string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                                    " SET  IDModello = @IDModello " +
                                    " WHERE ID = @IDPerizia";


                    int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", myIDPerizia),
                                                                         
                                                                         new SqlParameter("@IDModello", aIDModelloCasa));

                    sqlcmd = " UPDATE AGR_PerizieExpGrim_Temp_MVC   " +
                                                  "  SET ID_TrasportatoreGrimaldi = @ID_TrasportatoreGrimaldi, " +
                                                  "  ID_TipoRotabile = @ID_TipoRotabile "  +
                                                  " WHERE ID = @ID ";


                    

                    Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@ID", myIDPerizia),
                                                                     new SqlParameter("@ID_TrasportatoreGrimaldi", myIDTrasportatore),
                                                                     new SqlParameter("@ID_TipoRotabile", myIDTipoRotabile));

                    errMess = "TARGA CONOSCIUTA";
                }
                catch { }// bruciamo eccezione

            }

            // Dati per dropdown Spedizione
            if (Session["Classe"].ToString() == "0")
            {
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
            }
            else
            {
                var Spedizione = from m in db.AGR_SpedizioniWEB_vw
                                 where m.IDCliente == "BE"
                                 //where m.DataInizioImbarco >= ini
                                 select m;
                model.AGR_SpedizioniWEB_vw = Spedizione.ToList();
                var ElencoSpedizioni = new SelectList(model.AGR_SpedizioniWEB_vw.ToList(), "ID", "DescrMin");


                ViewData["ElencoSpedizioni"] = ElencoSpedizioni;
                ViewBag.IDSpedizione = IDSpedizione;
            }


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

            var Dettagli = from m in db.AGR_PERIZIE_DETT_TEMP_MVC_vw
                           where m.IDPerizia == myIDPerizia
                           select m;
            model.AGR_PERIZIE_DETT_TEMP_MVC_vw = Dettagli.ToList();

            ViewBag.OldIDSpedizione = OldIDSpedizione;
            return View(model);
        }

        public ActionResult SalvaPeriziaTesta(string IDPerito, string IDSpedizione, string IDMeteo, string IDTP, string Chassis, string DataPerizia, string IDModelloCasa, string IDTrasportatoreGrim,
                                              string IDTipoRotabile, bool? isDamaged, string Condizione, string Annotazioni, string myIDPerizia, 
                                              bool IsUpdate = false,bool Filtrati=true, bool isRapid = false, bool ToDoRefresh = false)
        {
            if (isRapid == true)
                isDamaged = false;

            if (isDamaged == true)
                isRapid = false;

            DateTime ora = DateTime.Now;
            string ore = ora.Hour.ToString("00");
            string minuti = ora.Minute.ToString("00"); ;
            string secondi = ora.Second.ToString("00"); ;

            string myISoDate = DataPerizia.Right(4) + DataPerizia.Substring(3, 2) + DataPerizia.Left(2) + " " + ore + ":" + minuti + ":" + secondi;

            // Verifico Sia tutto ok.. to do !!!!!
            bool isOK = CheckAll(myIDPerizia, IDSpedizione, Chassis, IDModelloCasa, IDTrasportatoreGrim , IDTipoRotabile, Condizione , Annotazioni,DataPerizia,  IDTP,  out string myerrMess);
            if (isOK)
            {
                

                // Aggiorno dati perizia
                string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                                " SET IDSpedizione = @IDSpedizione , IDModello = @IDModello, Telaio = @Telaio, NumFoto = @NumFoto , FileNumber = 0 , Note = @Note,DataPerizia = @DataPerizia " +
                                " WHERE ID = @IDPerizia";
                //string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                //                " SET IDSpedizione = @IDSpedizione , IDModello = @IDModello, Telaio = @Telaio, NumFoto = @NumFoto , FileNumber = 0 , Note = @Note " +
                //                " WHERE ID = @IDPerizia";


                int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", myIDPerizia),
                                                                     new SqlParameter("@IDSpedizione", IDSpedizione),
                                                                     new SqlParameter("@IDModello", IDModelloCasa),
                                                                     new SqlParameter("@Telaio", Chassis),
                                                                     new SqlParameter("@NumFoto", "0"),
                                                                     new SqlParameter("@DataPerizia", myISoDate),
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

                    if (IDModelloCasa == "1240" || IDModelloCasa == "1241")
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

                            sqlcmd = " INSERT INTO AGR_PERIZIE_DETT_TEMP_MVC (IDPerizia,IDParte, IDDanno, QTA,Flags, Note)" +
                                     " VALUES(@IDPerizia,@IDParte, @IDDanno, @QTA,@Flags, @Note)";
                            try
                            {
                                Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", myIDPerizia),
                                                                             new SqlParameter("@IDParte", "045"),
                                                                             new SqlParameter("@IDDanno", "Y"),
                                                                             new SqlParameter("@QTA", 1),
                                                                              new SqlParameter("@Flags", "0"),
                                                                             new SqlParameter("@Note", "Danni da utilizzo")
                                                                             );
                            }
                            catch (Exception exc)
                            {
                                string a = exc.Message;
                            }
                        }
                        
                    }
                    else if (String.IsNullOrEmpty(Condizione) || Condizione == "N")
                    {
                        var hasdanni = (from m in db.AGR_PERIZIE_DETT_TEMP_MVC_vw
                                        where m.IDPerizia == myIDPerizia
                                        where m.IDParte == "045"
                                        where m.IDDanno == "Y"
                                        select m).Count();
                        
                            sqlcmd = " DELETE FROM AGR_PERIZIE_DETT_TEMP_MVC " +
                                     "  WHERE IDPerizia = @IDPerizia " +
                                     " AND IDParte = @IDParte " +
                                     " AND IDDanno = @IDDanno";
                        
                        try
                        {
                            Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", myIDPerizia),
                                                                         new SqlParameter("@IDParte", "045"),
                                                                         new SqlParameter("@IDDanno", "Y")
                                                                         );
                        }
                        catch (Exception exc)
                        {
                            string a = exc.Message;
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

        public ActionResult SalvaPeriziaDettagli(string myIDPerizia, string myIDParte, bool IsUpdate= false, string ErrMess = "")
        {
            var model = new Models.HomeModel();
            bool ISGravitaEnabled = true;

            // Carichiamo UN PO' DI DATI...
            // *******************************
            bool ISGEFCO_GN_51 = pISGEFCO_GN_51(myIDPerizia);

            // Dati per dropdown AGR_Parti
            if (ISGEFCO_GN_51 && Session["Classe"].ToString() == "0")
            {
                var parti = from m in db.WEB_AGR_Parti_vw
                            where m.IDCliente == "**"
                            where m.IDCasa == "RTB"
                            select m;
                model.WEB_AGR_Parti_vw = parti.ToList();
                var ElencoParti = new SelectList(model.WEB_AGR_Parti_vw.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
                ViewData["ElencoParti"] = ElencoParti;
            }
            else if (Session["Classe"].ToString() == "1")
            {
                var parti = from m in db.AGR_Parti_SDU
                          
                            select m;
                model.AGR_Parti_SDU = parti.ToList();
                var ElencoParti = new SelectList(model.AGR_Parti_SDU.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
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
            if (Session["Classe"].ToString() == "0")
            {

               
                var danni = from m in db.WEB_AGR_Danni_vw
                            where m.IDCliente == "**"

                            select m;
                model.WEB_AGR_Danni_vw = danni.ToList();
                var ElencoDanni = new SelectList(model.WEB_AGR_Danni_vw.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
                ViewData["ElencoDanni"] = ElencoDanni;
            }
            else if (Session["Classe"].ToString() == "1")
            {
                // Dati per dropdown AGR_Danni
                var danni = from m in db.AGR_Danni_SDU
                            select m;
                model.AGR_Danni_SDU = danni.ToList();
                var ElencoDanni = new SelectList(model.AGR_Danni_SDU.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
                ViewData["ElencoDanni"] = ElencoDanni;
            }


            // Dati per dropdown AGR_Gravita
            // *******************************
            if (ISGEFCO_GN_51 )
            {
                var gravita = from m in db.WEB_AGR_Gravita_vw
                              where m.IDCliente == "FI"
                              select m;
                model.WEB_AGR_Gravita_vw = gravita.ToList();
                var ElencoGravita = new SelectList(model.WEB_AGR_Gravita_vw.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
                ViewData["ElencoGravita"] = ElencoGravita;
            }
            else if ( Session["Classe"].ToString() == "1")
            {
                var gravita = from m in db.AGR_Gravita_SDU
                              select m;
                model.AGR_Gravita_SDU = gravita.ToList();
                var ElencoGravita = new SelectList(model.AGR_Gravita_SDU.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
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

            // Ricerca dettagli altre tipi perizia dello stesso telaio
            //var DettagliAltri = from m in db.AGR_PERIZIE_DETT_TEMP_MVC_vw
            //                    where m.IDPerizia == myIDPerizia
            //                    select m;
            //model.AGR_PERIZIE_DETT_TEMP_MVC_vw_Altri = DettagliAltri.ToList();

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
            ViewBag.ErrMess = ErrMess;
            return View(model);
        }

        [HttpPost]
        public ActionResult SalvaPeriziaDettagli(string myIDPerizia, string IDParte, string IDDanno, string Qta, string Note, string Flags,
            string IDGravita, string IDResponsabilita, string IDAttribuzione , bool IsUpdate = false)
        {
            
            string myMessDett = "";
            bool isOK = CheckAllDetails(IDParte, IDDanno, Note, myIDPerizia, out myMessDett);
            if (isOK)
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
                                                                         new SqlParameter("@Flags", "0"),
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
                return RedirectToAction("SalvaPeriziaDettagli", "TelaiAnagrafica", new { myIDPerizia, IsUpdate = IsUpdate });
            }
            else
            {
                return RedirectToAction("SalvaPeriziaDettagli", "TelaiAnagrafica", new { myIDPerizia, IsUpdate = IsUpdate, ErrMess = myMessDett });
            }
            
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

            if(!IsUpdate)
                return RedirectToAction("InputTelaio", "TelaiAnagrafica" ,new { IDPerito, IDSpedizione, IDMeteo, IDTP });
            else
                return RedirectToAction("EditSpedizione", "ListaPerizie", new { IDPerito, IDSpedizione, IDMeteo, IDTP });
            
        }

        public bool HasDamages(string aIDPerizia)
        {
            var cnt = (from m in db.AGR_PERIZIE_DETT_TEMP_MVC_vw
                       where m.IDPerizia == aIDPerizia
                       select m.ID).Count();

            return cnt > 0;
        }

        
        public ActionResult AggiornaSpedizioneEDataPerizia(string IDSpedizione, string IDPerizia, string IDMeteo , string IDTP, string IDPerito,string Chassis,string aIDModello, string IDModelloCasa,
                            bool IsInspecting = false, bool IsModelActive = false)
        {
            // Controlli
            string a = "";
            string myIDModello = "";
            string aNewModel = "";
            ViewBag.IsModelActive = IsModelActive;
            var oldPerizia = (from m in db.AGR_PERIZIE_TEMP_MVC
                       where m.ID == IDPerizia
                       select new { m.IDSpedizione , m.IDModello}).FirstOrDefault();
            myIDModello = oldPerizia.IDModello.ToString();

            var oldgest = (from m in db.AGR_Spedizioni
                           where m.ID == oldPerizia.IDSpedizione
                           select new { m.IDCliente, m.IDCasa }).FirstOrDefault();

            var newgest = (from m in db.AGR_Spedizioni
                           where m.ID == IDSpedizione
                           select new { m.IDCliente, m.IDCasa }).FirstOrDefault();

            try
            {
                aNewModel = IDModelloCasa;
                
            }
            catch { }

            if (oldgest.IDCliente != newgest.IDCliente)
            {
                if (oldgest.IDCliente == "GN" && oldPerizia.IDModello.ToString() == "1240")
                {
                    myIDModello = "1241";
                }
                if (oldgest.IDCliente == "51" && oldPerizia.IDModello.ToString() == "1241")
                {
                    myIDModello = "1240";
                }

                if(oldgest.IDCasa != newgest.IDCasa)
                {
                    DateTime myDate = DateTime.Now;
                    var model = new Models.HomeModel();

                    var datixmodel = from m in db.WEB_ListaPerizieFlat_MVC_vw
                                     where m.Telaio == Chassis
                                     where m.IDSpedizione != IDSpedizione
                                     where m.IDTipoPerizia == IDTP
                                     where m.DataPerizia < myDate
                                     where m.IsClosed == false
                                     select m;
                    model.WEB_ListaPerizieFlat_MVC_vw = datixmodel.ToList();
                    ViewBag.IDPerito = IDPerito;
                    ViewBag.IDSpedizione = IDSpedizione;
                    ViewBag.IDMeteo = IDMeteo;
                    ViewBag.IDTP = IDTP;
                    ViewBag.IDModello = aIDModello;
                    ViewBag.IsModelActive = true;
                    var modello = from m in db.AGR_ModelliAuto_vw
                                  where m.IDCliente == "**"
                                  where m.IDCasa == newgest.IDCasa
                                  
                                  select m;
                    model.AGR_ModelliAuto_vw = modello.ToList().OrderBy(m => m.Descr);
                    var ElencoModelli = new SelectList(model.AGR_ModelliAuto_vw.ToList(), "ID", "Descr");
                    ViewData["ElencoModelli"] = ElencoModelli;
                    ViewBag.IsTrasportatore = false;
                    
                    return View("TelaioEsistente", model);
                }
            }
            else
            {
                if (oldgest.IDCasa == "RTB" && (myIDModello != "1240" & myIDModello != "1241"))
                     return View("ModificaNonGestita");
                //string aa = "";

            }

            string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                                " SET DataPerizia = @DataPerizia , " +
                                "     IDSpedizione = @IDSpedizione, " + 
                                "     IDModello = @IDModello " + 
                                " WHERE ID = @IDPerizia";
                DateTime aNewDataPerizia = DateTime.Now;

                int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, 
                                                                new SqlParameter("@IDPerizia", IDPerizia),
                                                                new SqlParameter("@DataPerizia", aNewDataPerizia),
                                                                new SqlParameter("@IDSpedizione", IDSpedizione),
                                                                new SqlParameter("@IDModello", myIDModello));
                if(!IsInspecting)
                    return RedirectToAction("InputTelaio", "TelaiAnagrafica", new { IDPerito, IDSpedizione, IDMeteo, IDTP });
                else
                    return RedirectToAction("InputTelaio", "TelaiAnagrafica", new { IDPerito, IDSpedizione, IDMeteo, IDTP ,Chassis });


        }

        [HttpPost]
        public ActionResult AggiornaSpedizioneEDataPerizia(string IDSpedizione, string IDPerizia, string IDMeteo, string IDTP, string IDPerito, string Chassis, string aIDModello, string IDModelloCasa,
                           bool IsInspecting = false, bool IsModelActive = false, string diff = "" )
        {
            // Controlli
            string a = "";
            string myIDModello = "";
            string aNewModel = "";
            ViewBag.IsModelActive = IsModelActive;

            var oldPerizia = (from m in db.AGR_PERIZIE_TEMP_MVC
                              where m.ID == IDPerizia
                              select new { m.IDSpedizione, m.IDModello }).FirstOrDefault();
            myIDModello = oldPerizia.IDModello.ToString();

            var oldgest = (from m in db.AGR_Spedizioni
                           where m.ID == oldPerizia.IDSpedizione
                           select new { m.IDCliente, m.IDCasa }).FirstOrDefault();

            var newgest = (from m in db.AGR_Spedizioni
                           where m.ID == IDSpedizione
                           select new { m.IDCliente, m.IDCasa }).FirstOrDefault();

            var idVero = (from m in db.AGR_ModelliAuto
                          where m.IDCasa == newgest.IDCasa
                          where m.IDModelloCasa == IDModelloCasa
                          select m.ID).FirstOrDefault();

            myIDModello = idVero.ToString();

            string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                                " SET DataPerizia = @DataPerizia , " +
                                "     IDSpedizione = @IDSpedizione, " +
                                "     IDModello = @IDModello " +
                                " WHERE ID = @IDPerizia";
            DateTime aNewDataPerizia = DateTime.Now;

            int Inserted = db.Database.ExecuteSqlCommand(sqlcmd,
                                                            new SqlParameter("@IDPerizia", IDPerizia),
                                                            new SqlParameter("@DataPerizia", aNewDataPerizia),
                                                            new SqlParameter("@IDSpedizione", IDSpedizione),
                                                            new SqlParameter("@IDModello", myIDModello));
            if (!IsInspecting)
                return RedirectToAction("InputTelaio", "TelaiAnagrafica", new { IDPerito, IDSpedizione, IDMeteo, IDTP });
            else
                return RedirectToAction("InputTelaio", "TelaiAnagrafica", new { IDPerito, IDSpedizione, IDMeteo, IDTP, Chassis });


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
            var myPDF = (from f in db.WEB_AUTO_PDF
                         where f.IDPerizia == aIDPerizia
                          select f).Count();

            string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                                " SET NumFoto = @NumFoto, " +
                                "     NumPDF = @NumPDF " +
                                " WHERE ID = @IDPerizia";


            int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@NumFoto", myFoto), new SqlParameter("@NumPDF", myPDF), new SqlParameter("@IDPerizia", aIDPerizia));
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

        public int ContaDanni(string aIdPerizia)
        {
            int cnt = 0;
            cnt = (from m in db.AGR_PERIZIE_TEMP_MVC
                   where m.ID == aIdPerizia
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

        public bool CheckAll(string myIDPerizia ,string aIDSpedizione ,string aTelaio, string IDModelloCasa, string IDTrasportatoreGrim,  string IDTipoRotabile , string Condizione,
                             string Annotazioni,string DataPerizia,string IDTP, out string errMEss)
        {
            // Dati spedizione
            errMEss = "";

            // Controllo modello
            if(String.IsNullOrEmpty(IDModelloCasa))
            {
                if(Session["Classe"].ToString() =="0")
                { errMEss = "Modello obbligatorio"; return false; }
                else
                { errMEss = "Marca costruttore obbligatoria"; return false; }
            }
        
            // Usato NUovo
            if(!String.IsNullOrEmpty(IDModelloCasa))
            {
                if(IDModelloCasa != "1240" && IDModelloCasa !="1241" && String.IsNullOrEmpty(Condizione))
                { errMEss = "Nuovo / Usato info obbligatoria"; return false; }

            }

            //if(ContaFoto(myIDPerizia)<2)
            //{
            //    if(ContaDanni(myIDPerizia) > 0)
            //    {
            //        errMEss += "Numero foto insufficiente ! ";
            //        return false;
            //    }

                
            //}

            //if(Condizione == "U")
            //{
            //    var hasdanni = (from m in db.AGR_PERIZIE_DETT_TEMP_MVC_vw
            //                    where m.IDPerizia == myIDPerizia
            //                    where m.IDParte == "045"
            //                    where m.IDDanno == "Y"
            //                    select m).Count();
            //    if(hasdanni==0)
            //    {
            //        errMEss += "Manca dettaglio 054 - Y ";
            //        return false;
            //    }
            //}
            
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


            if (Session["Classe"].ToString() == "0")
            {
                var DataxConfronto = (from m in db.AGR_Spedizioni
                                      where m.ID == aIDSpedizione
                                      select new { m.DataPartenzaImbarco , m.DataArrivoSbarco}).FirstOrDefault();


                DateTime myDate = DateTime.ParseExact(DataPerizia, "dd/MM/yyyy",
                                           System.Globalization.CultureInfo.InvariantCulture);

                if (IDTP == "C")
                {
                    if (myDate > DataxConfronto.DataPartenzaImbarco)
                    {

                        errMEss += "Data errata :  non deve superare data partenza !";
                        return false;
                    }
                }
                if (IDTP == "D")
                {
                    if (myDate < DataxConfronto.DataArrivoSbarco)
                    {

                        errMEss += "Data errata :  non deve essere minore della data partenza !";
                        return false;
                    }
                }

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

            if (IDTrasportatoreGrim == "0" && String.IsNullOrEmpty(Annotazioni))
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

        public bool CheckAllDetailsRapid(string IDParte, string IDDanno ,string Note,string myIDPerizia, out string errMEss)
        {
            int contaitems = (from m in db.AGR_PERIZIE_DETT_TEMP_MVC
                              where m.IDPerizia == myIDPerizia
                              where m.IDParte == IDParte
                              where m.IDDanno == IDDanno
                              select m.ID).Count();
            if (contaitems > 0)
            {
                errMEss = "Esiste già identico item /tipo danno !";
                return false;
            }

            if (String.IsNullOrEmpty(IDParte))
            {
                errMEss = "Inserire codice parte...";
                return false;
            }
            else if (String.IsNullOrEmpty(IDDanno))
            {
                errMEss = "Inserire codice danno...";
                return false;
            }

            else if (IDParte.Left(1) != "2" && IDParte.Left(1) != "3")
            {
                errMEss = "Codice parte danneggiata NON VALIDO";
                return false;
            }
            else if((IDParte == "300" || IDParte == "045") && String.IsNullOrEmpty(Note))
            {
                errMEss = "Note obbligatorie per questo componente";
                return false;

            }

           
            else
            {
                errMEss = "";
                return true;
            }
        }

        public bool CheckAllDetails(string IDParte, string IDDanno, string Note, string myIDPerizia, out string errMEss)
        {
            int contaitems = (from m in db.AGR_PERIZIE_DETT_TEMP_MVC
                              where m.IDPerizia == myIDPerizia
                              where m.IDParte == IDParte
                              where m.IDDanno == IDDanno
                              select m.ID).Count();
            if (contaitems > 0)
            {
                errMEss = "Esiste già identico item /tipo danno !";
                return false;
            }


            if (String.IsNullOrEmpty(IDParte))
            {
                errMEss = "Inserire codice parte...";
                return false;
            }
            else if (String.IsNullOrEmpty(IDDanno))
            {
                errMEss = "Inserire codice danno...";
                return false;
            }

            else if ((IDParte == "300" || IDParte == "045") && String.IsNullOrEmpty(Note))
            {
                errMEss = "Note obbligatorie per questo componente";
                return false;

            }

           

            else
            {
                errMEss = "";
                return true;
            }
        }
        public void EliminaTelaiSenzaModello(string IDPerito, string aIDModello = null, string Chassis = null)
        {
            
            if (String.IsNullOrEmpty(aIDModello))
                {
                string sqlcmd = " DELETE FROM AGR_PerizieExpGrim_Temp_MVC WHERE ID IN(SELECT ID FROM AGR_PERIZIE_TEMP_MVC WHERE IDPErito = @IDPErito AND IDOperatore = @IDOperatore AND IDModello IS NULL)";
                int deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPErito", IDPerito), new SqlParameter("@IDOperatore", (int)Session["IDOperatore"]));
                sqlcmd = " DELETE FROM AGR_PERIZIE_TEMP_MVC WHERE IDPErito =  @IDPErito AND IDOperatore = @IDOperatore AND IDModello IS NULL";
                deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPErito", IDPerito), new SqlParameter("@IDOperatore", (int)Session["IDOperatore"]));
            }
            else
            {
                string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                                                    " SET  IDModello = @IDModello " +
                                                    " WHERE Telaio = @Chassis" +
                                                    " AND IDPErito = @IDPErito " +
                                                    " AND IDModello IS NULL ";


                int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPErito", IDPerito),
                                                                     new SqlParameter("@IDModello", aIDModello),
                                                                     new SqlParameter("@Chassis", Chassis));
            }
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
            IDDanno = IDDanno.ToUpper();
            bool isOK = CheckAllDetailsRapid(IDParte, IDDanno , Note , myIDPerizia, out myMessDett);

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

                // Decodifico Gravità
                if (IDGravita == "*")
                    IDGravita = "*";

                if (IDGravita == "1")
                    IDGravita = "08";

                if (IDGravita == "2")
                    IDGravita = "09";

                if (IDGravita == "3")
                    IDGravita = "10";

                if (IDGravita == "4")
                    IDGravita = "11";

                if (IDGravita == "5")
                    IDGravita = "12";

                if (IDGravita == "6")
                    IDGravita = "13";


                if (!String.IsNullOrEmpty(myIDPerizia) && !String.IsNullOrEmpty(IDParte) && !String.IsNullOrEmpty(IDDanno) && !String.IsNullOrEmpty(myIDPerizia))
                {

                    string sqlcmd = " INSERT INTO AGR_PERIZIE_DETT_TEMP_MVC (IDPerizia, IDParte, IDDanno, Qta, Note, Flags, IDGravita, IDResponsabilita, IDAttribuzione) " +
                                   "VALUES (@IDPerizia, @IDParte, @IDDanno, @Qta, @Note, @Flags, @IDGravita, @IDResponsabilita, @IDAttribuzione)";
                    int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", myIDPerizia),
                                                                         new SqlParameter("@IDParte", IDParte),
                                                                         new SqlParameter("@IDDanno", IDDanno),
                                                                         new SqlParameter("@Qta", Qta),
                                                                         new SqlParameter("@Note", Note),
                                                                         new SqlParameter("@Flags", "0"),
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

        public bool StessoTelaioDifferenteViaggio(string Chassis, string IDSpedizione, string IDTP, string DataPerizia)
        {
            //DateTime myDate = DateTime.ParseExact(DataPerizia, "dd/MM/yyyy",
            //                              System.Globalization.CultureInfo.InvariantCulture);
             DateTime myDate = DateTime.Now;
            int cnt = (from m in db.AGR_PERIZIE_TEMP_MVC
                       where m.Telaio == Chassis
                       where m.IDSpedizione != IDSpedizione
                       where m.IDTipoPerizia == IDTP
                       where m.DataPerizia < myDate
                       where m.IsClosed == false
                       select m.ID).Count();

            return cnt > 0;
        }

        public ActionResult TipoPeriziaErrato(string Message)
        {
            ViewBag.MEssage = Message;
            return View();
        }
    }
}