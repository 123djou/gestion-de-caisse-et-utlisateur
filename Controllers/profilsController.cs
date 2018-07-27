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
   
    public class profilsController : Controller
    {
        private GestionEntities db = new GestionEntities();

        // GET: profils
        public ActionResult Index()
        {
            string op = "0001";
            if (!authentifier(op))
            {
                return RedirectToAction("Index", "Login");
            }
        
            return View(db.profil.ToList());
        }

        // GET: profils/Details/5
   
            public ActionResult create()
        {
            string op = "0001";
            if (!authentifier(op))
            {
                return RedirectToAction("Index", "Login");
            }


            return View();
        }
        [HttpPost]
        public ActionResult create(profil p )
        {
            var a = db.profil.Find(p.id_profil);
            if(a != null )
            { ViewBag.error = "id exist !";
                return View();
            }
            db.profil.Add(p);
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
