namespace La_Ñata.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        public int id { get; set; }

        [Required]
        [StringLength(60)]
        public string email { get; set; }

        [Required]
        [StringLength(64)]
        public string password { get; set; }

        public int id_rol { get; set; }

        public virtual Rol Rol { get; set; }
    }
}
