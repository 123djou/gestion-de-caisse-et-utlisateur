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
    
    public partial class mouvement
    {
        public int Id_mouv { get; set; }
        public string sens_mouv { get; set; }
        public System.DateTime date_mouv { get; set; }
        public double montant { get; set; }
        public string id_caisse { get; set; }
        public string id_compte { get; set; }
        public string operation { get; set; }
    
        public virtual caisse caisse { get; set; }
    }
}
