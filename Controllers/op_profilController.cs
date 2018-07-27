using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using gestionarretecaisse.Models;

namespace gestionarretecaisse.Controllers
{
   
    public class op_profilController : Controller
    {
        private GestionEntities db = new GestionEntities();

        // GET: op_profil
        public ActionResult Index()
        {
            string op = "2002";
            if(!authentifier(op))
            {
                return RedirectToAction("Index", "Login");
            }

            var op_profil = db.op_profil.Include(o => o.operation).Include(o => o.profil);
            return View(op_profil.OrderBy(a=>a.id_profil).ToList());
        }

        // GET: op_profil/Details/5
   
        // GET: op_profil/Create
        public ActionResult Create()
        {
            string op = "2002";
            if (!authentifier(op))
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.id_operation = new SelectList(db.operation, "Id_operation", "libelle_operation");
            ViewBag.id_profil = new SelectList(db.profil, "id_profil", "id_profil");
            return View();
        }

        // POST: op_profil/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_profil,id_operation,id_op_profil")] op_profil op_profil)
        {
            if (ModelState.IsValid)
            {
                Random rd = new Random();
             
                op_profil op = db.op_profil.Where(a => a.id_profil.Equals(op_profil.id_profil) && a.id_operation.Equals(op_profil.id_operation)).FirstOrDefault();
                if(op!=null)
                {
                    ViewBag.msg = "l'operation" + op.id_operation + " est deja afecter au profil" + op.id_profil + "";
                    ViewBag.id_operation = new SelectList(db.operation, "Id_operation", "libelle_operation");
                    ViewBag.id_profil = new SelectList(db.profil, "id_profil", "id_profil");
                    return View();
                }
                op_profil.id_op_profil  = rd.Next(1000,9000).ToString();
                db.op_profil.Add(op_profil);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_operation = new SelectList(db.operation, "Id_operation", "niveau_operation", op_profil.id_operation);
            ViewBag.id_profil = new SelectList(db.profil, "id_profil", "id_profil", op_profil.id_profil);
            return View(op_profil);
        }

        // GET: op_profil/Edit/5
       
        // GET: op_profil/Delete/5
        public ActionResult Delete(string id)
        {
            string op = "2002";
            if (!authentifier(op))
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            op_profil op_profil = db.op_profil.Find(id);
            if (op_profil == null)
            {
                return HttpNotFound();
            }
            return View(op_profil);
        }

        // POST: op_profil/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            op_profil op_profil = db.op_profil.Find(id);
            db.op_profil.Remove(op_profil);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public bool authentifier(string op)
        {
            utilisateur us = Session["usr"] as utilisateur;
            if (us == null)
            {
                return (false);
            }
            else {

                operation o = db.operation.Find(op);
                op_profil of = db.op_profil.Where(a => a.id_operation.Equals(o.Id_operation) && a.id_profil.Equals(us.id_profil)).FirstOrDefault();
                if (of == null || us.niveau_utilisateur < o.niveau_operation)
                {
                    return (false);
                }
            }
            return (true);

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
