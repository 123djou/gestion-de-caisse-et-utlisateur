using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using gestionarretecaisse.Models;
using System.Web.Security;
using System.Diagnostics;

namespace gestionarretecaisse.Controllers
{
   
       public class LoginController : Controller
    {
        private GestionEntities db = new GestionEntities();
        
        utilisateur user;



        // GET: Login
        public ActionResult Index()
        {
            ViewBag.ok = "";
            return View();
        }
        [HttpPost]

        // GET: Login/Details/5
        
        public ActionResult Index(utilisateur us)
        {

            
                user = db.utilisateur.Where(a => a.id_utilisateur.Equals(us.id_utilisateur) && a.pass.Equals(us.pass)).FirstOrDefault();
            if(user!=null)
            {
                user = db.utilisateur.Find(us.id_utilisateur);
                Session["usr"] = user;
                Session["dem"] = db.demande.ToList() ;
                Session["send"] = null;
                Session["etatcaisse"] =db.caisse.ToList();
                Session["error"] = "";
               
                Session["demande"] = db.demande.Where(a => a.source_demande.Equals(user.id_caisse)).ToList();
                Session["list"] = db.op_profil.Where(a=>a.id_profil.Equals(user.id_profil)&&(a.operation.niveau_operation<=user.niveau_utilisateur) ).ToList();
                if (user.id_caisse != null)
                {
                    caisse cs = db.caisse.Find(user.id_caisse);

                cs.etat = "ouvert";
                    db.Entry(cs).State = EntityState.Modified;
                    db.SaveChanges();
                        }
                return View("accueil");
                     
            
            }
            else
            {
                ViewBag.erreur = "login ou mot de passe incorrecte ! ";
                return View();
            }

           
        }

        public ActionResult accueil()
        {
            if (Session["usr"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            utilisateur us = Session["usr"] as utilisateur;
            Session["demande"] = db.demande.Where(a => a.source_demande.Equals(us.id_caisse)).ToList();
            return View(user);
        }
       
        public ActionResult logout()
        {
            if (Session["usr"] != null)
            {
                Session.Clear();
                Session.Abandon();
                Session.RemoveAll();
                          }




            return View("Index");
        }


    }
}
