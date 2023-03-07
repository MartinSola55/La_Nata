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

        [Required(ErrorMessage = "Por favor, ingrese un email")]
        [StringLength(60, ErrorMessage = "Ingresa un email de menos de 60 caracteres")]
        [EmailAddress(ErrorMessage = "Por favor, ingrese un email válido")]
        public string email { get; set; }

        [Required(ErrorMessage = "Por favor, ingrese una contraseña")]
        [StringLength(20, ErrorMessage = "Ingresa una contraseña de menos de 20 caracteres")]
        public string password { get; set; }

        public int id_rol { get; set; }

        public virtual Rol Rol { get; set; }
    }
}
