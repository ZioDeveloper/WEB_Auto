﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_Auto.Models;

namespace WEB_Auto.Controllers
{
    public class ModificaViaggioController : Controller
    {
        private wisedbEntities db = new wisedbEntities();
        // GET: ModificaViaggio
        public ActionResult ModificaViaggio(string aViaggio,string TipoMezzo = "TUTTE")
        {
            var model = new Models.HomeModel();
            string aPerito = Session["IDPeritoVero"].ToString();
            
            if (TipoMezzo == "TUTTE")
            {
                var listaFlt = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                                where m.IDOriginale1 == aViaggio
                                where m.IDPerito == aPerito
                                where m.IsClosed == false
                                select m).ToList().OrderBy(s => s.Telaio);
                model.WEB_Auto_ListaPerizieXSpedizione_vw = listaFlt;
            }
            else if (TipoMezzo == "RTB")
            {
                var listaFlt = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                            where m.IDOriginale1 == aViaggio
                            where m.IDPerito == aPerito
                                where m.IsClosed == false
                                where m.IDModello.ToString() == "1240" || m.IDModello.ToString() == "1241"
                             select m).ToList().OrderBy(s => s.Telaio);
                model.WEB_Auto_ListaPerizieXSpedizione_vw = listaFlt;
            }
            if (TipoMezzo == "AUTO")
            {
                var listaFlt = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                            where m.IDOriginale1 == aViaggio
                            where m.IDPerito == aPerito
                                where m.IsClosed == false
                                where m.IDModello.ToString() != "1240" && m.IDModello.ToString() != "1241"
                             select m).ToList().OrderBy(s => s.Telaio);
                model.WEB_Auto_ListaPerizieXSpedizione_vw = listaFlt;
            }


            //// var model = new Models.HomeModel();
            //var lista = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
            //             where m.IDOriginale1 == aViaggio

            //             select m).ToList().OrderBy(s => s.Telaio);
            //model.WEB_Auto_ListaPerizieXSpedizione_vw = lista;

            ViewBag.myViaggio = aViaggio;
            return View(model);

        }

        [HttpPost]
        public ActionResult ModificaViaggio(FormCollection formCollection, string NuovoViaggio, string VecchioViaggio)
        {
            bool IsCorrect = false;
            if (String.IsNullOrEmpty(NuovoViaggio))
            {
                return RedirectToAction("ModificaViaggio", new { aViaggio = VecchioViaggio });
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
                        aPerizia = aPerizia;

                        IsCorrect = ModificaSpedizione(NuovoViaggio, aPerizia);
                        if (!IsCorrect)
                            return RedirectToAction("ModificaNonConsentita", "ModificaViaggio", new { Message = "Modifica non ammessa, contattare Maurizio, perizia : " + aPerizia });
                    }
                }
                catch { }
            }
           
                return RedirectToAction("ModificaViaggio");
           

        }

        public bool ModificaSpedizione( string newViaggio,string IDPerizia)
        {
            var myNewIDSped = "";

            // Cerco la spedizione giusta per questa perizia
            var myPerizia = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                             where m.ID == IDPerizia
                             select new { m.IDSpedizione, m.IDCasa,m.Telaio,m.IDModello }).FirstOrDefault();

            string myIDSpedizione = myPerizia.IDSpedizione;
            string myIDCasa = myPerizia.IDCasa;

            if (myPerizia.IDModello.ToString() != "1240" && myPerizia.IDModello.ToString() != "1241")
            {
                 myNewIDSped = (from m in db.AGR_Spedizioni
                                   where m.IDOriginale1 == newViaggio
                                   where m.IDCasa == myIDCasa
                                   select m.ID).FirstOrDefault();
            }
            else
            {
                myNewIDSped = (from m in db.AGR_Spedizioni
                                   where m.IDOriginale1 == newViaggio
                                   where m.IDCasa == "CAB" || m.IDCasa =="RTB"
                                   select m.ID).FirstOrDefault();
            }

            // Verifico che nn ci sia già lo stesso telaio 
            var cnt = (from m in db.AGR_PERIZIE_TEMP_MVC
                            where m.IDSpedizione == myNewIDSped
                            where m.Telaio == myPerizia.Telaio
                            select m.ID).Count();

            if (cnt == 0)
            {
                
                if (myPerizia.IDModello.ToString() != "1240" && myPerizia.IDModello.ToString() != "1241")
                {
                    if (!String.IsNullOrEmpty(myNewIDSped))
                    {
                        string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                                                " SET  IDSpedizione = @IDSpedizione " +
                                                " WHERE ID = @IDPerizia";
                        int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", IDPerizia),
                                                                             new SqlParameter("@IDSpedizione", myNewIDSped));
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
                    }
                }
            }
            else
            {
                return false;

                //RedirectToAction("ModificaNonConsentita", "ModificaViaggio", new { Message = "Modifica non ammessa, contattare Maurizio." });
                /*var IDPeriziaDaCancellare = (from m in db.AGR_PERIZIE_TEMP_MVC
                                          where m.Telaio == myPerizia.Telaio
                                          where m.IDSpedizione == myNewIDSped
                                          select m.ID ).FirstOrDefault();

                string sqlcmd = " DELETE FROM  AGR_PerizieExpGrim_Temp_MVC  WHERE ID = @ID";
                int deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@ID", IDPeriziaDaCancellare));

                sqlcmd = " DELETE FROM  AGR_PERIZIE_DETT_TEMP_MVC  WHERE IDPerizia = @IDPerizia";
                deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", IDPeriziaDaCancellare));

                sqlcmd = " DELETE FROM  AGR_PERIZIE_TEMP_MVC WHERE ID = @ID";
                deleted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@ID", IDPeriziaDaCancellare));


                // Se esiste già un telaio nella nuova spedizione, lo cancello e inserisco i dati più vecchi
               
                sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                         " SET  IDSpedizione = @IDSpedizione " +
                         " WHERE ID = @IDPerizia";
                int Updated = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", IDPerizia),
                                                                    new SqlParameter("@IDSpedizione", myNewIDSped));*/



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
                        aPerizia = aPerizia;
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

        public ActionResult ModificaNonConsentita(string Message)
        {
            ViewBag.MEssage = Message;
            return View();
        }
    }
}