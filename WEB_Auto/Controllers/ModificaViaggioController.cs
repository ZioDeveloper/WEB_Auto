using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_Auto.Models;
using System.Transactions;
using System.Data.Entity;


namespace WEB_Auto.Controllers
{
    public class ModificaViaggioController : Controller
    {
        private wisedbEntities db = new wisedbEntities();
       
        // GET: ModificaViaggio
        public ActionResult ModificaViaggio(string aViaggio,string TipoMezzo = "TUTTE" , string aMsg = "")
        {
            var model = new Models.HomeModel();
            string aPerito = Session["IDPeritoVero"].ToString();
            var NumTelai = 0;
            
            if (TipoMezzo == "TUTTE")
            {
                var listaFlt = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                                //join n in db.AGR_TelaiScartati_MVC on m.Telaio equals n.Telaio into r
                                //where r.Count() == 0
                               
                                where m.IDOriginale1 == aViaggio
                                where m.IDPerito == aPerito
                                where m.IsClosed == false
                                select  m ).ToList().OrderBy(s => s.Telaio)     ;
                model.WEB_Auto_ListaPerizieXSpedizione_vw = listaFlt;
                NumTelai = listaFlt.Count();
            }
            else if (TipoMezzo == "RTB")
            {
                var listaFlt = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                                //join n in db.AGR_TelaiScartati_MVC on m.Telaio equals n.Telaio into r
                                //where r.Count() == 0
                                where m.IDOriginale1 == aViaggio
                                where m.IDPerito == aPerito
                                where m.IsClosed == false
                                where m.IDModello.ToString() == "1240" || m.IDModello.ToString() == "1241"
                             select m).ToList().OrderBy(s => s.Telaio);
                model.WEB_Auto_ListaPerizieXSpedizione_vw = listaFlt;
                NumTelai = listaFlt.Count();
            }
            if (TipoMezzo == "AUTO")
            {
                var listaFlt = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                                //join n in db.AGR_TelaiScartati_MVC on m.Telaio equals n.Telaio into r
                                //where r.Count() == 0
                                where m.IDOriginale1 == aViaggio
                                where m.IDPerito == aPerito
                                where m.IsClosed == false
                                where m.IDModello.ToString() != "1240" && m.IDModello.ToString() != "1241"
                             select m).ToList().OrderBy(s => s.Telaio);
                model.WEB_Auto_ListaPerizieXSpedizione_vw = listaFlt;
                NumTelai = listaFlt.Count();
            }

            DateTime ini = DateTime.Today;
            DateTime end = DateTime.Today;
            
            ini = DateTime.Today.AddDays(-20);
            end = DateTime.Today.AddDays(+20);
            string myIDPorto = Session["IDPortoPerito"].ToString();

            // OLD select che non forniva anche troppe spedizioni
            // ******************************************************************
            //var Spedizioni = from m in db.AGR_SpedizioniWEB_vw
            //                 where m.DataInizioImbarco >= ini
            //                 where m.DataInizioImbarco <= end
            //                 where (m.IDPortoImbarco == myIDPorto || m.IDPortoSbarco == myIDPorto)
            //                 where m.IDCliente == "51" || m.IDCliente == "GN"
            //                 select m;
            //model.AGR_SpedizioniWEB_vw = Spedizioni.ToList().OrderBy(s => s.DataInizioImbarco);

            // Nuova select che fornisce solo le spedizioni usate dal perito che vuole modificare la perizia
            // ******************************************************************

            //var Spedizioni = from m in db.AGR_SpedizioniWEB_vw
            //                 join o in db.AGR_PERIZIE_TEMP_MVC on m.ID equals o.IDSpedizione
            //                 where m.DataInizioImbarco >= ini &&
            //                       m.DataInizioImbarco <= end &&
            //                       (m.IDPortoImbarco == myIDPorto || m.IDPortoSbarco == myIDPorto) &&
            //                       (m.IDCliente == "51" || m.IDCliente == "GN") &&
            //                       o.IDPerito == aPerito
            //                 group o by new { m.IDOriginale1, m.DescrAlt, m.DataInizioImbarco } into g
            //                 orderby g.Key.DataInizioImbarco
            //                 select new
            //                 {
            //                     IDOriginale1 = g.Key.IDOriginale1,
            //                     DescrAlt = g.Key.DescrAlt,
            //                     DataInizioImbarco = g.Key.DataInizioImbarco,
            //                     altre colonne della tabella AGR_SpedizioniWEB_vw da includere
            //                 };


            //model.AGR_SpedizioniWEB_vw = Spedizioni.ToList().Select(s => new AGR_SpedizioniWEB_vw
            //{
            //    IDOriginale1 = s.IDOriginale1,
            //    DescrAlt = s.DescrAlt,
            //    DataInizioImbarco = s.DataInizioImbarco,
            //    altre colonne della tabella AGR_SpedizioniWEB_vw da includere
            //}).ToList();

            // NUova select che fornisce tutte le spedizioni usate dal perito che vuole modificare la perizia o non usate da nessuno !
            // ******************************************************************

            var Spedizioni = from m in db.AGR_SpedizioniWEB_vw
                             join o in db.AGR_PERIZIE_TEMP_MVC on m.ID equals o.IDSpedizione into g
                             from o in g.DefaultIfEmpty()
                             where m.DataInizioImbarco >= ini &&
                                   m.DataInizioImbarco <= end &&
                                   (m.IDPortoImbarco == myIDPorto || m.IDPortoSbarco == myIDPorto) &&
                                   (m.IDCliente == "51" || m.IDCliente == "GN")
                                  // && (o == null || o.IDPerito == aPerito)
                             group o by new { m.IDOriginale1, m.DescrAlt, m.DataInizioImbarco } into g
                             orderby g.Key.DataInizioImbarco
                             select new
                             {
                                 IDOriginale1 = g.Key.IDOriginale1,
                                 DescrAlt = g.Key.DescrAlt,
                                 DataInizioImbarco = g.Key.DataInizioImbarco,
                                 // altre colonne della tabella AGR_SpedizioniWEB_vw da includere
                             };

            model.AGR_SpedizioniWEB_vw = Spedizioni.ToList().Select(s => new AGR_SpedizioniWEB_vw
            {
                IDOriginale1 = s.IDOriginale1,
                DescrAlt = s.DescrAlt,
                DataInizioImbarco = s.DataInizioImbarco,
                // altre colonne della tabella AGR_SpedizioniWEB_vw da includere
            }).ToList();

            var ElencoSpedizioni = new SelectList(model.AGR_SpedizioniWEB_vw.ToList(), "IDOriginale1", "DescrAlt");


            var ElencoSelezionati = from a in db.AGR_TelaiSelezionati_MVC
                                 select a;
            model.AGR_TelaiSelezionati_MVC = ElencoSelezionati.ToList();

            var ElencoScartati = from a in db.AGR_TelaiScartati_MVC
                                    select a;
            model.AGR_TelaiScartati_MVC = ElencoScartati.ToList();

            ViewBag.aMsg = aMsg;
            ViewBag.myViaggio = aViaggio;
            ViewBag.IsOpen = true;
            ViewBag.TipoMezzo = TipoMezzo;
            if(NumTelai == 0)
                ViewBag.NumSelezionati = 0;
            else
                ViewBag.NumSelezionati = ElencoSelezionati.Count();
            ViewBag.NumTelai = NumTelai;
            ViewData["ElencoSpedizioni"] = ElencoSpedizioni;

            return View(model);

        }

        [HttpPost]
        public ActionResult ModificaViaggio(FormCollection formCollection, string NuovoViaggio, string VecchioViaggio, string TipoMezzo = "TUTTE")
        {
            bool IsCorrect = false;
            if (String.IsNullOrEmpty(NuovoViaggio))
            {
                return RedirectToAction("ModificaViaggio", new { aViaggio = VecchioViaggio });
            }

            // MI scrivo i telai selzionati...
            string[] sel = formCollection["ID"].Split(new char[] { ',' });
            foreach (string id in sel)
            {
                var perizia = id;
                string aPerizia = perizia.ToString();
                if (aPerizia != "false")
                {
                    // MI scrivo i telai selzionati...
                    string aPerito = Session["IDPeritoVero"].ToString();
                    var myPerizia = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                                     where m.ID == aPerizia
                                     select new { m.Telaio }).FirstOrDefault();
                    string sqlcmd = " INSERT INTO  AGR_TelaiSelezionati_MVC " +
                                           " (Telaio,IDPeritoVero ) VALUES( @Telaio,@IDPeritoVero) ";
                    try
                    {
                        int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@Telaio", myPerizia.Telaio.ToString()),
                                                                             new SqlParameter("@IDPeritoVero", aPerito));
                    }
                    catch { }


                }
            }

            string[] ids = formCollection["ID"].Split(new char[] { ',' });
            foreach (string id in ids)
            {
                try
                {
                    var perizia = id;
                    string aPerizia = perizia.ToString();
                    if (aPerizia != "false")
                    {
                        

                        IsCorrect = ModificaSpedizione(NuovoViaggio, aPerizia, out string aMsg);
                        if (!IsCorrect)
                        {
                            return RedirectToAction("ModificaNonConsentita", "ModificaViaggio", new { formCollection, Message = aMsg , aViaggio = VecchioViaggio, TipoMezzo });
                        }
                    }
                }
                catch { return RedirectToAction("ModificaNonConsentita", "ModificaViaggio", new { formCollection, Message = "Errore non riconoscituo contattare Astrea" , aViaggio = VecchioViaggio, TipoMezzo }); }
            }

             string myPerito = Session["IDPeritoVero"].ToString();

            string sql = " DELETE FROM  AGR_TelaiSelezionati_MVC WHERE IDPeritoVero = @IDPeritoVero "; 
            int i = db.Database.ExecuteSqlCommand(sql,new SqlParameter("@IDPeritoVero", myPerito));

            sql = " DELETE FROM  AGR_TelaiScartati_MVC WHERE IDPeritoVero = @IDPeritoVero ";
            i = db.Database.ExecuteSqlCommand(sql, new SqlParameter("@IDPeritoVero", myPerito));

            return RedirectToAction("ModificaViaggio" , new { aViaggio = VecchioViaggio, aMsg = "MODIFCA TERMINATA - VERIFICA NEL MENU LISTA VIAGGI" });
           

        }

        public bool ModificaSpedizione( string newViaggio,string IDPerizia, out string aMsg)
        {
            var myNewIDSped = "";
            
            aMsg = "";
            // Cerco la spedizione giusta per questa perizia
            var myPerizia = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw

                             where m.ID == IDPerizia
                             select new { m.ID,m.IDSpedizione, m.IDCasa,m.Telaio,m.IDModello,m.DataPerizia,m.IDTipoPerizia,m.POL,m.POD }).FirstOrDefault();

            string myIDSpedizione = myPerizia.IDSpedizione;
            string myIDCasa = myPerizia.IDCasa;

            if (myPerizia.IDModello.ToString() != "1240" && myPerizia.IDModello.ToString() != "1241")
            {
                 myNewIDSped = (from m in db.AGR_Spedizioni
                                   where m.IDOriginale1 == newViaggio
                                   where m.IDCasa == myIDCasa
                                   orderby m.DataPartenzaImbarco descending
                                select m.ID).FirstOrDefault();
            }
            else
            {
                myNewIDSped = (from m in db.AGR_Spedizioni
                                   where m.IDOriginale1 == newViaggio
                                   where m.IDCasa == "CAB" || m.IDCasa =="RTB"
                                   orderby m.DataPartenzaImbarco descending
                               select m.ID).FirstOrDefault();
            }

            if(String.IsNullOrEmpty(myNewIDSped))
            {
                aMsg = "Dati nuovo viaggio non compatibili con il precedente !";
                return false;
            }

            if (myIDSpedizione == myNewIDSped)
            {
                aMsg = "Il viaggio è lo stesso , da: " + myIDSpedizione + " a: " + myNewIDSped + " modifica annullata !";
                return false;
            }

            string myIDPortoPErito = Session["IDPortoPerito"].ToString();
            string oldTP = myPerizia.IDTipoPerizia;
            var newPOL = (from m in db.AGR_Spedizioni
                         where m.IDOriginale1 == newViaggio
                          orderby m.DataPartenzaImbarco descending
                          select m.IDPortoImbarco).FirstOrDefault();
            var newPOD = (from m in db.AGR_Spedizioni
                          where m.IDOriginale1 == newViaggio
                          orderby m.DataPartenzaImbarco descending
                          select m.IDPortoSbarco).FirstOrDefault();
            if (oldTP == "C")
            {
                if(newPOL.ToString() != myPerizia.POL)
                {
                    aMsg = "Tipo perizia  : PRELOAD - dati  non compatibili con il cambiamento assegnazione viaggio , i porti di imbarco sono differenti!";
                    // Scrivo il telaio che origina l'errore....
                    string aPerito = Session["IDPeritoVero"].ToString();
                    string sqlcmd = " INSERT INTO  AGR_TelaiScartati_MVC (Telaio, IDPeritoVero) VALUES(@Telaio, @IDPeritoVero) ";

                    try
                    {
                        int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@Telaio", myPerizia.Telaio.ToString()),
                                                                             new SqlParameter("@IDPeritoVero", aPerito));
                    }
                    catch { }
                    return false;
                }
            }
            else if(oldTP == "D")
            {
                if (newPOD.ToString() != myPerizia.POD)
                {
                    aMsg = "Tipo perizia: POST DISCHARGE, dati non compatibili con il cambiamento assegnazione viaggio , i porti di imbarco sono differenti!";
                    // Scrivo il telaio che origina l'errore....
                    string aPerito = Session["IDPeritoVero"].ToString();
                    string sqlcmd = " INSERT INTO  AGR_TelaiScartati_MVC (Telaio, IDPeritoVero) VALUES(@Telaio, @IDPeritoVero) ";

                    try
                    {
                        int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@Telaio", myPerizia.Telaio.ToString()),
                                                                             new SqlParameter("@IDPeritoVero", aPerito));
                    }
                    catch { }
                    return false;
                }
            }


            // Verifico che nn ci sia già lo stesso telaio , nello stesso viaggio, con medesimo tipo perizia
            var cnt = (from m in db.AGR_PERIZIE_TEMP_MVC
                            where m.IDSpedizione == myNewIDSped
                            where m.IDTipoPerizia == oldTP
                            where m.Telaio == myPerizia.Telaio
                            select m.ID).Count();

            if (cnt > 0)
            {
                aMsg = "Il telaio : " + myPerizia.Telaio.ToString() + " è già presente nel viaggio : " + myNewIDSped.ToString();

                // Scrivo il telaio che origina l'errore....
                string aPerito = Session["IDPeritoVero"].ToString();
                string sqlcmd = " INSERT INTO  AGR_TelaiScartati_MVC (Telaio, IDPeritoVero) VALUES(@Telaio, @IDPeritoVero) ";

                try
                {
                    int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@Telaio", myPerizia.Telaio.ToString()),
                                                                         new SqlParameter("@IDPeritoVero", aPerito));
                }
                catch { }


                return false;
            }

            var dataInizioImbarco =  (from m in db.AGR_Spedizioni
                            where m.ID == myNewIDSped
                            select new { m.DataInizioImbarco }).OrderByDescending(s=>s.DataInizioImbarco).Take(1).FirstOrDefault();

            if(myPerizia.DataPerizia.Value.Date > Convert.ToDateTime(dataInizioImbarco.DataInizioImbarco.Value.Date))
            {
                cnt = 1;
                aMsg = "Data Errata : la data partenza non può essere inferiore alla data perizia!";
                return false;

            }

            // TP
            // TODO !


            if (cnt == 0)
            {
                
                if (myPerizia.IDModello.ToString() != "1240" && myPerizia.IDModello.ToString() != "1241")
                {
                    if (!String.IsNullOrEmpty(myNewIDSped))
                    {
                        string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                                                " SET  IDSpedizione = @IDSpedizione , " +
                                                "      IDOperatore = @IDOperatore  " +
                                                " WHERE ID = @IDPerizia";
                        int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", IDPerizia),
                                                                             new SqlParameter("@IDOperatore", (int)Session["IDOperatore"]),
                                                                             new SqlParameter("@IDSpedizione", myNewIDSped));

                        string OS = Session["OS"].ToString();
                        string aPerito = Session["IDPeritoVero"].ToString();
                        sqlcmd = " INSERT INTO AGR_PERIZIE_TEMP_MVC_LOG (IDPerizia,Telaio,InsertDate,IDPerito,IDOperatore , MachineName,TipoOperazione) " +
                             " VALUES (@IDPerizia, @Telaio, @InsertDate, @IDPerito, @IDOperatore, @MachineName, @TipoOperazione)";

                        try
                        {
                            Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", myPerizia.ID.ToString()),
                                                                             new SqlParameter("@Telaio", myPerizia.Telaio.ToString()),
                                                                             new SqlParameter("@InsertDate", DateTime.Now),
                                                                             new SqlParameter("@IDPerito", aPerito),
                                                                             new SqlParameter("@IDOperatore", (int)Session["IDOperatore"]),
                                                                             new SqlParameter("@MachineName", OS),
                                                                             new SqlParameter("@TipoOperazione", "Modifica viaggio: da " + myPerizia.IDSpedizione.ToString() + " a : " + myNewIDSped));
                        }
                        catch(Exception ex)
                        {
                            string a = ex.Message;
                        }
                    }
                }
                else
                {
                    string myIDModello = "";
                    var myCASA = (from m in db.AGR_Spedizioni
                                  where m.ID == myNewIDSped
                                  select m.IDCasa).FirstOrDefault();
                    if (myCASA == "RTB")
                        myIDModello = "1241";
                    else
                        myIDModello = "1240";

                    if (!String.IsNullOrEmpty(myNewIDSped) )
                    {
                        string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                                                " SET  IDSpedizione = @IDSpedizione, " +
                                                " IDModello = @IDModello " +
                                                " WHERE ID = @IDPerizia";
                        int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", IDPerizia),
                                                                             new SqlParameter("@IDSpedizione", myNewIDSped),
                                                                             new SqlParameter("@IDModello", myIDModello));
                        string OS = Session["OS"].ToString();
                        string aPerito = Session["IDPeritoVero"].ToString();
                        sqlcmd = " INSERT INTO AGR_PERIZIE_TEMP_MVC_LOG (IDPerizia,Telaio,InsertDate,IDPerito,IDOperatore , MachineName,TipoOperazione) " +
                             " VALUES (@IDPerizia, @Telaio, @InsertDate, @IDPerito, @IDOperatore, @MachineName, @TipoOperazione)";


                        Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", myPerizia.ID.ToString()),
                                                                         new SqlParameter("@Telaio", myPerizia.Telaio.ToString()),
                                                                         new SqlParameter("@InsertDate", DateTime.Now),
                                                                         new SqlParameter("@IDPerito", aPerito),
                                                                         new SqlParameter("@IDOperatore", (int)Session["IDOperatore"]),
                                                                         new SqlParameter("@MachineName", OS),
                                                                         new SqlParameter("@TipoOperazione", "Modifica viaggio: da " + myPerizia.IDSpedizione.ToString() + " a : " + myNewIDSped));
                    }
                    else
                    {
                        aMsg = "Spedizione non identificata !";
                        return false;
                        
                    }
                }
            }
            else
            {
                //aMsg = "Errore in fase cambio spedizione, contattare EDP !";
                return false;


            }
           
            return true;

        }

        public ActionResult ModificaDataPerizia(string aViaggio, string errMess = "")
        {
            var model = new Models.HomeModel();
            var lista = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                         where m.IDOriginale1 == aViaggio

                         select m).ToList().OrderBy(s => s.DataPerizia);
            model.WEB_Auto_ListaPerizieXSpedizione_vw = lista;
            ViewBag.myViaggio = aViaggio;
            ViewBag.errMess = errMess;
            return View(model);

        }
        [HttpPost]
        public ActionResult ModificaDataPerizia(FormCollection formCollection, string NuovaDataPerizia, string Viaggio)
        {
            string aMessage = "";
            if (String.IsNullOrEmpty(NuovaDataPerizia))
            {
                return RedirectToAction("ModificaDataPerizia");
            }

            var DataxConfronto = (from m in db.AGR_Spedizioni
                                  where m.IDOriginale1 == Viaggio
                                  orderby m.DataPartenzaImbarco descending
                                  select m.DataPartenzaImbarco).FirstOrDefault();


            DateTime myDate = DateTime.ParseExact(NuovaDataPerizia, "dd/MM/yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);

            if(myDate > DataxConfronto)
            {
                aMessage = "Data errata :  non deve superare data partenza !";
                return RedirectToAction("ModificaDataPerizia", new { errMess = aMessage });
            }

            string[] ids = formCollection["ID"].Split(new char[] { ',' });
            foreach (string id in ids)
            {
                try
                {
                    var perizia = id;
                    string aPerizia = perizia.ToString();
                    if (aPerizia != "false")
                    {
                        //aPerizia = aPerizia;
                        ModificaData(NuovaDataPerizia, aPerizia);
                        aMessage = "Cambio data OK !";
                    }
                }
                catch (Exception exc){ aMessage = "Errore : " + exc.Message; }
            }
            return RedirectToAction("ModificaDataPerizia", new {errMess = aMessage });
        }

        public void ModificaData(string NuovaDataPerizia, string IDPerizia)
        {

            var myPerizia = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                             where m.ID == IDPerizia
                             select new { m.IDSpedizione, m.IDCasa }).FirstOrDefault();

            string myISoDate = NuovaDataPerizia.Right(4) + NuovaDataPerizia.Substring(3, 2) + NuovaDataPerizia.Left(2);

            //if (!String.IsNullOrEmpty(myNewIDSped))
            // {
            string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                                        " SET  DataPerizia = @DataPerizia " +
                                        " WHERE ID = @IDPerizia";
                int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", IDPerizia),
                                                                 new SqlParameter("@DataPerizia", myISoDate));



            //}

        }

        public ActionResult ModificaNonConsentita(FormCollection formCollection , string Message , string aViaggio , string TipoMezzo = "TUTTE")
        {
            ViewBag.MEssage = Message;
            ViewBag.aViaggio = aViaggio;
            ViewBag.TipoMezzo = TipoMezzo;
            return View();
        }

        public ActionResult Verifica(FormCollection formCollection, string NuovoViaggio, string VecchioViaggio)
        {
            return RedirectToAction("ModificaViaggio");
        }
    }
}