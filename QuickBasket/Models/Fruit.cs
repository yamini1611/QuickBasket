//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuickBasket.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Fruit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Fruit()
        {
            this.Carts = new HashSet<Cart>();
        }
    
        public int fid { get; set; }
        public string name { get; set; }
        public Nullable<int> originalcost { get; set; }
        public Nullable<int> retailprice { get; set; }
        public Nullable<int> stock { get; set; }
        public byte[] image { get; set; }
        public string offer { get; set; }
        public string category { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart> Carts { get; set; }
    }
}
