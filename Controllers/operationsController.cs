using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using gestionarretecaisse.Models;
using System.Net.Mail;

namespace gestionarretecaisse.Controllers
{
  
    public class operationsController : Controller
    {
        private GestionEntities db = new GestionEntities();

        // GET: operations
        public ActionResult Index()
        {
            string op = "2002";
            if (!authentifier(op))
            {
                return RedirectToAction("Index", "Login");
            }

            return View(db.operation.ToList());
        }

        // GET: operations/Details/5
   public ActionResult Edit(string id )
        {
            string o = "2002";
            if (!authentifier(o))
            {
                return RedirectToAction("Index", "Login");
            }

            operation op = db.operation.Find(id);
            return View(op);

        }
        [HttpPost]
        public ActionResult Edit(operation op)
        {
            db.Entry(op).State = EntityState.Modified;
            db.SaveChanges();


            return RedirectToAction("Index");

        }


        // GET: operations/Edit/5

        public ActionResult sendmail()
        {
            Trace tr = Session["trace"] as Trace;
            utilisateur us = db.utilisateur.Where(a => a.id_caisse.Equals(tr.id_caisse)).First();
            string ecart;
            if ( tr.ecart<0)
            {
                 ecart = "déficite";
            }
            else
            {
                ecart = "exident";   
            }


            ViewBag.to = new SelectList(db.Direction,"email", "nom_direction");
            sendmail sd = new sendmail();
            sd.sub = "notification écart caisse";
         
            sd.msg = "la caisse"+tr.id_caisse + "  relatif à M/Mm "+us.nom+" "+us.prenom+" présente un "+ecart+" de "+tr.ecart+"" ;

            return View(sd);
        }
        [HttpPost]
        public ActionResult sendmail(sendmail snd)
        {

            Trace tr = Session["trace"] as Trace;
            db.Trace.Remove(tr);
            db.SaveChanges();


            try
            {utilisateur us = Session["usr"] as utilisateur;
                
            SmtpClient client = new SmtpClient("smtp.gmail.com");
                client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(snd.mail,snd.pass);
                client.EnableSsl = false;
                client.Host = "localhost";
                MailMessage mailMessage = new MailMessage();
            mailMessage.From =  new MailAddress("djoujamel21@gmail.com",us.nom+us.prenom) ;
            mailMessage.To.Add(snd.to);
            mailMessage.Subject = snd.sub;
                mailMessage.IsBodyHtml = true;
            mailMessage.Body = snd.msg ;
           
                client.Send(mailMessage);
                mailMessage.Dispose();
                Session["send"] = "message envoyer avec succes";

                ViewBag.to = new SelectList(db.Direction, "email", "nom_direction");
                return View("sendmail");
            }
            catch(Exception ex)
            {
                Session["send"] = "message envoyer avec succes" ;

                ViewBag.to = new SelectList(db.Direction, "email", "nom_direction");
                return RedirectToAction("sendmail");
            }







          

            
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
