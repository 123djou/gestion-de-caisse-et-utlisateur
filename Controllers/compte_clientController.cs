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
    
    public class compte_clientController : Controller
    {

    private GestionEntities db = new GestionEntities();
        compte_client cp = new compte_client();
        string id;

        public ActionResult Index()
        {
           
            if (Session["usr"] == null)
            {
               return RedirectToAction("Index", "Login");
            }
            utilisateur us = Session["usr"] as utilisateur;
            Session["demande"] = db.demande.Where(a => a.source_demande.Equals(us.id_caisse)).ToList();
            return View();

        }
        [HttpPost]
        public ActionResult Index(compte_client cpt)
        {
            Session["confirm"] = "";
            ViewBag.error = "";

            cp = db.compte_client.Where(a => a.Id_compte.Equals(cpt.Id_compte)).FirstOrDefault();
            if (cp == null)
            {
               ViewBag.confirm = "compte inexistant !";
                return View();
            }
            else {
                ViewBag.confirm = "trouve";
                id = cpt.Id_compte;
              Session["idcpt"] = cpt.Id_compte;

                return View(cp);
                
            } }
        public ActionResult Details(string id)
        {
            
            if (Session["usr"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            utilisateur us = Session["usr"] as utilisateur;
            Session["demande"] = db.demande.Where(a => a.source_demande.Equals(us.id_caisse)).ToList();
            return View(cp); }
        [HttpPost]
        public ActionResult Details() {
         
            return View("Transaction",id); }
        // GET: compte_client/Create
        public ActionResult Create()
        {
            if (Session["usr"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        // POST: compte_client/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_compte,solde,nom_client,prenom_client")] compte_client compte_client)
        {
            if (Session["usr"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                db.compte_client.Add(compte_client);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(compte_client);
        }

        // GET: compte_client/Edit/5
        public ActionResult Edit(string id)
        {
      
            if (Session["usr"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            compte_client compte_client = db.compte_client.Find(id);
            if (compte_client == null)
            {
                return HttpNotFound();
            }
            return View(compte_client);
        }

        // POST: compte_client/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_compte,solde,nom_client,prenom_client")] compte_client compte_client)
        {
            if (Session["usr"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                db.Entry(compte_client).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(compte_client);
        }

        // GET: compte_client/Delete/5
      public ActionResult Transaction()
        {
            if (Session["usr"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            
            utilisateur us = Session["usr"] as utilisateur;
            Session["demande"] = db.demande.Where(a => a.source_demande.Equals(us.id_caisse)).ToList();
            return View(); 
        }
        [HttpPost]
        public ActionResult Transaction(compte_client tr)
        {
            utilisateur us = Session["usr"] as utilisateur;
            string a = us.id_profil;
            ViewBag.prof = a;

            string c = us.id_caisse;
            caisse cs = db.caisse.Find(c);
            mouvement mv = new mouvement() ;
            
            if (tr.Id_compte.Equals("1"))
            {
                id = Session["idcpt"] as string;
                cp = db.compte_client.Find(id);
                if (cp == null)
                {
                    Session["confirm"] = "aucun id compte saisi !";
                    return RedirectToAction("Index"); }
                if (cs.solde_actuel-tr.solde<0  )
                {
                    ViewBag.error = "solde caisse insuffisant !";
                    return View("Index");
                }else { 
                if (cp.solde - tr.solde <0 )
                {
                    ViewBag.error = "solde de client insuffisant !";
                    return View("Index");
                }
                else {
                        DateTime d = DateTime.Now ;
                        Random rnd = new Random();
                     mv.Id_mouv=  rnd.Next(10,10000);
                        mv.sens_mouv = "credit";
                        mv.montant = tr.solde;
                        mv.id_compte = cp.Id_compte;
                        mv.id_caisse = cs.id_caisse;
                        mv.date_mouv = d;
                        mv.operation = "transaction client";
                            db.mouvement.Add(mv);
                         
                       
                             cp.solde = cp.solde -tr.solde;
             
                db.Entry(cp).State = EntityState.Modified;

                        cs.solde_actuel -= tr.solde;
                        db.Entry(cs).State = EntityState.Modified;
                        db.SaveChanges();


                        Session["confirm"] = "operation terminer avec succes";
                Session["idcpt"] = null;
                return RedirectToAction("Index");}

            }
            }
            else {
               
                    id = Session["idcpt"] as string;
                    cp = db.compte_client.Find(id);
                    cp.solde = cp.solde + tr.solde;
                cs.solde_actuel += tr.solde;
                DateTime d = DateTime.Now;
                Random rnd = new Random();
                mv.Id_mouv = rnd.Next(10, 10000);
                mv.sens_mouv = "debit";
                mv.montant = tr.solde;
                mv.id_compte = cp.Id_compte;
                mv.id_caisse = cs.id_caisse;
                mv.date_mouv = d;
                mv.operation = "transaction client";
                db.mouvement.Add(mv);
                cs.solde_actuel += tr.solde;
                db.Entry(cs).State = EntityState.Modified;
                db.Entry(cp).State = EntityState.Modified;
                db.SaveChanges();
                Session["confirm"] = "operation terminer avec succes";
                Session["idcpt"] = null;
                return RedirectToAction("Index");

           
            }
          
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
