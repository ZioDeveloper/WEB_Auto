using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Web;
using System.Web.Mvc;
using WEB_Auto.Models;

namespace WEB_Auto.Controllers
{
    public class StoricoController : Controller
    {
        // GET: Storico
        private wisedbEntities db = new wisedbEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CercaTelaio()
        {
            return View();
            // test

        }

        public ActionResult CercaTelaioSingolo(string aTelaio,string FiltroData,string FiltroStato)
        {
            string myTelaio = aTelaio.ToUpper();
            var model = new Models.HomeModel();

            var Chassis1 = from m in db.WEB_ListaPerizieFlat_MVC_vw
                        where m.Telaio == myTelaio 
                        select m;
            model.WEB_ListaPerizieFlat_MVC_vw = Chassis1.ToList();
            ViewBag.Chassis1 = myTelaio;

            var Chassis2 = from m in db.WEB_ListaPerizieFlat_TMP_vw
                      where m.Telaio == myTelaio
                      select m;
            model.WEB_ListaPerizieFlat_TMP_vw = Chassis2.ToList();

            var Chassis3 = from m in db.WEB_ListaPerizieFlat_DEF_vw
                           where m.Telaio == myTelaio
                           select m;
            model.WEB_ListaPerizieFlat_DEF_vw = Chassis3.ToList().OrderByDescending(m=>m.DataPerizia);

            return View(model);
        }

        public ActionResult CercaStoriaTelaio(string aTelaio, int FiltroData, string FiltroStato, bool? ISSemirimorchio)
        {
            // Convertiamo il telaio in maiuscolo
            string myTelaio = aTelaio.ToUpper();

            // Impostiamo il timeout del comando del database
            db.Database.CommandTimeout = 300;

            // Creiamo il modello da passare alla vista
            var model = new Models.HomeModel();

            // Recuperiamo e ordiniamo i dati dalla vista WEB_ListaPerizieFlat_MVC_vw
            var Chassis1 = db.WEB_ListaPerizieFlat_MVC_vw
                .Where(m => m.Telaio == myTelaio && (m.IsClosed == false || m.IsClosed == null))
                .OrderByDescending(m => m.DataPerizia)
                .ToList();
            model.WEB_ListaPerizieFlat_MVC_vw = Chassis1;

            // Passiamo il telaio alla vista tramite ViewBag
            ViewBag.Chassis1 = myTelaio;

            // Recuperiamo e ordiniamo i dati dalla vista WEB_ListaPerizieFlat_TMP_vw
            var Chassis2 = db.WEB_ListaPerizieFlat_TMP_vw
                .Where(m => m.Telaio == myTelaio)
                .OrderByDescending(m => m.DataPerizia)
                .ToList();
            model.WEB_ListaPerizieFlat_TMP_vw = Chassis2;

            if (ISSemirimorchio == true)
            {
                // Recuperiamo i dati dalla vista WEB_ListaPerizieFlat_DEF_ALL_vw
                var Chassis3 = db.WEB_ListaPerizieFlat_DEF_ALL_vw
                    .Where(f => f.Telaio == myTelaio)
                    .ToList();

                // Applicare i filtri di stato
                if (FiltroStato == "DAMAGED")
                {
                    Chassis3 = Chassis3.Where(f => f.STATUS.ToString() == "DMG").ToList();
                }
                else if (FiltroStato == "GOOD")
                {
                    Chassis3 = Chassis3.Where(f => f.STATUS.ToString() == "GOOD").ToList();
                }

                // Applicare il filtro di data
                if (FiltroData != 90)
                {
                    var dataFiltro = DateTime.Now.AddDays(-FiltroData);
                    Chassis3 = Chassis3.Where(f => f.DataPerizia >= dataFiltro).ToList();
                }

                // Ordiniamo i dati filtrati e li assegnamo al modello
                model.WEB_ListaPerizieFlat_DEF_ALL_vw = Chassis3.OrderByDescending(f => f.DataPerizia).ToList();
            }
            else
            {
                // Recuperiamo i dati dalla vista WEB_ListaPerizieFlat_DEF_ALL_vw
                var Chassis3 = db.WEB_ListaPerizieFlat_DEF_ALL_vw
                    .Where(f => f.Telaio == myTelaio)
                    .ToList();

                // Applicare i filtri di stato
                if (FiltroStato == "DAMAGED")
                {
                    Chassis3 = Chassis3.Where(f => f.STATUS.ToString() == "DMG").ToList();
                }
                else if (FiltroStato == "GOOD")
                {
                    Chassis3 = Chassis3.Where(f => f.STATUS.ToString() == "GOOD").ToList();
                }

                // Applicare il filtro di data
                if (FiltroData != 90)
                {
                    var dataFiltro = DateTime.Now.AddDays(-FiltroData);
                    Chassis3 = Chassis3.Where(f => f.DataPerizia >= dataFiltro).ToList();
                }

                // Ordiniamo i dati filtrati e li assegnamo al modello
                model.WEB_ListaPerizieFlat_DEF_ALL_vw = Chassis3.OrderByDescending(f => f.DataPerizia).ToList();
            }

            // Passiamo i filtri alla vista tramite ViewBag
            ViewBag.FiltroData = FiltroData;
            ViewBag.FiltroStato = FiltroStato;

            // Ritorniamo il risultato alla vista
            return View(model);
        }


        public ActionResult CercaStoriaTelaioOLD(string aTelaio,  int FiltroData, string FiltroStato)
        {
            string myTelaio = aTelaio.ToUpper();

            db.Database.CommandTimeout = 300;
            var model = new Models.HomeModel();

            var Chassis1 = from m in db.WEB_ListaPerizieFlat_MVC_vw
                           where m.Telaio == myTelaio
                           where m.IsClosed == false
                           //where m.IDTipoPerizia == "C"
                           select m;
            model.WEB_ListaPerizieFlat_MVC_vw = Chassis1.ToList().OrderByDescending(s=>s.DataPerizia);
            ViewBag.Chassis1 = myTelaio;


            var Chassis2 = from m in db.WEB_ListaPerizieFlat_TMP_vw
                           where m.Telaio == myTelaio
                           select m;
            model.WEB_ListaPerizieFlat_TMP_vw = Chassis2.ToList().OrderByDescending(s => s.DataPerizia);

            //Filtriamo...

            //var Chassis3 = from m in db.WEB_ListaPerizieFlat_DEF_vw
            //               where m.Telaio == myTelaio
            //               select m;

            //model.WEB_ListaPerizieFlat_DEF_vw = Chassis3.ToList().OrderByDescending(m => m.DataPerizia);

            var Chassis3 = (from f in db.WEB_ListaPerizieFlat_DEF_ALL_vw
                            where f.Telaio == myTelaio
                            select f).ToList();

            if(FiltroStato == "DAMAGED")
            {
                Chassis3 = (from f in Chassis3
                            where f.STATUS.ToString() == "DMG"
                         select f).ToList();
            }

            if (FiltroStato == "GOOD")
            {
                Chassis3 = (from f in Chassis3
                            where f.STATUS.ToString() == "GOOD"
                            select f).ToList();
            }

            if (FiltroData != 90)
            {
                Chassis3 = (from f in Chassis3
                            select f).Where(p => p.DataPerizia >= DateTime.Now.AddDays(-FiltroData)).ToList();
            }


            model.WEB_ListaPerizieFlat_DEF_ALL_vw = Chassis3.ToList().OrderByDescending(s => s.DataPerizia);

            ViewBag.FiltroData = FiltroData;
            ViewBag.FiltroStato = FiltroStato;


            return View(model);
        }

        public ActionResult VisualizzaPreload(string aViaggio, int FiltroData = 0, string FiltroStato = "", string FiltroTelaio = "")
        {
            var model = new Models.HomeModel();

            if (!String.IsNullOrEmpty(aViaggio))
            {
                // Inizio dati su palmare
               /* var L1 = (from m in db.WEB_ListaPerizieFlat_MVC_vw
                         where m.Viaggio == aViaggio
                         where m.IDTipoPerizia == "C"
                         where m.IsClosed == false
                         select m).ToList();*/
                var L1 = (from m in db.WEB_ListaPerizieFlat_MVC_vw
                         
                          where m.IDTipoPerizia == "C"
                          where m.IsClosed == false
                          select m).Where(s => s.Viaggio.ToUpper().Contains(aViaggio)).ToList();


                if (FiltroStato == "DAMAGED")
                {
                    L1 = (from m in L1
                         where m.Status.ToString().ToUpper() == "DMG"
                         select m).ToList();
                }

                if (FiltroStato == "GOOD")
                {
                    L1 = (from m in L1
                          where m.Status.ToString().ToUpper() == "GOOD"
                          select m).ToList();
                }

                if (FiltroTelaio != "")
                {
                    L1 = (from m in L1
                          where m.Telaio.ToUpper() == FiltroTelaio.ToUpper()
                          select m).ToList();
                }

                if (FiltroData != 90)
                {
                    L1 = (from f in L1
                          select f).Where(p => p.DataPerizia >= DateTime.Now.AddDays(-FiltroData)).ToList();
                }

                model.WEB_ListaPerizieFlat_MVC_vw = L1.ToList().OrderBy(s => s.Telaio);

                // Inizio dati su DEFINITIVO

                /*var L2 = (from m in db.WEB_ListaPerizieFlat_DEF_ALL_vw
                         where m.Viaggio == aViaggio
                         where m.IDTipoPerizia == "C"
                         select m).ToList();*/

                var L2 = (from m in db.WEB_ListaPerizieFlat_DEF_ALL_vw
                          where m.IDTipoPerizia == "C"
                          select m).Where(s=>s.Viaggio.ToUpper().Contains(aViaggio)).ToList();

                if (FiltroStato == "DAMAGED")
                {
                    L2 = (from m in L2
                          where m.STATUS.ToString().ToUpper() == "DMG"
                          select m).ToList();
                }

                if (FiltroStato == "GOOD")
                {
                    L2 = (from m in L2
                          where m.STATUS.ToString().ToUpper() == "GOOD"
                          select m).ToList();
                }

                if (FiltroTelaio != "")
                {
                    L2 = (from m in L2
                          where m.Telaio.ToString().ToUpper() == FiltroTelaio.ToUpper()
                          select m).ToList();
                }

                if (FiltroData != 90)
                {
                    L2 = (from f in L2
                          select f).Where(p => p.DataPerizia >= DateTime.Now.AddDays(-FiltroData)).ToList();
                }

                model.WEB_ListaPerizieFlat_DEF_ALL_vw = L2.ToList().OrderBy(s => s.Telaio);


                /*var L3 = (from m in db.WEB_ListaPerizieFlat_TMP_vw
                         where m.Viaggio == aViaggio
                         where m.IDTipoPerizia == "C"
                         select m).ToList();*/

                var L3 = (from m in db.WEB_ListaPerizieFlat_TMP_vw
                          where m.IDTipoPerizia == "C"
                          select m).Where(s => s.Viaggio.ToUpper().Contains(aViaggio)).ToList();

                if (FiltroStato == "DAMAGED")
                {
                    L3 = (from m in L3
                          where m.Status.ToString().ToUpper() == "DMG"
                          select m).ToList();
                }

                if (FiltroStato == "GOOD")
                {
                    L3 = (from m in L3
                          where m.Status.ToString().ToUpper() == "GOOD"
                          select m).ToList();
                }

                if (FiltroTelaio != "")
                {
                    L3 = (from m in L3
                          where m.Telaio.ToString().ToUpper() == FiltroTelaio.ToUpper()
                          select m).ToList();
                }

                if (FiltroData != 90)
                {
                    L3 = (from f in L3
                          select f).Where(p => p.DataPerizia >= DateTime.Now.AddDays(-FiltroData)).ToList();
                }

                model.WEB_ListaPerizieFlat_TMP_vw = L3.ToList().OrderBy(s => s.Telaio);
            }
            else
            {
                var L1 = from m in db.WEB_ListaPerizieFlat_MVC_vw
                         where 0==1
                         where m.Viaggio == aViaggio
                         where m.IDTipoPerizia == "C"
                         select m;
                model.WEB_ListaPerizieFlat_MVC_vw = L1.ToList().OrderBy(s => s.Status);

                var L2 = from m in db.WEB_ListaPerizieFlat_DEF_ALL_vw
                         where 0 == 1
                         where m.Viaggio == aViaggio
                         where m.IDTipoPerizia == "C"
                         select m;
                model.WEB_ListaPerizieFlat_DEF_ALL_vw = L2.ToList().OrderBy(s => s.STATUS);

                var L3 = from m in db.WEB_ListaPerizieFlat_TMP_vw
                         where 0==1
                         where m.Viaggio == aViaggio
                         where m.IDTipoPerizia == "C"
                         select m;
                model.WEB_ListaPerizieFlat_TMP_vw = L3.ToList().OrderBy(s => s.Status);

            }

            ViewBag.Viaggio = aViaggio;
            return View(model);
        }

        public ActionResult CarouselFotoStoriche(string aIDPerizia, string aTelaio, int FiltroData = 0, string FiltroStato = "")
        {
            


            var model = new Models.HomeModel();

            var foto = (from m in db.WEB_ListaPerizieFlat_DEF_ALL_vw
                        where m.IDPerizia == aIDPerizia
                        select m).ToList();
            model.WEB_ListaPerizieFlat_DEF_ALL_vw = foto;
            ViewBag.NumFoto = foto[0].NumFoto;
            ViewBag.IDPErizia = foto[0].IDPerizia;
            ViewBag.Telaio = aTelaio;
            ViewBag.FiltroData = FiltroData;
            ViewBag.FiltroStato = FiltroStato;
            return View(model);
        }

        public ActionResult CarouselFotoTemporaneo(string aIDPerizia, string aTelaio, int? FiltroData , string FiltroStato)
        {
            // Cerca le foto perizie precedenti ancora da elaborare


            var model = new Models.HomeModel();

            var foto = (from m in db.WEB_AUTO_FOTO
                        where m.IDPerizia == aIDPerizia
                        select m).ToList();
            model.WEB_AUTO_FOTO = foto;

            var foto1 = (from m in db.WEB_ListaPerizieFlat_TMP_vw
                        where m.IDPerizia == aIDPerizia
                        where m.Telaio == aTelaio
                        select m).ToList();
            model.WEB_ListaPerizieFlat_TMP_vw = foto1;
            //ViewBag.NumFoto = foto[0].NumFoto;
            ViewBag.IDPErizia = foto[0].IDPerizia;
            ViewBag.Telaio = aTelaio;
            ViewBag.FiltroData = FiltroData;
            ViewBag.FiltroStato = FiltroStato;
            return View(model);
        }
    }
}