//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace gm.db.model
{
    using System;
    using System.Collections.Generic;
    
    public partial class tv_emissora
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tv_emissora()
        {
            this.geo_brasil_cidade = new HashSet<geo_brasil_cidade>();
        }
    
        public short id_emissora { get; set; }
        public short id_rede { get; set; }
        public string uf { get; set; }
        public string numero_canal { get; set; }
        public string nome_emissora { get; set; }
    
        public virtual tv_rede tv_rede { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<geo_brasil_cidade> geo_brasil_cidade { get; set; }
    }
}
