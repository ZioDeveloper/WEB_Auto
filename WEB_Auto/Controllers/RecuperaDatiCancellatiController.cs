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
    public class RecuperaDatiCancellatiController : Controller
    {
        private wisedbEntities db = new wisedbEntities();

        // GET: RecuperaDatiCancellati
        public ActionResult ListaPerizieCancellate()
        {
            var model = new Models.HomeModel();
            var ListaCancellate = (from m in db.BKP_AGR_Perizie_TEMP_MVC_ELIMINATE_vw
                                   select m).ToList();

            model.BKP_AGR_Perizie_TEMP_MVC_ELIMINATE_vw = ListaCancellate;
            return View(model);
        }
    }
}