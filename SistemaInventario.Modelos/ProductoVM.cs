using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{

    //los ViewModels es una clase con varios objetos relacionados 

    public class ProductoVM
    {
        public Producto Producto { get; set; }
        public IEnumerable<SelectListItem> CategoriasLista { get; set; }
        public IEnumerable<SelectListItem> MarcasLista { get; set; }

        public IEnumerable<SelectListItem> PadresLista { get; set; }


    }
}
