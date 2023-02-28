namespace La_Ñata.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Expense")]
    public partial class Expense
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Debes agregar una fecha")]
        public DateTime date { get; set; }

        [Required(ErrorMessage = "Debes agregar una descripción")]
        [StringLength(500, ErrorMessage = "Debes agregar una descripción de menos de 500 caracteres")]
        [RegularExpression(@"^[0-9a-zA-Z\u00C0-\u017F\s']+$", ErrorMessage = "Ingrese una descripción válida")]
        public string description { get; set; }

        [Required(ErrorMessage = "Debes agregar un precio")]
        [Range(0, 1000000, ErrorMessage = "Debes agregar un precio entre $0 y $1.000.000")]
        public double price { get; set; }

        public DateTime? deleted_at { get; set; }
    }
}
