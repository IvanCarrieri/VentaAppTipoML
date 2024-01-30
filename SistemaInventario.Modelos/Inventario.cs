using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
    public class Inventario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        [Required]
        public DateTime FechaInicial { get; set; }
        [Required]
        public DateTime FechaFinal { get; set; }

        [Required(ErrorMessage = "El depósito es requerido")]
        public int DepositoId { get; set; }

        [ForeignKey("DepositoId")]
        public Deposito Deposito { get; set;}

        [Required]
        public bool Estado {  get; set; }

    }
}
