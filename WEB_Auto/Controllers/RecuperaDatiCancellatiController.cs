using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Net;
using System.Web.Mvc;
using WEB_Auto.Models;
using System.Text.RegularExpressions;
using System.Transactions;


namespace WEB_Auto.Controllers
{
    public class RecuperaDatiCancellatiController : Controller
    {
        private wisedbEntities db = new wisedbEntities();

        // GET: RecuperaDatiCancellati
        public ActionResult ListaPerizieCancellate(string POL, string POD,string DataINI, string DataEND, bool CaricaDati = false, int FiltroData = 30)
        {
            var model = new Models.HomeModel();
            if (CaricaDati)
            {

                var Lista = (from m in db.BKP_AGR_Perizie_TEMP_MVC_ELIMINATE_vw
                                           //where m.POL ==POL
                                           //where m.POD == POD
                                       select m).ToList().OrderByDescending(s => s.DataPerizia).Where(s => s.DataPerizia >= DateTime.Now.AddDays(-FiltroData));



               

                if (!String.IsNullOrEmpty(POL))
                {
                    Lista = (from f in Lista
                             where f.POL.ToUpper() == POL.ToUpper()
                                       select f).ToList();
                }
                if (!String.IsNullOrEmpty(POD))
                {
                    Lista = (from f in Lista
                             where f.POD.ToUpper() == POD.ToUpper()
                                       select f).ToList();
                }
                model.BKP_AGR_Perizie_TEMP_MVC_ELIMINATE_vw = Lista;
            }
            else
            {
                var Lista = (from m in db.BKP_AGR_Perizie_TEMP_MVC_ELIMINATE_vw
                                           where 0==1
                                           //where m.POD == POD
                                       select m).ToList().OrderByDescending(s => s.DataPerizia).Where(s => s.DataPerizia >= DateTime.Now.AddDays(-FiltroData));
                model.BKP_AGR_Perizie_TEMP_MVC_ELIMINATE_vw = Lista;
            }

            return View(model);
        }

        public ActionResult RipristinaPerizia(string aIDPerizia)
        {
            using (wisedbEntities db = new wisedbEntities())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {

                    try
                    {

                        // Ripristina le perizie a suo tempo cancellate
                        string sqlcmd1 = " INSERT INTO AGR_PERIZIE_TEMP_MVC  " +
                            " SELECT * FROM   BKP_AGR_PERIZIE_TEMP_MVC WHERE ID = @IDPerizia";
                        int Inserted1 = db.Database.ExecuteSqlCommand(sqlcmd1, new SqlParameter("@IDPerizia", aIDPerizia));

                        string sqlcmd2 = " INSERT INTO AGR_PerizieExpGrim_Temp_MVC  " +
                                        " SELECT * FROM   BKP_AGR_PerizieExpGrim_Temp_MVC WHERE ID = @IDPerizia";
                        int Inserted2 = db.Database.ExecuteSqlCommand(sqlcmd2, new SqlParameter("@IDPerizia", aIDPerizia));

                        string sqlcmd3 = " INSERT INTO AGR_PERIZIE_DETT_TEMP_MVC   " +
                                        " SELECT IDPerizia ,IDParte ,IDDanno ,Qta,Note,Flags,IDGravita,IDResponsabilita,IDAttribuzione ,IDValuta,OreIntervento ,CostoOrarioIntervento,CostoComponenti ,CostoIntervento ,CostoVerniciatura ,TotaleItem ,NoteValoreItem ,PosLetter ,PosNumber ,PosSide FROM   BKP_AGR_PERIZIE_DETT_TEMP_MVC WHERE IDPerizia = @IDPerizia";
                        int Inserted3 = db.Database.ExecuteSqlCommand(sqlcmd3, new SqlParameter("@IDPerizia", aIDPerizia));

                        string OS = Session["OS"].ToString();
                        string aPerito = Session["IDPeritoVero"].ToString();
                        string sqlcmd4 = " INSERT INTO AGR_PERIZIE_TEMP_MVC_LOG (IDPerizia,Telaio,InsertDate,IDPerito,IDOperatore , MachineName,TipoOperazione) " +
                             " VALUES (@IDPerizia, @Telaio, @InsertDate, @IDPerito, @IDOperatore, @MachineName, @TipoOperazione)";

                        int Inserted4 = db.Database.ExecuteSqlCommand(sqlcmd4, new SqlParameter("@IDPerizia", aIDPerizia),
                                                                              new SqlParameter("@Telaio", ""),
                                                                              new SqlParameter("@InsertDate", DateTime.Now),
                                                                              new SqlParameter("@IDPerito", aPerito),
                                                                              new SqlParameter("@IDOperatore", (int)Session["IDOperatore"]),
                                                                              new SqlParameter("@MachineName", OS),
                                                                              new SqlParameter("@TipoOperazione", "Ripristino perizia cancellata precedentemente"));



                        string sqlcmd5 = " DELETE FROM BKP_AGR_PerizieExpGrim_Temp_MVC  " +
                                         "  WHERE ID = @IDPerizia";
                        int Inserted5 = db.Database.ExecuteSqlCommand(sqlcmd5, new SqlParameter("@IDPerizia", aIDPerizia));

                        string sqlcmd6 = " DELETE FROM BKP_AGR_PERIZIE_DETT_TEMP_MVC  " +
                                        "  WHERE IDPerizia = @IDPerizia";
                        int Inserted6 = db.Database.ExecuteSqlCommand(sqlcmd6, new SqlParameter("@IDPerizia", aIDPerizia));

                        string sqlcmd7 = " DELETE FROM BKP_AGR_PERIZIE_TEMP_MVC  " +
                                         "  WHERE ID = @IDPerizia";
                        int Inserted7 = db.Database.ExecuteSqlCommand(sqlcmd7, new SqlParameter("@IDPerizia", aIDPerizia));

                        transaction.Commit();
                    }
                    catch (SqlException exc)
                    {
                        string a = exc.Message;
                        transaction.Rollback();
                    }
                }
            } 

            return RedirectToAction("ListaPerizieCancellate");

        }
    }
}