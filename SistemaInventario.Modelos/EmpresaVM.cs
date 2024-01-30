using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
    public class EmpresaVM
    {
        public Empresa Empresa { get; set; }
        public IEnumerable<SelectListItem> DepositoLista { get; set; }
    }
}
