using System;
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
        public ActionResult ModificaViaggio(string aViaggio)
        {
            var model = new Models.HomeModel();
            var lista = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                         where m.IDOriginale1 == aViaggio

                         select m).ToList().OrderBy(s=>s.Telaio);
            model.WEB_Auto_ListaPerizieXSpedizione_vw = lista;
            ViewBag.myViaggio = aViaggio;
            return View(model);

        }

        [HttpPost]
        public ActionResult ModificaViaggio(FormCollection formCollection, string NuovoViaggio, string VecchioViaggio)
        {
            if(String.IsNullOrEmpty(NuovoViaggio))
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
                        ModificaSpedizione(NuovoViaggio, aPerizia);
                    }
                }
                catch { }
            }
            return RedirectToAction("ModificaViaggio");
        }

        public void ModificaSpedizione( string newViaggio,string IDPerizia)
        {

            var myPerizia = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
                             where m.ID == IDPerizia
                             select new { m.IDSpedizione, m.IDCasa }).FirstOrDefault();

            string myIDSpedizione = myPerizia.IDSpedizione;
            string myIDCasa = myPerizia.IDCasa;

            var myNewIDSped = (from m in db.AGR_Spedizioni
                               where m.IDOriginale1 == newViaggio
                               where m.IDCasa == myIDCasa
                               select m.ID).FirstOrDefault();


            //var myIDNew = (from m in db.AGR_Spedizioni
            //                where m.IDOriginale1 == newViaggio
            //                select m.ID).FirstOrDefault();

            if (!String.IsNullOrEmpty(myNewIDSped))
            {
                string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                                        " SET  IDSpedizione = @IDSpedizione " +
                                        " WHERE ID = @IDPerizia";
                int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", IDPerizia),
                                                                 new SqlParameter("@IDSpedizione", myNewIDSped));
            }

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
    }
}