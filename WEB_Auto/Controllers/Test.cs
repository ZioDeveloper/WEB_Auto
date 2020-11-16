using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WEB_Auto.Models;

namespace WEB_Auto.Controllers
{
    public class Test : Controller
    {
        private wisedbEntities db = new wisedbEntities();

        // GET: Test
        public ActionResult Index()
        {
            var aGR_PERIZIE_TEMP_MVC = db.AGR_PERIZIE_TEMP_MVC.Include(a => a.AGR_PerizieExpGrim_Temp_MVC);
            return View(aGR_PERIZIE_TEMP_MVC.ToList());
        }

        // GET: Test/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AGR_PERIZIE_TEMP_MVC aGR_PERIZIE_TEMP_MVC = db.AGR_PERIZIE_TEMP_MVC.Find(id);
            if (aGR_PERIZIE_TEMP_MVC == null)
            {
                return HttpNotFound();
            }
            return View(aGR_PERIZIE_TEMP_MVC);
        }

        // GET: Test/Create
        public ActionResult Create()
        {
            ViewBag.ID = new SelectList(db.AGR_PerizieExpGrim_Temp_MVC, "ID", "Bisarca");
            return View();
        }

        // POST: Test/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,BARCODE,IDSpedizione,IDPerito,IDTipoPerizia,DataPerizia,IDNazione,IDModello,Telaio,NumFoto,IDScheda,Note,NoteConc,Flags,DRichiesta,VRichiesta,DDefinizione,VDefinizione,DContab,VContab,Stato,FileNumber,IDUtente,DataModUtente,DataModPerito,DataSpedizione,DataArrivo,FlagControllo,IRichiesta,IDefinizione,IContab,IDMeteo,IDShip,IDPortL,IDPortD,DataImbarco,DataSbarco,IDLocalitaPerizia,TotaleItems,NumPDF,IsClosed")] AGR_PERIZIE_TEMP_MVC aGR_PERIZIE_TEMP_MVC)
        {
            if (ModelState.IsValid)
            {
                db.AGR_PERIZIE_TEMP_MVC.Add(aGR_PERIZIE_TEMP_MVC);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID = new SelectList(db.AGR_PerizieExpGrim_Temp_MVC, "ID", "Bisarca", aGR_PERIZIE_TEMP_MVC.ID);
            return View(aGR_PERIZIE_TEMP_MVC);
        }

        // GET: Test/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AGR_PERIZIE_TEMP_MVC aGR_PERIZIE_TEMP_MVC = db.AGR_PERIZIE_TEMP_MVC.Find(id);
            if (aGR_PERIZIE_TEMP_MVC == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = new SelectList(db.AGR_PerizieExpGrim_Temp_MVC, "ID", "Bisarca", aGR_PERIZIE_TEMP_MVC.ID);
            return View(aGR_PERIZIE_TEMP_MVC);
        }

        // POST: Test/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,BARCODE,IDSpedizione,IDPerito,IDTipoPerizia,DataPerizia,IDNazione,IDModello,Telaio,NumFoto,IDScheda,Note,NoteConc,Flags,DRichiesta,VRichiesta,DDefinizione,VDefinizione,DContab,VContab,Stato,FileNumber,IDUtente,DataModUtente,DataModPerito,DataSpedizione,DataArrivo,FlagControllo,IRichiesta,IDefinizione,IContab,IDMeteo,IDShip,IDPortL,IDPortD,DataImbarco,DataSbarco,IDLocalitaPerizia,TotaleItems,NumPDF,IsClosed")] AGR_PERIZIE_TEMP_MVC aGR_PERIZIE_TEMP_MVC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aGR_PERIZIE_TEMP_MVC).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID = new SelectList(db.AGR_PerizieExpGrim_Temp_MVC, "ID", "Bisarca", aGR_PERIZIE_TEMP_MVC.ID);
            return View(aGR_PERIZIE_TEMP_MVC);
        }

        // GET: Test/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AGR_PERIZIE_TEMP_MVC aGR_PERIZIE_TEMP_MVC = db.AGR_PERIZIE_TEMP_MVC.Find(id);
            if (aGR_PERIZIE_TEMP_MVC == null)
            {
                return HttpNotFound();
            }
            return View(aGR_PERIZIE_TEMP_MVC);
        }

        // POST: Test/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AGR_PERIZIE_TEMP_MVC aGR_PERIZIE_TEMP_MVC = db.AGR_PERIZIE_TEMP_MVC.Find(id);
            db.AGR_PERIZIE_TEMP_MVC.Remove(aGR_PERIZIE_TEMP_MVC);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
