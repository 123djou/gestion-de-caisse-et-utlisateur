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
    
    public partial class compte
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public compte()
        {
            this.caisse = new HashSet<caisse>();
        }
    
        public string Id_compte { get; set; }
        public double solde { get; set; }
        public string nom_client { get; set; }
        public string prenom_client { get; set; }
        public string type_cp { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<caisse> caisse { get; set; }
    }
}
