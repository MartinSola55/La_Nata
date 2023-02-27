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

        public DateTime date { get; set; }

        [Required]
        public string description { get; set; }

        [Required]
        public double price { get; set; }

        public DateTime? deleted_at { get; set; }
    }
}
