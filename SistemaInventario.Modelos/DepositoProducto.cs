﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
    public class DepositoProducto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DepositoId { get; set; }

        [ForeignKey("DepositoId")]
        public Deposito Deposito { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }

        public  int Cantidad { get; set; }

    }
}