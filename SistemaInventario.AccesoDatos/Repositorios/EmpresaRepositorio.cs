
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaInventario.AccesoDatos.Repositorios.IRepositorios;
using SistemaInventario.Data;
using SistemaInventario.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorios
{
    public class EmpresaRepositorio : Repositorio<Empresa>, IEmpresaRepositorio
    {
        private ApplicationDbContext db;

        public EmpresaRepositorio(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Actualizar(Empresa empresa)
        {
            var empresaBD = db.Empresas.FirstOrDefault(b => b.Id == empresa.Id);

            if(empresaBD != null)
            {

                empresaBD.Nombre = empresa.Nombre;
                empresaBD.Descripcion = empresa.Descripcion;
                empresaBD.Pais = empresa.Pais;
                empresaBD.Ciudad = empresa.Ciudad;
                empresaBD.Direccion = empresa.Direccion;
                empresaBD.Telefono = empresa.Telefono;
                empresaBD.DepositoVentaId = empresa.DepositoVentaId;
                
                empresaBD.ActualizadoPorUsuarioId = empresa.ActualizadoPorUsuarioId;
                empresaBD.FechaActualizacion = empresa.FechaActualizacion;
                

                //aqui no van las propiedades Foreingkeys ni debo actualizar la fecha de creacion ni la propiedad de usiario creacion

                db.SaveChanges();
            }
        }

        
        
    }
}
