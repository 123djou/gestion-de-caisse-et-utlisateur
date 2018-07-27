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
   
    public class demandesController : Controller
    {
        private GestionEntities db = new GestionEntities();

        // GET: demandes
        public ActionResult Index()
        {
            string op = "5005";
            if (!authentifier(op))
            {
                return RedirectToAction("Index", "Login");
            }
            utilisateur us = Session["usr"] as utilisateur;
            Session["demande"] = db.demande.Where(a => a.source_demande.Equals(us.id_caisse)).ToList();
            var demande = db.demande.Include(d => d.caisse).Include(d => d.caisse1);
            return View(demande.Where(a=>a.distination_demande.Equals(us.id_caisse)&&(a.etat.Equals("en ettente") )).ToList());
        }

        // GET: demandes/Details/5
       
        // GET: demandes/Create
        public ActionResult Create()
        {
            string op = "0550";
            if (!authentifier(op))
            {
                return RedirectToAction("Index", "Login");
            }
            utilisateur us = Session["usr"] as utilisateur;
            Session["demande"] = db.demande.Where(a => a.source_demande.Equals(us.id_caisse)).ToList();
            ViewBag.source_demande = new SelectList(db.caisse, "id_caisse", "id_caisse");
            ViewBag.distination_demande = new SelectList(db.caisse, "id_caisse", "id_caisse");
            return View();
        }

        // POST: demandes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_demande,date_demande,montant,source_demande,distination_demande")] demande demande)
        {
       
            if (ModelState.IsValid)
            {
                ViewBag.date = DateTime.Now.ToString();
                var u = Session["usr"] as utilisateur;
                ViewBag.u = u.id_utilisateur;
                Random rnd = new Random();
             demande.Id_demande= rnd.Next(10, 10000).ToString();
                demande.date_demande = ViewBag.date;
                demande.source_demande = u.id_caisse;
                demande.etat = "en ettente";
                db.demande.Add(demande);
                db.SaveChanges();
                return RedirectToAction("voir"); 
            }

            ViewBag.source_demande = new SelectList(db.caisse, "id_caisse", "id_caisse", demande.source_demande);
            ViewBag.distination_demande = new SelectList(db.caisse, "id_caisse", "id_caisse", demande.distination_demande);
            return View(demande);
        }

        // GET: demandes/Edit/5
        public ActionResult Edit(string id)
        {
            string op = "5005";
            if (!authentifier(op))
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            demande demande = db.demande.Find(id);
            if (demande == null)
            {
                return HttpNotFound();
            }
           DateTime d = DateTime.Now;
            Random rnd = new Random();
            caisse cs = db.caisse.Find(demande.distination_demande);
            mouvement mv = new mouvement();
            cs.solde_actuel -= demande.montant;

           
            mv.Id_mouv = rnd.Next(10, 10000);
            mv.sens_mouv = "C";
            mv.montant = demande.montant;
            mv.id_compte = null ;
            mv.id_caisse = cs.id_caisse;
            mv.date_mouv = d;
            mv.operation = "nivellement";
            db.mouvement.Add(mv);
            db.Entry(cs).State = EntityState.Modified;
            db.SaveChanges();
            mouvement m = new mouvement();
            cs = db.caisse.Find(demande.source_demande);
            cs.solde_actuel += demande.montant;
            m.Id_mouv = rnd.Next(50000, 99999);
            m.sens_mouv = "D";
            m.montant = demande.montant;
            m.id_compte = null;
            m.id_caisse = cs.id_caisse;
            m.date_mouv = d;
            m.operation = "alimentation";
            db.mouvement.Add(m);
            demande.etat = "accepter";
            db.Entry(cs).State = EntityState.Modified;
            
            db.SaveChanges();
            utilisateur us = Session["usr"] as utilisateur;
            Session["demande"] = db.demande.Where(a=>a.source_demande.Equals(us.id_caisse ) ).ToList()  ;

            return RedirectToAction("Index");

        }

        // POST: demandes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
     
        // GET: demandes/Delete/5
        public ActionResult Delete(string id)
        {
            string op = "5005";
            if (!authentifier(op))
            {
                return RedirectToAction("Index", "Login");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            demande demande = db.demande.Find(id);
            if (demande == null)
            {
                return View("voir");
            }
            db.demande.Remove(demande);
            db.SaveChanges();
            return RedirectToAction("voir");
        }

        // POST: demandes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            demande demande = db.demande.Find(id);
            db.demande.Remove(demande);
            db.SaveChanges();
            return RedirectToAction("Voir");
        }
       public ActionResult voir()
        {
            {
                string op = "5005";
                if (!authentifier(op))
                {
                    return RedirectToAction("Index", "Login");
                }
                utilisateur us = Session["usr"] as utilisateur;
                Session["demande"] = db.demande.Where(a => a.source_demande.Equals(us.id_caisse)).ToList();
                return View();
            }
        }
        
       public ActionResult alimenter()
        {
            string op = "0003";
            if (!authentifier(op))
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.id_caisse = new SelectList(db.caisse.Where(a => a.etat.Equals("ouvert")&& !a.type_caisse.Equals("3")),"id_caisse","id_caisse" );
            
        
            return View();  }
        [HttpPost]
        public ActionResult alimenter(nb_billet tr)
        {  caisse cs = db.caisse.Find(tr.id_caisse);
                   
                   
            compte cp = db.compte.Find(cs.id_cp);
            cp.solde += tr.nombre.Value;
            db.Entry(cp).State = EntityState.Modified;
            mouvement mv = new mouvement();
            Random rd = new Random();
            mv.Id_mouv = rd.Next(1500, 100000);
            mv.id_caisse = cs.id_caisse;
            mv.montant = tr.nombre.Value;
           
            if (cs.type_caisse.Equals("1"))
            {
                mv.operation = "alimentaion IBS";
                cs.solde_actuel += (tr.nombre.Value * 2) ;
                db.Entry(cs).State = EntityState.Modified;
            }
            else { 
                    mv.operation = "alimentation";
                cs.solde_actuel += tr.nombre.Value;
                db.Entry(cs).State = EntityState.Modified;

            }
            mv.sens_mouv = "D";
            mv.date_mouv = DateTime.Now;
            mv.id_compte = cs.id_cp;
            db.mouvement.Add(mv);
          db.SaveChanges() ;
            utilisateur u = Session["usr"] as utilisateur;
            cs = db.caisse.Find(u.id_caisse);
            cs.solde_actuel -= tr.nombre.Value;
            db.Entry(cs).State = EntityState.Modified;
             cp = db.compte.Find(cs.id_cp);
            cp.solde -= tr.nombre.Value;
            db.Entry(cp).State = EntityState.Modified;
            mouvement m = new mouvement();
            m.id_caisse = cs.id_caisse;
          m.Id_mouv = rd.Next(1500, 100000);
            m.montant = tr.nombre.Value;
            m.operation = "alimentation";
            m.sens_mouv = "c";
            m.date_mouv = DateTime.Now;
            m.id_compte = cs.id_cp;
            db.mouvement.Add(m);
            db.SaveChanges();

            ViewBag.ok = "1";
               ViewBag.id_caisse = new SelectList(db.caisse.Where(a => !a.etat.Equals("suspondu")&& !a.etat.Equals("fermer") && !a.type_caisse.Equals("3")),"id_caisse","id_caisse");
         


            return View();
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
