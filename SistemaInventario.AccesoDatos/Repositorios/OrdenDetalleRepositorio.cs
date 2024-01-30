
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
    public class OrdenDetalleRepositorio : Repositorio<OrdenDetalle>, IOrdenDetalleRepositorio
    {
        private ApplicationDbContext db;

        public OrdenDetalleRepositorio(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

       

        //Otra forma mas simple para actualizar:
        public void Actualizar(OrdenDetalle ordenDetalle)
        {
            db.Update(ordenDetalle);
        }




    }
}
