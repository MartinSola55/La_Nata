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

        [Required(ErrorMessage = "Debes agregar un cliente")]
        [StringLength(500, ErrorMessage = "Debes agregar una cliente de menos de 500 caracteres")]
        [RegularExpression(@"^[a-zA-Z\u00C0-\u017F\s']+$", ErrorMessage = "Ingrese un nombre de cliente válido")]
        public string client_name { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Debes agregar un teléfono válido")]
        [StringLength(20, ErrorMessage = "Debes agregar un teléfono de menos de 20 caracteres")]
        public string phone { get; set; }

        [Required(ErrorMessage = "Debes agregar una dirección")]
        [StringLength(500, ErrorMessage = "Debes agregar una dirección de menos de 500 caracteres")]
        [RegularExpression(@"^[0-9a-zA-Z\u00C0-\u017F\s']+$", ErrorMessage = "Ingrese una dirección válida")]
        public string event_adress { get; set; }

        [Required(ErrorMessage = "Debes agregar una fecha")]
        public DateTime date { get; set; }

        [Range(0, 1000000, ErrorMessage = "Debes agregar un flete mayor a $0")]
        public double? shipment_price { get; set; }

        [StringLength(500, ErrorMessage = "Debes agregar una observación de menos de 500 caracteres")]
        [RegularExpression(@"^[0-9a-zA-Z\u00C0-\u017F\s']+$", ErrorMessage = "Ingrese una observación válida")]
        public string observation { get; set; }

        public DateTime created_at { get; set; }

        public DateTime? deleted_at { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductOrder> ProductOrder { get; set; }
    }
}
