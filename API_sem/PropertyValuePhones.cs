//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace API_sem
{
    using System;
    using System.Collections.Generic;
    
    public partial class PropertyValuePhones
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PropertyValuePhones()
        {
            this.PropertyValueToPhones = new HashSet<PropertyValueToPhones>();
        }
    
        public long ID { get; set; }
        public string Value { get; set; }
        public Nullable<long> PTid { get; set; }
    
        public virtual PropertyTypePhones PropertyTypePhones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PropertyValueToPhones> PropertyValueToPhones { get; set; }
    }
}