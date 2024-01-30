using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
    public class Kardex
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public  int DepositoProductoId { get; set; }
        [ForeignKey("DepositoProductoId")]
        public DepositoProducto DepositoProducto { get; set;}

        [Required]
        [MaxLength(100)]
        public string Tipo {  get; set; } // De entrada o salida

        [Required]
        public string Detalle { get; set; }

        [Required]
        public int StockAnterior { get; set; }
        [Required]
        public int Cantidad { get; set; }
        [Required]
        public double Costo { get; set; }
        [Required]
        public int Stock {  get; set; }
        [Required]
        public double Total { get; set; }

        [Required]
        public string UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public  Usuario Usuario { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }
    }
}
