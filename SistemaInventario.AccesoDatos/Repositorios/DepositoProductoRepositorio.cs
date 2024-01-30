
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
    public class DepositoProductoRepositorio : Repositorio<DepositoProducto>, IDepositoProductoRepositorio
    {
        private ApplicationDbContext db;

        public DepositoProductoRepositorio(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Actualizar(DepositoProducto Dproducto)
        {
            var DproductoBD = db.DepositoProductos.FirstOrDefault(b => b.Id == Dproducto.Id);

            if (DproductoBD != null)
            {
                DproductoBD.Cantidad = Dproducto.Cantidad;

                db.SaveChanges();
            }
        }  
         
    }
}
