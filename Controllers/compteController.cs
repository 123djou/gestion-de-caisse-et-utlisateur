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
    
    public class compteController : Controller
    {

    private GestionEntities db = new GestionEntities();
        compte cp = new compte();
        string id;
        /*recherche de compte client*/
        public ActionResult Index()
        {
            string b = "1103";


            if (!authentifier(b))
            {
               return RedirectToAction("Index", "Login");
            }
            utilisateur us = Session["usr"] as utilisateur;
            Session["demande"] = db.demande.Where(a => a.source_demande.Equals(us.id_caisse)).ToList();
            return View();

        }
        [HttpPost]
        public ActionResult Index(compte cpt)
        {
            Session["confirm"] = "";
            ViewBag.error = "";

            cp = db.compte.Where(a => a.Id_compte.Equals(cpt.Id_compte)).FirstOrDefault();
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
       
       /* realiser le transaction client retrait et versement */
      public ActionResult Transaction()
        {
            string b = "1103";


            if (!authentifier(b))
            {
                return RedirectToAction("Index", "Login");
            }

            utilisateur us = Session["usr"] as utilisateur;
            Session["demande"] = db.demande.Where(a => a.source_demande.Equals(us.id_caisse)).ToList();
            return View(); 
        }
        [HttpPost]
        public ActionResult Transaction(compte tr)
        {
            utilisateur us = Session["usr"] as utilisateur;
            string a = us.id_profil;
            ViewBag.prof = a;
            id = Session["idcpt"] as string;
            cp = db.compte.Find(id);
         
            if (cp == null)
            {
                Session["confirm"] = "aucun id compte saisi !";
                return RedirectToAction("Index");
            }
            string c = us.id_caisse;
            caisse cs = db.caisse.Find(c);
            mouvement mv = new mouvement() ;
        /* operation de retrait */
            if (tr.Id_compte.Equals("1"))
            {
               
                /* si le solde de caisse insuffisant au montant de retrait */
                if (cs.solde_actuel-tr.solde<0  )
                {
                    ViewBag.error = "solde caisse insuffisant !";
                    return View("Index");
                }else { 
                    /*si solde cliznt insuffisant au montant de retrait */
                if (cp.solde - tr.solde <0 )
                {
                    ViewBag.error = "solde de client insuffisant !";
                    return View("Index");
                }
                else {
                        DateTime d = DateTime.Now ;
                        Random rnd = new Random();
                     mv.Id_mouv=  rnd.Next(10,10000);
                        mv.sens_mouv = "C";
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
               
                    /*operation de versement */
                   
                    cp.solde = cp.solde + tr.solde;
                cs.solde_actuel += tr.solde;
                DateTime d = DateTime.Now;
                Random rnd = new Random();
                mv.Id_mouv = rnd.Next(10, 10000);
                mv.sens_mouv = "D";
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
        /*teste de droit d'acces aux operations */
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
