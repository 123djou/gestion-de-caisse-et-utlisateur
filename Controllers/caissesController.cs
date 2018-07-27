 using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using gestionarretecaisse.Models;
using System.Data.Entity.Core.Objects;

namespace gestionarretecaisse.Controllers
{
   /* [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]*/
    public class caissesController : Controller
    { 
      
        private GestionEntities db = new GestionEntities();
       
        // affichage de liste des caisses
        public ActionResult Index()
        {
            string op = "0112";
            if (!authentifier(op))
            { return RedirectToAction("Index","Login"); }


            var caisse = db.caisse;
            return View(caisse.ToList());
        }

        // afficher les mouvement d'un caisse
        public ActionResult Details(string id)
        {
            string op = "0220";
            if (!authentifier(op))
            { return RedirectToAction("Index", "Login"); }



            caisse caisse = db.caisse.Find(id);

            List<mouvement> mv = db.mouvement.Where(a => a.id_caisse.Equals(caisse.id_caisse)).OrderByDescending(a=>a.date_mouv).ToList();

            ViewBag.id_caisse = id;

            return View(mv);
        }

        // ajouter une nouvelle caisse
        public ActionResult Create()
        {
            string op = "0112";
            if (!authentifier(op))
            { return RedirectToAction("Index", "Login"); }


            ViewBag.id_cp = new SelectList(db.compte.Where(a => a.type_cp.Equals("interne")), "id_compte", "id_compte");
            ViewBag.id_agence = new SelectList(db.agence, "Id_agence", "Id_agence");
            return View();
        }

     
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_caisse,type_caisse,solde_veille,solde_actuel,id_cp,etat,id_utilisateur,billetage,creer_par,modifier_par,date_creation,date_modification,id_agence")] caisse caisse)
        {
            Session["error"] = "";
            
            if (ModelState.IsValid)
            {
                        caisse cs = db.caisse.Find(caisse.id_caisse);
                if (cs!= null )
                { Session["error"] = "la numéro de la caisse est deja exist !  ";
                    return RedirectToAction("Create"); }
                var c = db.caisse.Where(a => a.type_caisse.Equals("1"));
                if(c!=null&&caisse.type_caisse.Equals("1"))
                {
                    Session["error"] = "impossible de créer deux caisse principale !  ";
                    return RedirectToAction("Create");
                }
              List<caisse> cp = db.caisse.ToList();
                foreach(var i in cp)
                {

                    if(i.id_cp.Equals(caisse.id_cp))
                    {
                        Session["error"] = "impossible d'affecter deux caisses a un seul compte !";
                        return RedirectToAction("Create");
                    }
                }
                ViewBag.date = DateTime.Now.ToString();
                var u = Session["usr"] as utilisateur ;
                ViewBag.u = u.id_utilisateur;
                caisse.date_modification = ViewBag.date;
                caisse.date_creation = ViewBag.date;
                caisse.creer_par = ViewBag.u;
                caisse.modifier_par = ViewBag.u;
             
                caisse.etat = "fermer";
                db.caisse.Add(caisse);
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_cp = new SelectList(db.compte.Where(a=>a.type_cp.Equals("interne")) , "id_compte", "id_compte", caisse.id_cp);
         
            return View(caisse);
        }

        // GET: caisses/Edit/5
        public ActionResult Edit(string id)
        {
            string op = "0112";
            if (!authentifier(op))
            { return RedirectToAction("Index", "Login"); }



            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            caisse caisse = db.caisse.Find(id);
            if (caisse == null)
            {
                return HttpNotFound();
            }ViewBag.id_cp = new SelectList(db.compte.Where(a => a.type_cp.Equals("interne")), "id_compte", "id_compte");
        
            return View(caisse);
        }

        // POST: caisses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_caisse,type_caisse,solde_veille,solde_actuel,id_cp,etat,id_utilisateur,billetage,creer_par,modifier_par,date_creation,date_modification,id_agence")] caisse caisse)
        {
            if (ModelState.IsValid)
            {
               
                var u = Session["usr"] as utilisateur;
                ViewBag.u = u.id_utilisateur;
                caisse.date_modification = DateTime.Now.ToString(); ;
              caisse.modifier_par = ViewBag.u;
                
                db.Entry(caisse).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
       
            ViewBag.id_cp = new SelectList(db.compte.Where(a=>a.type_cp.Equals("interne")), "id_compte", "id_compte", caisse.id_cp );
            return View(caisse);
        }

        // GET: caisses/Delete/5
        public ActionResult Delete(string id)
        {
            string op = "0112";
            if (!authentifier(op))
            { return RedirectToAction("Index", "Login"); }
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            caisse caisse = db.caisse.Find(id);
            if (caisse == null)
            {
                return HttpNotFound();
            }
            return View(caisse);
        }

        // POST: caisses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            caisse caisse = db.caisse.Find(id);
            caisse.etat = "suspondu";
            db.Entry(caisse).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult ajustage(string id)
        {
            string op = "0110";
            if (!authentifier(op))
            {
                op = "1001";
                if (!authentifier(op))
                {
                    return RedirectToAction("Index", "Login");
                }
            }


            transaction tr = new transaction();
            utilisateur us = Session["usr"] as utilisateur;
            if(us==null)
            {
                return RedirectToAction("Index", "Login");
            }
            caisse cs = db.caisse.Find(id);
            
            Session["id"] = cs;
            caisse cc = db.caisse.Find(us.id_caisse);
            tr.op = 0;
            op_profil p = db.op_profil.Where(a => a.operation.libelle_operation.Equals("reajustage") && a.id_profil.Equals(us.id_profil)).FirstOrDefault() ;
            if (cc == null) { 
                return View(tr);
            }
            cs = cc;
            if (p == null && cs.etat.Equals("non_ajuster"))
          { 

                tr.op = 1;
            }

            return View(tr);  }
        [HttpPost]
        public ActionResult ajustage(transaction mont,string id)
        { 
            utilisateur us = Session["usr"] as utilisateur;

            caisse cs = Session["id"] as caisse;
            if (us.id_caisse != null)
            {

                cs = db.caisse.Find(us.id_caisse);
            }
           
           
            double solde = 0;

            mont.op = 0;
            solde = cs.solde_actuel;
            if(solde== mont.montant )
            {
                if(cs.id_caisse!=us.id_caisse)
                {
                    cs.etat = "reajuster";
                    Trace t = db.Trace.Where(a => a.id_caisse.Equals(cs.id_caisse)).FirstOrDefault();
                    t.ajuster_par = us.id_utilisateur;
                    db.Entry(t).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else {
                cs.etat = "ajuster";
              
                }
            }
            else { cs.etat = "non_ajuster";
                Random rend = new Random();
                if (us.id_caisse != null)
                {
                    Trace tr = new Trace();
                    tr.id_caisse = cs.id_caisse;

                    tr.Id_trace = rend.Next(1, 900000);

                    tr.montant_saisir = Convert.ToInt32(mont.montant);
                    tr.ecart = mont.montant - cs.solde_actuel;
                    tr.date_trace = DateTime.Now;
                    tr.ajuster_par = us.id_utilisateur;
                    db.Trace.Add(tr);
                    db.SaveChanges();
                }
  mont.op = 1;
            }
            

            Session["ajustage"]= cs.etat  ;
          
                db.Entry(cs).State = EntityState.Modified;
                db.SaveChanges();
       

            return View(mont); }

        
        public ActionResult verifier()
        {
            string op = "0220";
            if (!authentifier(op))
            { return RedirectToAction("Index", "Login"); }


            List<caisse> cs = db.caisse.Where(a =>!a.etat.Equals("suspondu")).ToList();

            
            return View(cs);  }
        public ActionResult annuler(Int32 id)
        {
            mouvement mv = db.mouvement.Find(id);
            caisse cs = db.caisse.Find(mv.id_caisse);
            Trace tr = db.Trace.Where(a => a.id_caisse.Equals(cs.id_caisse)).First();
            utilisateur us = Session["usr"] as utilisateur;
            if (mv.sens_mouv.Equals("D"))
            { cs.solde_actuel -= mv.montant;
                if(cs.solde_actuel==tr.montant_saisir )
                {
                    cs.etat = "ajuster";
                    tr.ajuster_par = us.id_utilisateur;
                    db.Entry(tr).State = EntityState.Modified;
                }
                db.Entry(cs).State = EntityState.Modified;

                db.SaveChanges(); }
            if (mv.sens_mouv.Equals("C"))
                    { cs.solde_actuel += mv.montant;
                if (cs.solde_actuel == tr.montant_saisir)
                {
                    cs.etat = "ajuster";
                    tr.ajuster_par = us.id_utilisateur;
                    db.Entry(tr).State = EntityState.Modified;
                }
                db.Entry(cs).State = EntityState.Modified;
                db.SaveChanges();
            }
            mv.operation = "annuler";
            db.Entry(mv).State = EntityState.Modified;
            Trace t = db.Trace.Where(a => a.id_caisse.Equals(cs.id_caisse)).First();
            if (t != null)
            {
                db.Trace.Remove(t);
                db.SaveChanges();
            }
            db.SaveChanges();
            ViewBag.msg = "operation annuler avec succes";
            return RedirectToAction("verifier");

        }
       public ActionResult reajustage()
        {
            string op = "1001";
            if (!authentifier(op))
            {return RedirectToAction("Index", "Login"); }
            

            List<Trace> tr = db.Trace.OrderByDescending(a => a.date_trace).ToList();

            
            return View(tr);
        }
       

        public ActionResult DAB( )
        {
            string op = "1023";
            if (!authentifier(op))
            {return RedirectToAction("Index", "Login"); }


            ViewBag.id_caisse = new SelectList(db.caisse.Where(a=>a.type_caisse.Equals("3")), "id_caisse", "id_caisse");

            return View();

        }
        [HttpPost]
        public ActionResult DAB(Trace tr)
        {
            ViewBag.id_caisse = new SelectList(db.caisse.Where(a => a.type_caisse.Equals("3")), "id_caisse", "id_caisse");
            Random rd = new Random();
            mouvement mv = new mouvement();
            utilisateur u = Session["usr"] as utilisateur;
            caisse cs = db.caisse.Where(a =>a.id_caisse.Equals(tr.id_caisse)).FirstOrDefault() ;
            if(tr.montant_saisir==1 )
            {
                cs.solde_veille= 0 ;
                cs.solde_actuel += tr.ecart.Value;
                compte c = db.compte.Find(cs.id_cp);
                c.solde += tr.ecart.Value;
                db.Entry(c).State = EntityState.Modified;
                caisse cc = db.caisse.Find(u.id_caisse);
                cc.solde_actuel -= tr.ecart.Value;
                if(cc.solde_actuel<0)
                {
                    ViewBag.erreur = "solde de caisse insuffisant !";
                    return View();
                }
                compte cp = db.compte.Find(cc.id_cp);
                cp.solde -= tr.ecart.Value;
                db.Entry(cc).State = EntityState.Modified;
                db.Entry(cp).State = EntityState.Modified;
                mv.date_mouv = DateTime.Now;
                mv.id_caisse = cs.id_caisse;
                mv.montant = tr.ecart.Value ;
                mv.Id_mouv = rd.Next(1000, 9000000);
                mv.operation = "chargement DAB";
                mv.sens_mouv = "D";
                mv.id_compte = c.Id_compte;
                db.mouvement.Add(mv);
                db.Entry(cs).State = EntityState.Modified;
                
                db.SaveChanges();
                ViewBag.message = "le DAB est charger avec succes";
                return View();
            }
            else
            {if(cs.solde_veille-tr.ecart.Value < 0 )
                {
                    ViewBag.error="montant non valide !";
                    return View();
                }
                cs.solde_veille -= tr.ecart.Value;
                cs.solde_actuel -= tr.ecart.Value;
                compte c = db.compte.Find(cs.id_cp);
                c.solde -= tr.ecart.Value;
                db.Entry(c).State = EntityState.Modified;
                mv.date_mouv = DateTime.Now;
                mv.id_caisse = cs.id_caisse;
                mv.montant = tr.ecart.Value;
                mv.Id_mouv = rd.Next(1000, 9000000);
                mv.operation = "dechargement DAB";
                mv.sens_mouv = "C";
                mv.id_compte = cs.id_cp;
                db.mouvement.Add(mv);
                db.Entry(cs).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.message = "le DAB est decharger avec succes";
                return View();

            }



        }

        public ActionResult arrete()
        {
            string op = "0002";
            if (!authentifier(op))
            {return RedirectToAction("Index", "Login"); }


            utilisateur us = Session["usr"] as utilisateur;
            caisse cs = db.caisse.Find(us.id_caisse);

            if((cs.etat.Equals("ajuster")||cs.etat.Equals("reajuster"))&&((cs.solde_actuel ==0)||(us.id_profil.Equals("3333"))))
            {
                cs.etat = "fermer";
               
                db.Entry(cs).State = EntityState.Modified;
                List<Trace> t = db.Trace.Where(a => a.id_caisse.Equals(cs.id_caisse)).ToList();
                if (t != null) {
                foreach (var item in t)
                {
                    db.Trace.Remove(item);
                    db.SaveChanges();

                } }
                db.SaveChanges();
                Session["usr"] = null;
               
                return RedirectToAction("logout","Login");

            }
         
            if (cs.etat.Equals("non_ajuster") || cs.etat.Equals("ouvert"))
            {
                transaction tr = new transaction();
                tr.op = 0;
                return RedirectToAction("ajustage", tr);
            }
            else
            {
                if (cs.solde_actuel != 0 && us.id_profil.Equals("4444"))
                {

                    return RedirectToAction("niveler");

                }

                else { return RedirectToAction("Index","Login"); }
            }

           
            
        }
        public ActionResult niveler()
        {
            string op = "0004 ";
            if (!authentifier(op))
            {return RedirectToAction("Index", "Login"); }




            return View();
        }
        [HttpPost]
        public ActionResult niveler(billet b)
        {
            utilisateur us = Session["usr"] as utilisateur;
            caisse cs = db.caisse.Find(us.id_caisse);
            if(cs.solde_actuel- b.valeur<0  )
            {
                ViewBag.error = "solde insuffisant !";
                return View();
            }
            cs.solde_actuel -= b.valeur;

            db.Entry(cs).State = EntityState.Modified;
            mouvement mv = new mouvement();
            Random rd = new Random();
            mv.date_mouv = DateTime.Now;
            mv.id_caisse = cs.id_caisse;
            mv.Id_mouv = rd.Next(10000, 999999);
            mv.operation = "nivellement";
            mv.sens_mouv = "C";
            mv.montant = b.valeur;
            mv.id_compte = cs.id_cp;
            db.mouvement.Add(mv);
            compte c = db.compte.Find(cs.id_cp);
            c.solde -= b.valeur ;
            db.Entry(c).State = EntityState.Modified;
            db.SaveChanges();
            Random d = new Random();
            cs = db.caisse.Where(a => a.type_caisse.Equals("1")).FirstOrDefault();
            cs.solde_actuel += b.valeur;
            db.Entry(cs).State = EntityState.Modified;
            mouvement m = new mouvement();
            m.date_mouv = DateTime.Now;
            m.id_caisse = cs.id_caisse;
            m.Id_mouv = d.Next(100, 99999);
            m.operation = "nivellement";
            m.sens_mouv = "D";
            m.montant = b.valeur;
            m.id_compte = cs.id_cp;
            db.mouvement.Add(m);
            compte cp = db.compte.Find(cs.id_cp);
            cp.solde -= b.valeur;
            db.Entry(cp).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.msg = "nivellement effectuer avec succes";
            
            return View();
        }
        public ActionResult extrait()
        {string op = "12210 ";
            if (!authentifier(op))
            {return RedirectToAction("Index", "Login"); }


            ViewBag.id_caisse= new SelectList(db.caisse , "id_caisse","id_caisse");


            return View();
        }
        [HttpPost]
        public ActionResult extrait(mouvement mv)
        {
            
            var m = new List<mouvement>();
            if (mv.id_caisse==null) { return View(); } else
            {
              var   mouv = db.mouvement.Where(a => a.id_caisse.Equals(mv.id_caisse)).OrderByDescending(a=>a.date_mouv).ToList();

                if(mv.date_mouv!=null)
                {
                   
                    foreach(var i in mouv)
                    {
                        if((i.date_mouv > mv.date_mouv.Date))
                        {
                            m.Add(i);
                        }
                    }


               
                    
            }DateTime d = Convert.ToDateTime(mv.operation);
                if ((mv.operation!=null )&&(d.Date > mv.date_mouv.Date) )
                {
                 
                    foreach(var i in m)
                    {
                        if(i.date_mouv.Date >d.Date )
                        {
                            mouv.Remove(i);
                            
                        }

                        
                    }

                    mouv = m ;
                }
                 return View("ext",mouv);
            }
          
           
        }
        public ActionResult ext(mouvement mv)
        {
            string op = "12210";
         if(!authentifier(op))
            {return RedirectToAction("Index", "Login"); }


            return View(mv);
        }
        public ActionResult fermer(int id)
        {
           Session["trace"]  = db.Trace.Find(id);
            Trace tr = db.Trace.Find(id);
            caisse cs = db.caisse.Find(tr.id_caisse);
            cs.etat = "fermer";
            db.Entry(cs).State = EntityState.Modified;

               Trace t = db.Trace.Where(a => a.id_caisse.Equals(cs.id_caisse)).First();
            Session["trace"] = t;
                    if (t != null)
                    {
                        db.Trace.Remove(t);
                        db.SaveChanges();
                    }
            db.SaveChanges();
            ViewBag.to = new SelectList(db.Direction, "email", "nom_direction");
            return RedirectToAction("sendmail","operations");
        }
        public bool authentifier(string op)
        {
            utilisateur us = Session["usr"] as utilisateur;
            if (us == null)
            {
                return (false);
            }
            else {
            
            operation o = db.operation.Find(op) ;
            op_profil of = db.op_profil.Where(a => a.id_operation.Equals(o.Id_operation) && a.id_profil.Equals(us.id_profil)).FirstOrDefault();
           if(of==null||us.niveau_utilisateur<o.niveau_operation)
            {
              return (false);
            } }
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
