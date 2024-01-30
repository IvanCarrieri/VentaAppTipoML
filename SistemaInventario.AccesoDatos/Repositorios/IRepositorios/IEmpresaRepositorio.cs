
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaInventario.AccesoDatos.Repositorios.IRepositorio;
using SistemaInventario.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorios.IRepositorios
{
    public interface IEmpresaRepositorio: IRepositorio<Empresa>
    {
        void Actualizar(Empresa empresa);
        
       
        
    }
}
