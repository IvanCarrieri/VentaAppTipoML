﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
    public class Usuario: IdentityUser 
    {
        [Required(ErrorMessage ="Nombre es requerido")]
        [MaxLength(100)]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Apellidos es requerido")]
        [MaxLength(100)]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "Dirección es requerida")]
        [MaxLength(200)]
        public string Dirección { get; set; }


        [Required(ErrorMessage = "Ciudad es requerida")]
        [MaxLength(100)]
        public string Ciudad { get; set; }


        [Required(ErrorMessage = "País es requerido")]
        [MaxLength(100)]
        public string Pais { get; set; }

        [NotMapped]//no se agrega a la tabla
        public string Role { get; set; }


    }
}