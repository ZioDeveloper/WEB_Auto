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
            var myIDNew = (from m in db.AGR_Spedizioni
                            where m.IDOriginale1 == newViaggio
                            select m.ID).FirstOrDefault();

            string sqlcmd = " UPDATE AGR_PERIZIE_Temp_MVC " +
                                    " SET  IDSpedizione = @IDSpedizione " +
                                    " WHERE ID = @IDPerizia";


            int Inserted = db.Database.ExecuteSqlCommand(sqlcmd, new SqlParameter("@IDPerizia", IDPerizia),
                                                                 new SqlParameter("@IDSpedizione", myIDNew));

        }
    }
}