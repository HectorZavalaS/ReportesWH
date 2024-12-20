using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ReportesWH.Models;

namespace ReportesWH.Controllers
{
    public class siixsem_id_materialController : Controller
    {
        private siixsem_wh_monthly_reportsEntities1 db = new siixsem_wh_monthly_reportsEntities1();

        // GET: siixsem_id_material
        public ActionResult Index()
        {
            return View(db.siixsem_id_material.ToList());
        }

        // GET: siixsem_id_material/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            siixsem_id_material siixsem_id_material = db.siixsem_id_material.Find(id);
            if (siixsem_id_material == null)
            {
                return HttpNotFound();
            }
            return View(siixsem_id_material);
        }

        // GET: siixsem_id_material/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: siixsem_id_material/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "se_id,se_date,se_id_material")] siixsem_id_material siixsem_id_material)
        {
            if (ModelState.IsValid)
            {
                db.siixsem_id_material.Add(siixsem_id_material);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(siixsem_id_material);
        }

        // GET: siixsem_id_material/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            siixsem_id_material siixsem_id_material = db.siixsem_id_material.Find(id);
            if (siixsem_id_material == null)
            {
                return HttpNotFound();
            }
            return View(siixsem_id_material);
        }

        // POST: siixsem_id_material/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "se_id,se_date,se_id_material")] siixsem_id_material siixsem_id_material)
        {
            if (ModelState.IsValid)
            {
                db.Entry(siixsem_id_material).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(siixsem_id_material);
        }

        // GET: siixsem_id_material/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            siixsem_id_material siixsem_id_material = db.siixsem_id_material.Find(id);
            if (siixsem_id_material == null)
            {
                return HttpNotFound();
            }
            return View(siixsem_id_material);
        }

        // POST: siixsem_id_material/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            siixsem_id_material siixsem_id_material = db.siixsem_id_material.Find(id);
            db.siixsem_id_material.Remove(siixsem_id_material);
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
