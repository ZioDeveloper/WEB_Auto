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
        public ActionResult ListaPerizieCancellate(bool CaricaDati = false)
        {
            var model = new Models.HomeModel();
            var ListaCancellate = (from m in db.BKP_AGR_Perizie_TEMP_MVC_ELIMINATE_vw
                                   select m).ToList().OrderByDescending(s=>s.DataPerizia).Where(s=>s.DataPerizia >= DateTime.Now.AddDays(-60));

            model.BKP_AGR_Perizie_TEMP_MVC_ELIMINATE_vw = ListaCancellate;
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