using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
    public class CarritoVM
    {
        public Empresa Empresa { get; set; }
        public Producto Producto { get; set; }
        public int Stock { get; set; }
        public Carrito Carrito { get; set; }
    }
}
