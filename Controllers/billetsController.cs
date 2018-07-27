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
    public class billetsController : Controller
    {

        private GestionEntities db = new GestionEntities();
        
        // GET: billets
        public ActionResult Index()
        {
            string c = "0114";
            if (!authentifier(c))
            {
                return RedirectToAction("Index", "Login");
            }
           

            var billet = db.billet.Include(b => b.nb_billet);
            return View(billet.ToList());
        }

        // GET: billets/Details/5
      

        // GET: billets/Create
        public ActionResult Create()
        {
            string b = "0114";
            if (!authentifier(b))
            {
                return RedirectToAction("Index", "Login");
            }

            ViewBag.Id_billet = new SelectList(db.nb_billet, "Id_billet", "id_caisse");
            return View();
        }

        // POST: billets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_billet,valeur,creer_par,modifier_par,date_creation,date_modification")] billet billet)
        {
            Session["error"] = "";
            if (ModelState.IsValid)
            {
                List<billet> b = db.billet.ToList() ;
                foreach(var item in b)
                {
                    if (billet.Id_billet==null)
                    {
                        Session["error"] = "1";
                        return RedirectToAction("Create");
                    }
                    if (item.Id_billet.Equals(billet.Id_billet))
                    {
                        Session["error"] = "1";
                        return RedirectToAction("Create");
                    }
                    else {
                        if (item.valeur.Equals(billet.valeur))
                        { Session["error"] = "2";
                            return RedirectToAction("Create");
                        }




                    }
                }
                    ViewBag.date = DateTime.Now.ToString();
                var u = Session["usr"] as utilisateur;
                ViewBag.u = u.id_utilisateur;
                billet.date_creation = ViewBag.date;
                billet.date_modification = ViewBag.date;
                billet.creer_par = u.id_utilisateur;
                billet.modifier_par = u.id_utilisateur;
                db.billet.Add(billet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_billet = new SelectList(db.nb_billet, "Id_billet", "id_caisse", billet.Id_billet);
            return View(billet);
        }


        // GET: billets/Delete/5
        public ActionResult Delete(string id)
        {
            string b = "0114";
            if (!authentifier(b))
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            billet billet = db.billet.Find(id);
            if (billet == null)
            {
                return HttpNotFound();
            }
            return View(billet);
        }

        // POST: billets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            billet billet = db.billet.Find(id);
            db.billet.Remove(billet);
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
