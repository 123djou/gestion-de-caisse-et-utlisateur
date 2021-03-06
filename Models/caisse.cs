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
    
    public partial class caisse
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public caisse()
        {
            this.demande = new HashSet<demande>();
            this.demande1 = new HashSet<demande>();
            this.mouvement = new HashSet<mouvement>();
            this.nb_billet = new HashSet<nb_billet>();
            this.Trace = new HashSet<Trace>();
            this.utilisateur = new HashSet<utilisateur>();
        }
    
        public string id_caisse { get; set; }
        public string type_caisse { get; set; }
        public double solde_veille { get; set; }
        public double solde_actuel { get; set; }
        public string etat { get; set; }
        public string billetage { get; set; }
        public string creer_par { get; set; }
        public string modifier_par { get; set; }
        public string date_creation { get; set; }
        public string date_modification { get; set; }
        public string id_cp { get; set; }
    
        public virtual compte compte { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<demande> demande { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<demande> demande1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<mouvement> mouvement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<nb_billet> nb_billet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Trace> Trace { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<utilisateur> utilisateur { get; set; }
    }
}
