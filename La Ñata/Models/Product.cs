namespace La_Ñata.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            ProductOrder = new HashSet<ProductOrder>();
        }

        public int id { get; set; }

        [Required(ErrorMessage = "Debes completar con una descripción")]
        [StringLength(255, ErrorMessage = "Ingresa una descripción más corta")]
        public string description { get; set; }

        [Required(ErrorMessage = "Debes ingresar un stock")]
        [Range(0, 10000, ErrorMessage = "Debes ingresar un stock entre 0 y 10.000")]
        public long stock { get; set; }

        [Required(ErrorMessage = "Debes ingresar un precio")]
        [Range(0, 10000000, ErrorMessage = "Debes ingresar un precio mayor a $0")]
        public double price { get; set; }

        [Required(ErrorMessage = "Debes ingresar un precio por rotura")]
        [Range(0, 10000000, ErrorMessage = "Debes ingresar un precio por rotura mayor a $0")]
        public double break_price { get; set; }

        public DateTime created_at { get; set; }

        public DateTime? deleted_at { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductOrder> ProductOrder { get; set; }
    }
}
