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
    
    public partial class Direction
    {
        public int Id { get; set; }
        public string nom_direction { get; set; }
        public string email { get; set; }
        public string id_agence { get; set; }
    
        public virtual agence agence { get; set; }
    }
}
