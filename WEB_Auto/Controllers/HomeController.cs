using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WEB_Auto.Models;

namespace WEB_Auto.Controllers
{
    public class HomeController : ExtendedController
    {

        private wisedbEntities db  = new wisedbEntities();

        
        public ActionResult ChangeLanguage(string lang)
        {
            new LanguageMang().SetLanguage(lang);
            Session["ISLanguageDecided"] = true;
            return RedirectToAction("Index", "Home", new { lang });
        }

        public ActionResult Index(string usr, string Filtro = "OGGI", string errMess = "", string IDPorto = "", string aSpedizione = "")
        {
            var model = new Models.HomeModel();

            
            //string a = Request.Browser.Platform;
            //a = Request.Browser.Version;
            //string MachineName = "";
            string OS = "";

            Session["DataRicorda"] = "";

            OS = Request.UserAgent;

            if (OS.ToUpper().Contains("ANDROID"))
                OS = "ANDROID";
            else if (OS.ToUpper().Contains("WINDOWS"))
                OS = "WINDOWS";
                
            else
                OS = "UNKNOWN";


            Session["OS"] = OS;

            if (String.IsNullOrEmpty(usr))
            {
              // usr = "caminita"; // pierangeli
                // usr = "Astrea"; // pierangeli
                //usr = "VGrimaldi";
                //usr = "grimaldi"; // 
               // usr = "pierangeli"; // 
               // usr = "patrizia"; // 
               // usr = "mmarti"; // 
               // usr = "DiGennaro";
               //usr = "DiSalvo";
              // usr = "patrizia";
                //usr = "Torresan"; // 
                usr = "DiNinno";
                // usr = "Maurizio";
                //  usr = "amolina"; // 
                //  usr = "pezzulo";
                //usr = Session["User"].ToString();
              //usr = "lmiguel";
              //  usr = "mmarti";
            }

            var myIDPerito = (from s in db.AGR_Periti_WEB
                              where s.Name == usr
                              select s.IDPerito).FirstOrDefault();

            var myIDOperatore = (from s in db.AGR_Periti_WEB
                              where s.Name == usr
                              select s.ID).FirstOrDefault();

            var myIDPeritoVero = (from s in db.AGR_Periti_WEB
                              where s.Name == usr
                              select s.IDVero).FirstOrDefault();

            var myLang = (from s in db.AGR_Periti_WEB
                                  where s.Name == usr
                                  select s.Lang).FirstOrDefault();

            Session["Lang"] = myLang;

            //var myIDPorto = (from s in db.AGR_Periti_WEB
            //                  where s.Name == usr
            //                  select s.IDPorto).FirstOrDefault();
            //Session["IDPortoPerito"] = myIDPorto;
            var myIDPorto = "";
            try
            {
                String test = Session["IDPortoPerito"].ToString();
                myIDPorto = Session["IDPortoPerito"].ToString();
            }
            catch
            {
                myIDPorto = (from s in db.AGR_Periti_WEB
                                 where s.Name == usr
                                 select s.IDPorto).FirstOrDefault();
                Session["IDPortoPerito"] = myIDPorto;
            }

            if (IDPorto != "")
            {
                Session["IDPortoPerito"] = IDPorto;
                myIDPorto = IDPorto;
            }

            if(myIDPorto == "TRI")
            {
                myIDPerito = "TR1 ";
                myIDPeritoVero = "PB";
            }

            if (myIDPorto == "GOA")
            {
                myIDPerito = "IWP";
                myIDPeritoVero = "IW";
            }

            if (myIDPorto == "CVV")
            {
                myIDPerito = "MDG";
                myIDPeritoVero = "MG";
            }

            if (myIDPorto == "PMI")
            {
                myIDPerito = "VR1";
                myIDPeritoVero = "Y1";
            }

            if (myIDPorto == "IBZ")
            {
                myIDPerito = "VR2";
                myIDPeritoVero = "Y2";
            }

            if (myIDPorto == "MAH")
            {
                myIDPerito = "VR3";
                myIDPeritoVero = "Y3";
            }

            if (myIDPorto == "BCN")
            {
                myIDPerito = "VR4";
                myIDPeritoVero = "Y4";
            }

            if (myIDPorto == "VLC")
            {
                myIDPerito = "VR5";
                myIDPeritoVero = "Y5";
            }

            if (myIDPorto == "NAP")
            {
                myIDPerito = "INA";
                myIDPeritoVero = "I0";
            }



            var myClasse = (from s in db.AGR_Periti_WEB
                             where s.Name == usr
                             select s.Classe).FirstOrDefault();
            Session["Classe"] = myClasse;

            var myIDRuolo = (from s in db.AGR_Periti_WEB
                            where s.Name == usr
                            select s.IDRuolo).FirstOrDefault();
            Session["IDRuolo"] = myIDRuolo;

            var datiperito = from m in db.Periti
                    where m.IDModem == myIDPerito.ToString()
                    select m;
            model.Periti = datiperito.ToList();

            // Porti - TEST TEST TEST
           
            var Porti = from m in db.AGR_Porti
                            where m.ID == "PMO" || m.ID == "TRI"
                            select m;
            model.AGR_Porti = Porti.ToList();
            var ElencoPorti = new SelectList(model.AGR_Porti.ToList(), "ID", "Descr");

            var PortiAdmin = from m in db.AGR_Porti
                        where m.ID == "PMO" || m.ID == "TRI" || m.ID == "GOA" || m.ID == "CVV" || m.ID == "PMI" || m.ID == "IBZ" || m.ID == "MAH" || m.ID == "BCN" || m.ID == "VLC" || m.ID == "NAP"
                             select m;
            model.AGR_Porti = PortiAdmin.ToList().OrderBy(s=>s.Descr);
            var ElencoPortiAdmin = new SelectList(model.AGR_Porti.ToList(), "ID", "Descr");


            // END TEST

            var datiporto = from m in db.AGR_Porti
                             where m.ID == myIDPorto.ToString()
                             select m;
            model.AGR_Porti = datiporto.ToList();


            var myPerito = from m in db.AGR_Periti_WEB
                             where m.Name == usr
                             select m;
            model.AGR_Periti_WEB = myPerito.ToList();



            // Dati per dropdown spedizioni
            DateTime ini = DateTime.Today;
            DateTime end = DateTime.Today;
            if (Filtro == "OGGI")
            {
                ini = DateTime.Today.AddDays(0);
                end = DateTime.Today.AddDays(0);
            }
            else if(Filtro == "TRE")
            {
                ini = DateTime.Today.AddDays(-3);
                end = DateTime.Today.AddDays(3);
            }
            else if (Filtro == "SETTE")
            {
                ini = DateTime.Today.AddDays(-7);
                end = DateTime.Today.AddDays(7);
            }
            else if (Filtro == "VENTUNO")
            {
                ini = DateTime.Today.AddDays(-21);
                end = DateTime.Today.AddDays(+21);
            }
             

            if (Session["Classe"].ToString() == "0")
            {
                var Spedizioni = from m in db.AGR_SpedizioniWEB_vw
                                 where m.DataInizioImbarco >= ini
                                 where m.DataInizioImbarco <= end
                                 where (m.IDPortoImbarco == myIDPorto || m.IDPortoSbarco == myIDPorto)
                                 where m.IDCliente == "51" || m.IDCliente == "GN"
                                 select m;
               // model.AGR_SpedizioniWEB_vw = Spedizioni.ToList().OrderBy(s => s.DataInizioImbarco).OrderBy(s => s.IDPortoImbarco).OrderBy(s => s.IDPortoSbarco).OrderBy(s => s.IDOriginale1);
                model.AGR_SpedizioniWEB_vw = Spedizioni.ToList().OrderBy(s => s.DataInizioImbarco).OrderBy(s => s.DataInizioImbarco);

                var ElencoSpedizioni = new SelectList(model.AGR_SpedizioniWEB_vw.ToList(), "ID", "DescrAlt");

                // Porti - TEST TEST TEST
                //if (myIDPorto == "PMO" || myIDPorto == "TRI")
                //{
                //    var Porti = from m in db.AGR_Porti
                //                where m.ID == "PMO" || m.ID == "TRI"
                //                select m;
                //    model.AGR_Porti = Porti.ToList();
                //    var ElencoPorti = new SelectList(model.AGR_Porti.ToList(), "ID", "Descr");
                //}

                // END TEST

                // Dati per dropdown Meteo
                var Meteo = from m in db.AGR_Meteo
                            where m.ID != "*"
                            where m.ID != "5"
                            where m.ID != "6"
                            where m.ID != "7"
                            select m;

                model.AGR_Meteo = Meteo.ToList();

                var ElencoMeteo = new SelectList(model.AGR_Meteo.ToList(), "ID", "DescrITA");

                // Dati per dropdown TipoPErizia
                var TP = from m in db.AGR_TipiPerizia
                         where m.ID == "C" ||
                               m.ID == "D"
                         select m;
                model.AGR_TipiPerizia = TP.ToList();
                var ElencoTP = new SelectList(model.AGR_TipiPerizia.ToList(), "ID", "DescrITA");
                Session["User"] = usr;
                Session["IDPerito"] = myIDPerito;
                Session["IDOperatore"] = myIDOperatore;
                Session["IDPeritoVero"] = myIDPeritoVero;
                Session["RTB"] = "";
               
                ViewData["ElencoSpedizioni"] = ElencoSpedizioni;
                ViewData["ElencoMeteo"] = ElencoMeteo;
                ViewData["ElencoTP"] = ElencoTP;
                ViewData["ElencoPorti"] = ElencoPorti;
                ViewData["ElencoPortiAdmin"] = ElencoPortiAdmin;
                ViewBag.IDPorto = myIDPorto;
                ViewBag.errMess = errMess;
                return View(model);
            }
            else  // GESTIONE Temporanea LIVORNO
            {
                var Spedizioni = from m in db.AGR_SpedizioniWEB_vw
                                 where m.IDCliente == "BE"
                                 where m.ID == "F2034275"
                                 //where m.DataInizioImbarco >= ini
                                 select m;
                model.AGR_SpedizioniWEB_vw = Spedizioni.ToList();

                var ElencoSpedizioni = new SelectList(model.AGR_SpedizioniWEB_vw.ToList(), "ID", "DescrAlt");

                // Dati per dropdown Meteo
                var Meteo = from m in db.AGR_Meteo
                            select m;


                model.AGR_Meteo = Meteo.ToList();

                var ElencoMeteo = new SelectList(model.AGR_Meteo.ToList(), "ID", "DescrITA");

                // Dati per dropdown TipoPErizia
                var TP = from m in db.AGR_TipiPerizia 
                         where m.ID == "Z" 
                         //where m.ID == "I" || m.ID == "N" || m.ID == "Z" || m.ID == "-" || m.ID == "+"
                         select m;
                model.AGR_TipiPerizia = TP.ToList().OrderBy(s=>s.Ordine);
                var ElencoTP = new SelectList(model.AGR_TipiPerizia.ToList(), "ID", "DescrITA");
                Session["User"] = usr;
                Session["IDPerito"] = myIDPerito;
                Session["IDOperatore"] = myIDOperatore;
                Session["IDPeritoVero"] = myIDPeritoVero;
                Session["RTB"] = "";
                ViewData["ElencoSpedizioni"] = ElencoSpedizioni;
                ViewData["ElencoMeteo"] = ElencoMeteo;
                ViewData["ElencoTP"] = ElencoTP;
                ViewBag.errMess = errMess;
                return View(model);
            }

            



        }

        public ActionResult About()
        {
            ViewBag.Message = "Autocargo - Rilevamento danni";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contatti per assistenza.";

            return View();
        }

       //public ActionResult SelezionaTelaio()
       // {
       //     return View();
       // }

       // public ActionResult VerificaTelaio(string IDPerito, string IDSpedizione, string IDMeteo, string IDTP, string aIDTrasportatore,
       //                                  string aIDTipoRotabile, string aIDModelloCasa, string myIDPerizia, string Chassis)
       // {

       //     int cnt = (from m in db.AGR_PERIZIE_TEMP_MVC
       //               where m.Telaio == Chassis
       //               select m.ID).Count();

       //     if (cnt == 0)
       //     {

       //         return RedirectToAction("DatiPerizia", new
       //         {
       //             IDPerito,
       //             IDSpedizione,
       //             IDMeteo,
       //             IDTP,
       //             aIDTrasportatore,
       //             aIDTipoRotabile,
       //             aIDModelloCasa,
       //             myIDPerizia
       //         });
       //     }
       //     else
       //     {
       //         return RedirectToAction("DatiPerizia", new
       //         {
       //             IDPerito,
       //             IDSpedizione,
       //             IDMeteo,
       //             IDTP,
       //             aIDTrasportatore,
       //             aIDTipoRotabile,
       //             aIDModelloCasa,
       //             myIDPerizia
       //         });
       //     }
       // }

       //     public ActionResult DatiPerizia( string IDPerito , string IDSpedizione, string IDMeteo , string IDTP , string aIDTrasportatore, 
       //                                  string aIDTipoRotabile, string aIDModelloCasa , string myIDPerizia)
       // {
       //     var model = new Models.HomeModel();

       //     // COME RECUPERARE CAMPI DA TABELLA/VISTA = select new{m. ecc ecc}

       //     var Casa = (from m in db.AGR_SpedizioniWEB_vw
       //                       where m.ID == IDSpedizione
       //                       select new { m.IDCliente, m.IDCasa }).FirstOrDefault();

       //     string aIDCliente = Casa.IDCliente;
       //     string aCasa = Casa.IDCasa;

       //     // Dati spedizione
       //     var Spedizioni = from m in db.AGR_SpedizioniWEB_Decoded_vw
       //                       where m.ID == IDSpedizione
       //                       select m;
       //     model.AGR_SpedizioniWEB_Decoded_vw = Spedizioni.ToList();

       //     // Dadti meteo
       //     var Meteo = from m in db.AGR_Meteo
       //                 where m.ID == IDMeteo
       //                 select m;
       //     model.AGR_Meteo = Meteo.ToList();


       //     var TP = from m in db.AGR_TipiPerizia
       //              where m.ID == IDTP
       //              select m;
       //     model.AGR_TipiPerizia = TP.ToList();

       //     // Dati per dropdown Modello
       //     var modello = from m in db.AGR_ModelliAuto
       //                   where m.IDCliente == "**"
       //                   where m.IDCasa == aCasa
       //                   select m;
       //     model.AGR_ModelliAuto = modello.ToList();
       //     var ElencoModelli = new SelectList(model.AGR_ModelliAuto.ToList(), "IDModelloCasa", "Descr");
       //     ViewData["ElencoModelli"] = ElencoModelli;
       //     ViewBag.aIDModelloCasa = aIDModelloCasa;

       //     // Dati per dropdown Trasportatore Grimaldi
       //     var TraspGrim = from m in db.AGR_TrasportatoriGrimaldi
       //                     where m.Descr.ToString().Substring(0,3) != "***"
       //                     select m;
       //     model.AGR_TrasportatoriGrimaldi = TraspGrim.ToList().OrderBy(m=>m.Descr);
       //     var ElencoTraspGrim = new SelectList(model.AGR_TrasportatoriGrimaldi.ToList(), "ID", "Descr");
       //     ViewData["ElencoTraspGrim"] = ElencoTraspGrim;
       //     ViewBag.aIDTrasportatore = aIDTrasportatore;

       //     // Dati per dropdown Tipo rotabile
       //     var TipoRotabile = from m in db.AGR_TipoRotabile
       //                        select m;
       //     model.AGR_TipoRotabile = TipoRotabile.ToList();
       //     var ElencoTipoRotabile = new SelectList(model.AGR_TipoRotabile.ToList(), "ID", "DescrITA");
       //     ViewData["ElencoTipoRotabile"] = ElencoTipoRotabile;
       //     ViewBag.aIDTipoRotabile = aIDTipoRotabile;

       //     ViewBag.IDPerito = IDPerito;
       //     ViewBag.IDSpedizione = IDSpedizione;
       //     if (String.IsNullOrEmpty(myIDPerizia))
       //     {
       //         // Mi creo un ID PErizia ...
       //         myIDPerizia = GetNewCode_AUTO(IDPerito);
       //     }
       //     else
       //     {

       //     }
       //     ViewBag.myIDPerizia = myIDPerizia;
       //     //ViewBag.IDPerito = IDPerito;
       //     //ViewBag.IDSpedizione = IDSpedizione;
       //     //ViewBag.IDMeteo = IDMeteo;
       //     //ViewBag.IDTP = IDTP;  
       //     //ViewBag.aIDTrasportatore = aIDTrasportatore;
       //     //ViewBag.aIDTipoRotabile = aIDTipoRotabile;
       //     //ViewBag.aIDModelloCasa = aIDModelloCasa;
       //     return View(model);
       // }


       // [HttpPost]
       // public ActionResult SalvaPeriziaTesta(string IDPerito, string IDSpedizione, string IDMeteo, string IDTP , string  Chassis, string DataPerizia, string IDModelloCasa, string IDTrasportatoreGrim, 
       //                                       string IDTipoRotabile, bool? isDamaged, string Condizione, string Annotazioni, string myIDPerizia)
       // {
       //     // Mi creo un ID PErizia ...
       //     //string aIDPerizia = GetNewCode_AUTO(IDPerito);

       //     // Verifico Sia tutto ok.. to do !!!!!
       //     bool isOK = CheckAll();

       //     // Inserisco dati perizia
       //     string myDataPerizia = DataPerizia.Substring(6, 4) + DataPerizia.Substring(3, 2) + DataPerizia.Substring(0, 2);
       //     string sqlcmd = " INSERT INTO AGR_PERIZIE_Temp_MVC (ID, IDSpedizione, IDPerito, IDTipoPerizia, DataPerizia, IDNazione, IDModello, Telaio, NumFoto, " +
       //                     "  Flags, IRichiesta, IDefinizione, IContab, DataModPerito, FlagControllo, IDMeteo, NumPDF,Note) " +
       //                     "VALUES (@ID, @IDSpedizione, @IDPerito, @IDTipoPerizia, @DataPerizia, @IDNazione, @IDModello, @Telaio, @NumFoto, " +
       //                     "  @Flags, @IRichiesta, @IDefinizione, @IContab, @DataModPerito, @FlagControllo, @IDMeteo, @NumPDF, @Note)";
       //     int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@ID", myIDPerizia),
       //                                                          new SqlParameter("@IDSpedizione", IDSpedizione),
       //                                                          new SqlParameter("@IDPerito", IDPerito),
       //                                                          new SqlParameter("@IDTipoPerizia", IDTP),
       //                                                          new SqlParameter("@DataPerizia", myDataPerizia),
       //                                                          new SqlParameter("@IDNazione", "***"),
       //                                                          new SqlParameter("@IDModello", IDModelloCasa),
       //                                                          new SqlParameter("@Telaio", Chassis),
       //                                                          new SqlParameter("@NumFoto", "0"),
       //                                                          new SqlParameter("@Flags", 16),
       //                                                          new SqlParameter("@IRichiesta", "0"),
       //                                                          new SqlParameter("@IDefinizione","0"),
       //                                                          new SqlParameter("@IContab", "0"),
       //                                                          new SqlParameter("@DataModPerito", DateTime.Now),
       //                                                          new SqlParameter("@FlagControllo", "0"),
       //                                                          new SqlParameter("@IDMeteo", IDMeteo),
       //                                                          new SqlParameter("@NumPDF", "0"),
       //                                                          new SqlParameter("@Note", Annotazioni));
       //     if (Inserted >0)
       //     {
       //         if (String.IsNullOrEmpty(Condizione))
       //             Condizione = "";
       //         if (String.IsNullOrEmpty(IDTipoRotabile))
       //             IDTipoRotabile = "";
       //         if (String.IsNullOrEmpty(IDTrasportatoreGrim))
       //             IDTrasportatoreGrim = "";

       //         sqlcmd = " INSERT INTO dbo.AGR_PerizieExpGrim_Temp_MVC  (ID, ID_TrasportatoreGrimaldi, ID_TipoRotabile, FlgNuovoUsato) " +
       //                   "  VALUES  (@ID, @ID_TrasportatoreGrimaldi, @ID_TipoRotabile, @FlgNuovoUsato)";
       //         Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@ID", myIDPerizia),
       //                                                          new SqlParameter("@ID_TrasportatoreGrimaldi", IDTrasportatoreGrim),
       //                                                          new SqlParameter("@ID_TipoRotabile", IDTipoRotabile),
       //                                                          new SqlParameter("@FlgNuovoUsato", Condizione));
       //     }


       //     if (isDamaged == false)
       //     {
       //         return RedirectToAction("DatiPerizia", "Home", new { IDPerito = IDPerito, IDSpedizione = IDSpedizione, IDMeteo = IDMeteo, IDTP = IDTP, aIDTrasportatore = IDTrasportatoreGrim, aIDTipoRotabile = IDTipoRotabile, aIDModelloCasa = IDModelloCasa });
       //     }
       //     else
       //     {
       //         return RedirectToAction("SalvaPeriziaDettagli", "Home", new { myIDPerizia });
       //     }
       // }



       // public ActionResult SalvaPeriziaDettagli(string aIDPerizia,string  myIDParte)
       // {
       //     var model = new Models.HomeModel();

       //     // Carichiamo UN PO' DI DATI...

       //     // Dati per dropdown AGR_Parti
       //     var parti = from m in db.WEB_AGR_Parti_vw
       //                   where m.IDCliente != "**"
       //                   where m.IDCasa != "RTB"
       //                   select m;
       //     model.WEB_AGR_Parti_vw = parti.ToList();
       //     var ElencoParti = new SelectList(model.WEB_AGR_Parti_vw.ToList().OrderBy(m=>m.DescrITA), "ID", "DescrITA");
       //     ViewData["ElencoParti"] = ElencoParti;

       //     // Dati per dropdown AGR_Parti
       //     var danni = from m in db.WEB_AGR_Danni_vw
       //                 where m.IDCliente == "**"
                        
       //                 select m;
       //     model.WEB_AGR_Danni_vw = danni.ToList();
       //     var ElencoDanni = new SelectList(model.WEB_AGR_Danni_vw.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
       //     ViewData["ElencoDanni"] = ElencoDanni;


       //     // Dati per dropdown AGR_Parti
       //     var gravita = from m in db.WEB_AGR_Gravita_vw
       //                 where m.IDCliente == "FI"

       //                 select m;
       //     model.WEB_AGR_Gravita_vw = gravita.ToList();
       //     var ElencoGravita = new SelectList(model.WEB_AGR_Gravita_vw.ToList().OrderBy(m => m.DescrITA), "ID", "DescrITA");
       //     ViewData["ElencoGravita"] = ElencoGravita;

       //     var Dettagli = from m in db.AGR_PERIZIE_DETT_TEMP_MVC_vw
       //                      where m.IDPerizia == aIDPerizia
       //                    select m;
       //     model.AGR_PERIZIE_DETT_TEMP_MVC_vw = Dettagli.ToList();
       //     ViewBag.IDPerizia = aIDPerizia;
       //     return View(model);
       // }

       // [HttpPost]
       // public ActionResult SalvaPeriziaDettagli(string aIDPerizia,  string IDParte, string IDDanno, string Qta, string Note, string Flags,
       //     string IDGravita, string IDResponsabilita, string IDAttribuzione)
       // {

       //     // Inserisco dettagli danno perizia
       //     if (String.IsNullOrEmpty(Flags))
       //         Flags = "";
       //     if (String.IsNullOrEmpty(IDResponsabilita))
       //         IDResponsabilita = "";
       //     if (String.IsNullOrEmpty(Flags))
       //         Flags = "";
       //     if (String.IsNullOrEmpty(IDAttribuzione))
       //         IDAttribuzione = "";



       //     string sqlcmd = " INSERT INTO AGR_PERIZIE_DETT_TEMP_MVC (IDPerizia, IDParte, IDDanno, Qta, Note, Flags, IDGravita, IDResponsabilita, IDAttribuzione) " +
       //                    "VALUES (@IDPerizia, @IDParte, @IDDanno, @Qta, @Note, @Flags, @IDGravita, @IDResponsabilita, @IDAttribuzione)";
       //     int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", aIDPerizia),
       //                                                          new SqlParameter("@IDParte", IDParte),
       //                                                          new SqlParameter("@IDDanno", IDDanno),
       //                                                          new SqlParameter("@Qta", Qta),
       //                                                          new SqlParameter("@Note", Note),
       //                                                          new SqlParameter("@Flags", 16),
       //                                                          new SqlParameter("@IDGravita", IDGravita),
       //                                                          new SqlParameter("@IDResponsabilita", IDResponsabilita),
       //                                                          new SqlParameter("@IDAttribuzione", IDAttribuzione));


       //     //var model = new Models.HomeModel();
       //     //var Detatgli = from m in db.AGR_PERIZIE_DETT_TEMP_MVC_vw
       //     //               where m.IDPerizia == aIDPerizia
       //     //               select m;
       //     //model.AGR_PERIZIE_DETT_TEMP_MVC_vw = Detatgli.ToList();
       //     return RedirectToAction("SalvaPeriziaDettagli", "Home", new { aIDPerizia });
       //     return RedirectToAction("SalvaPeriziaDettagli", aIDPerizia);
       //     //return View(model);
       // }


       // public ActionResult EditDettaglio(string aIDDett, string aIDPerizia)
       // {
       //     // return RedirectToAction("Edit", "Dettagli", new { ID = 28 });
           
       //     return RedirectToAction("SalvaPeriziaDettagli", "Home", new { aIDPerizia });
       // }

       // public ActionResult DeleteDettaglio(string aIDDett, string aIDPerizia)
       // {
       //     string sqlcmd = " DELETE FROM  AGR_PERIZIE_DETT_TEMP_MVC WHERE ID = @ID";
       //     int deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@ID", aIDDett));

       //     return RedirectToAction("SalvaPeriziaDettagli", "Home", new { aIDPerizia });
       // }

       // public string GetNewCode_AUTO(string aIDPerito)
       // {
       //     string aIDPerizia = "";
       //     var myIDModemPerito = (from s in db.Periti
       //                       where s.ID == aIDPerito
       //                       select s.IDModem).FirstOrDefault();

       //     DateTime ora = DateTime.Now;
       //     string now = ora.ToString("yyyyMMddhhmmss");

       //     int Totale = 0;

            

       //     //For iw = 1 To 17
       //     //    Totale = Totale + Asc(Mid(wTemp, iw, 1))
       //     //Next


       //     aIDPerizia = myIDModemPerito + now + Totale;
       //     return aIDPerizia;
       // }

       // public bool CheckAll()
       // {
       //     return true;
       // }

        
    }


}