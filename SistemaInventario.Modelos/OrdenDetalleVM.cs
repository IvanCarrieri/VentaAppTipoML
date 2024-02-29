using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
    public class OrdenDetalleVM
    {
        public Empresa Empresa { get; set; }
        public Orden Orden { get; set; }

        public IEnumerable<OrdenDetalle> OrdenDetalleLista { get; set; }
    }
}
