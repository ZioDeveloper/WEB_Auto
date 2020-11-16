﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_Auto.Models;
namespace WEB_Auto.Controllers
{
    public class ListaPerizieController : Controller
    {
        private wisedbEntities db = new wisedbEntities();

        // GET: ListaSpedizioni
        public ActionResult ListaSpedizioni()
        {

            var model = new Models.HomeModel();
            #region Codice Commentato
            //string usr = Session["USer"].ToString(); ;

            //var myIDPerito = (from s in db.AGR_Periti_WEB
            //                  where s.Name == usr
            //                  select s.IDPerito).FirstOrDefault();

            //var myIDPorto = (from s in db.AGR_Periti_WEB
            //                 where s.Name == usr
            //                 select s.IDPorto).FirstOrDefault();

            //var datiperito = from m in db.Periti
            //                 where m.IDModem == myIDPerito.ToString()
            //                 select m;
            //model.Periti = datiperito.ToList();

            //var datiporto = from m in db.AGR_Porti
            //                where m.ID == myIDPorto.ToString()
            //                select m;
            //model.AGR_Porti = datiporto.ToList();


            //var myPerito = from m in db.AGR_Periti_WEB
            //               where m.Name == usr
            //               select m;
            //model.AGR_Periti_WEB = myPerito.ToList();

            //// Dati per dropdown spedizioni
            //DateTime ini = DateTime.Today.AddDays(-1);
            //DateTime end = DateTime.Today.AddDays(+1);
            //var Spedizioni = from m in db.AGR_SpedizioniWEB_vw
            //                 where m.DataInizioImbarco >= ini
            //                 where m.DataInizioImbarco <= end
            //                 where (m.IDPortoImbarco == myIDPorto || m.IDPortoSbarco == myIDPorto)
            //                 where m.IDCliente == "51" || m.IDCliente == "GN"
            //                 select m;
            //model.AGR_SpedizioniWEB_vw = Spedizioni.ToList().OrderBy(s => s.ID);
            #endregion
            string aPerito = Session["IDPeritoVero"].ToString();
            

            

            var lista = (from m in db.WEB_AUTO_ListaSpedizioni_vw
                         where m.IDPerito == aPerito
                         select m).ToList();
            model.WEB_AUTO_ListaSpedizioni_vw = lista;

            return View(model);

            //using (var ctx = new wisedbEntities())
            //{
            //    //this will throw an exception
            //    var myForwardings = ctx.WEB_AUTO_ListaSpedizioni_vw.SqlQuery("SELECT  P.IDSpedizione, S.IDOriginale1 AS RifCliente,0 AS NumPerizie,0 AS Good, 0 As Damaged ,  P.IsClosed, P.IDPerito " +
            //" FROM            dbo.AGR_PERIZIE_TEMP_MVC AS P WITH(NOLOCK) INNER JOIN " +
            //                                                               " dbo.AGR_Spedizioni AS S ON P.IDSpedizione = S.ID LEFT OUTER JOIN " +
            //                                                               " dbo.AGR_PERIZIE_DETT_TEMP_MVC AS D WITH(NOLOCK) ON P.ID = D.IDPerizia " +
            //                                                               " --GROUP BY P.IDSpedizione, P.IsClosed, P.IDPerito, S.IDOriginale1").ToList();
            //return View(model.WEB_Auto_Test);
            //}

        }

        public ActionResult EditSpedizione(string IDSpedizione)
        {
            var model = new Models.HomeModel();
            //var lista = (from m in db.WEB_Auto_ListaPerizieXSpedizione_vw
            //             where m.IDSpedizione == IDSpedizione
            //             select m).ToList();
            //model.WEB_Auto_ListaPerizieXSpedizione_vw = lista;
            //ViewBag.IDSpedizione = IDSpedizione;
            return View(model);
        }
    }
}