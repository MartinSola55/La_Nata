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
        [RegularExpression(@"^[0-9a-zA-Z\u00C0-\u017F\s']+$", ErrorMessage = "Ingrese una descripción válida")]
        [StringLength(255, ErrorMessage = "Debes agregar una descripción de menos de 255 caracteres")]
        public string description { get; set; }

        [Required(ErrorMessage = "Debes agregar un stock")]
        [Range(0, 10000, ErrorMessage = "Debes agregar un stock entre 0 y 10.000 unidades")]
        public long stock { get; set; }

        [Required(ErrorMessage = "Debes agregar un precio")]
        [Range(0, 10000000, ErrorMessage = "Debes agregar un precio mayor a $0")]
        public double price { get; set; }

        [Required(ErrorMessage = "Debes agregar un precio por rotura")]
        [Range(0, 10000000, ErrorMessage = "Debes agregar un precio por rotura mayor a $0")]
        public double break_price { get; set; }

        public DateTime created_at { get; set; }

        public DateTime? deleted_at { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductOrder> ProductOrder { get; set; }
    }
}
