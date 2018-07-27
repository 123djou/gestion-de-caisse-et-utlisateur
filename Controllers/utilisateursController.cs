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
    
    public class utilisateursController : Controller
    {
        private GestionEntities db = new GestionEntities();

        // GET: utilisateurs
        public ActionResult Index()
        {
            string op = "0111";
            if (!authentifier(op))
            {
                return RedirectToAction("Index", "Login");
            }
           
            var utilisateur = db.utilisateur.Include(u => u.agence).Include(u => u.caisse).Include(u => u.profil);
            return View(utilisateur.ToList());
        }

        // GET: utilisateurs/Details/5
        public ActionResult Details(string id)
        {
            string op = "0111";
            if (!authentifier(op))
            {
                return RedirectToAction("Index", "Login");
            }


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            utilisateur utilisateur = db.utilisateur.Find(id);
            if (utilisateur == null)
            {
                return HttpNotFound();
            }
            return View(utilisateur);
        }

        // GET: utilisateurs/Create
        public ActionResult Create()
        {
            string op = "0111";
            if (!authentifier(op))
            {
                return RedirectToAction("Index", "Login");
            }
            Random rd = new Random();
            utilisateur us = Session["usr"] as utilisateur;
             ViewBag.prof  = us.id_profil;
            
            ViewBag.id_agence = new SelectList(db.agence, "Id_agence", "Id_agence");
            ViewBag.id_caisse = new SelectList(db.caisse.Where(a=>!a.type_caisse.Equals("3")&&!a.etat.Equals("suspondu")) , "id_caisse", "id_caisse");
            ViewBag.id_profil = new SelectList(db.profil, "id_profil", "id_profil");
            utilisateur u = new utilisateur();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            u.pass= new String(stringChars);
            return View(u);
        }

        // POST: utilisateurs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_utilisateur,nom,prenom,pass,etat_utilisateur,id_profil,niveau_utilisateur,id_agence,id_caisse,date_creation,date_modification,creer_par,modifier_par")] utilisateur utilisateur)
        {
            if (ModelState.IsValid)
            {
                Session["error"] = "";
                 List<utilisateur> us = db.utilisateur.Where(a=>!a.etat_utilisateur.Equals("suspondu")).ToList() ;
                if((utilisateur.id_profil.Equals("1111"))|| (utilisateur.id_profil.Equals("2222")))
              { utilisateur.id_caisse = null; }
                
                        foreach (var item in us )
                {
                if(item.id_utilisateur.Equals(utilisateur.id_utilisateur.ToString()) )
{
                    Session["error"] = "1";
                    return RedirectToAction("Create");
                    }
                    else {

                        if (item.id_caisse!=null&&item.id_caisse.Equals(utilisateur.id_caisse)&&!item.etat_utilisateur.Equals("suspondu"))

                        {
                            Session["error"] = "2";
                            return RedirectToAction("Create");
                        }
                }
                if(utilisateur.id_profil.Equals(item.id_profil)&&!item.etat_utilisateur.Equals("suspondu")&&((utilisateur.id_profil.Equals("1111")||utilisateur.id_profil.Equals("3333"))))
                        {
                            Session["error"] = "3";
                            return RedirectToAction("Create");
                        }
                    }
                


                ViewBag.date = DateTime.Now.ToString();
                var u = Session["usr"] as utilisateur;
                ViewBag.u = u.id_utilisateur;
                utilisateur.date_creation = ViewBag.date;
                utilisateur.date_modification = ViewBag.date;
                utilisateur.creer_par = u.id_utilisateur;
                utilisateur.modifier_par = u.id_utilisateur;
       
                utilisateur.etat_utilisateur = "actif";
                db.utilisateur.Add(utilisateur);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_agence = new SelectList(db.agence, "Id_agence", "adresse", utilisateur.id_agence);
            ViewBag.id_caisse = new SelectList(db.caisse.Where(a=>!a.type_caisse.Equals("3")) , "id_caisse", "type_caisse", utilisateur.id_caisse);
            ViewBag.id_profil = new SelectList(db.profil, "id_profil", "fonction", utilisateur.id_profil);
            return View(utilisateur);
        }

        // GET: utilisateurs/Edit/5
        public ActionResult Edit(string id)
        {
            string op = "0111";
            if (!authentifier(op))
            {
                return RedirectToAction("Index", "Login");
            }

            Session["caisse"] = db.utilisateur.Where(a=>a.id_caisse!=null ).ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            utilisateur utilisateur = db.utilisateur.Find(id);
            if (utilisateur == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_agence = new SelectList(db.agence, "Id_agence", "Id_agence", utilisateur.id_agence);
            ViewBag.id_caisse = new SelectList(db.caisse.Where(a=>!a.type_caisse.Equals("3")) , "id_caisse", "id_caisse", utilisateur.id_caisse);
            ViewBag.id_profil = new SelectList(db.profil, "id_profil", "id_profil", utilisateur.id_profil);
            return View(utilisateur);
        }

        // POST: utilisateurs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_utilisateur,nom,prenom,pass,etat_utilisateur,id_profil,niveau_utilisateur,id_agence,id_caisse,date_creation,date_modification,creer_par,modifier_par")] utilisateur utilisateur)
        { 
           
                if ((utilisateur.id_profil.Equals("1111")) || (utilisateur.id_profil.Equals("2222")))
                {
                    utilisateur.id_caisse = null;
                }
                else {
                    foreach (var item in Session["caisse"] as List<utilisateur>)
                    {
                        if (item.id_caisse.Equals(utilisateur.id_caisse)&&!item.id_utilisateur.Equals(utilisateur.id_utilisateur))
                    { 
                        Session["error"] = "2"; return RedirectToAction("Edit"); 
}

                    }
                }
                    ViewBag.date = DateTime.Now.ToString();
                var u = Session["usr"] as utilisateur;
                ViewBag.u = u.id_utilisateur;
                utilisateur.date_modification = ViewBag.date;
                utilisateur.modifier_par = u.id_utilisateur;
               
                db.Entry(utilisateur).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        
         

        // GET: utilisateurs/Delete/5
        public ActionResult Delete(string id)
        {
            string op = "0111";
            if (!authentifier(op))
            {
                return RedirectToAction("Index", "Login");
            }


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            utilisateur utilisateur = db.utilisateur.Find(id);
            if (utilisateur == null)
            {
                return HttpNotFound();
            }
            return View(utilisateur);
        }

        // POST: utilisateurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {

            
            utilisateur utilisateur = db.utilisateur.Find(id);
            utilisateur.etat_utilisateur = "suspndu";
            db.Entry(utilisateur).State = EntityState.Modified;
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
