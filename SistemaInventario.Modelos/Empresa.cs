using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
    public class Empresa
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nombre es requerido")]
        [MaxLength(80)]
        public  string Nombre { get; set; }

        [Required(ErrorMessage = "Descripcioón es requerido")]
        [MaxLength(200)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "País es requerido")]
        [MaxLength (100)]
        public string Pais { get; set; }

        [Required(ErrorMessage = "Ciudad es requerida")]
        [MaxLength(100)]
        public string Ciudad { get; set; }

        [Required(ErrorMessage = "Dirección es requerida")]
        [MaxLength(100)]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Teléfono es requerido")]
        [MaxLength(80)]
        public string Telefono { get; set; }



        [Required(ErrorMessage = "Deposito de venta es requerida")]
        public int DepositoVentaId { get; set; }

        [ForeignKey("DepositoVentaId")]
        public Deposito Deposito { get; set; }



        public string CreadoPorUsuarioId { get; set; }

        [ForeignKey("CreadoPorUsuarioId")]
        public Usuario CreadoPor { get; set; }



        public string ActualizadoPorUsuarioId { get; set; }

        [ForeignKey("ActualizadoPorUsuarioId")]
        public Usuario ActualizadoPor { get; set; }


        public DateTime FechaCreacion { get; set; }

        public DateTime FechaActualizacion { get; set; }

    }
}
