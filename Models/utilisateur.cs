//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace gestionarretecaisse.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class utilisateur
    {
        public string id_utilisateur { get; set; }
        public string nom { get; set; }
        public string prenom { get; set; }
        public string pass { get; set; }
        public string etat_utilisateur { get; set; }
        public string id_profil { get; set; }
        public int niveau_utilisateur { get; set; }
        public string id_agence { get; set; }
        public string id_caisse { get; set; }
        public string date_creation { get; set; }
        public string date_modification { get; set; }
        public string creer_par { get; set; }
        public string modifier_par { get; set; }
    
        public virtual agence agence { get; set; }
        public virtual caisse caisse { get; set; }
        public virtual profil profil { get; set; }
    }
}
