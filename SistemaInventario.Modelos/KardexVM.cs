using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
    public class KardexVM
    {
        public Producto Producto { get; set; }

        public IEnumerable<Kardex> KardexLista { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }

    }
}
