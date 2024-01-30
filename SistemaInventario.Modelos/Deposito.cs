using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SistemaInventario.Modelos
{
    public class Deposito
    {
        //primary key
        [Key]
        
        public int Id { get; set; }

        //En la misma pripiedad le agregamos los requerimientos
        
        [Required(ErrorMessage = "Nombre es requerido")]
        [MaxLength(60, ErrorMessage = "Nombre no debe tener mas de 60 caracteres")]
        public string Nombre { get; set; }
        
        [Required(ErrorMessage = "Descripción es requerido")]
        [MaxLength(100, ErrorMessage = "Descripcion no debe tener mas de 100 caracteres")]
        public string Descripcion { get; set; }
        
        [Required(ErrorMessage = "Estado es requerido")]
        public bool Estado { get; set; }
    }
}





