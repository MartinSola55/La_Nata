namespace La_Ñata.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            ProductOrder = new HashSet<ProductOrder>();
        }

        public int id { get; set; }

        [Required]
        public string client_name { get; set; }

        [StringLength(20)]
        public string phone { get; set; }

        [Required]
        public string event_adress { get; set; }

        public DateTime date { get; set; }

        public double? shipment_price { get; set; }

        public string observation { get; set; }

        public DateTime created_at { get; set; }

        public DateTime? deleted_at { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductOrder> ProductOrder { get; set; }
    }
}
