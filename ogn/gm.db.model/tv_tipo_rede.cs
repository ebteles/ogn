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
    
    public partial class tv_tipo_rede
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tv_tipo_rede()
        {
            this.tv_rede = new HashSet<tv_rede>();
        }
    
        public short id_tipo_rede { get; set; }
        public string nome_tipo_rede { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tv_rede> tv_rede { get; set; }
    }
}