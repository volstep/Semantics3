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
    
    public partial class PropertyValueToLaptops
    {
        public long ID { get; set; }
        public Nullable<long> LapId { get; set; }
        public Nullable<long> PVId { get; set; }
    
        public virtual Laptops Laptops { get; set; }
        public virtual PropertyValue PropertyValue { get; set; }
    }
}
