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
    
    public partial class geo_brasil_meso
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public geo_brasil_meso()
        {
            this.geo_brasil_micro = new HashSet<geo_brasil_micro>();
        }
    
        public short id_geo_meso { get; set; }
        public string uf { get; set; }
        public string meso_regiao { get; set; }
        public string poligono { get; set; }
    
        public virtual geo_brasil_uf geo_brasil_uf { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<geo_brasil_micro> geo_brasil_micro { get; set; }
    }
}
