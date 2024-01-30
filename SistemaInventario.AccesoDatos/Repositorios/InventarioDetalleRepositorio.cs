
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
    public class InventarioDetalleRepositorio : Repositorio<InventarioDetalle>, IInventarioDetalleRepositorio
    {
        private ApplicationDbContext db;

        public InventarioDetalleRepositorio(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

       

        public void Actualizar(InventarioDetalle inventarioDetalle)
        {
            var inventarioDetalleDB = db.InventarioDetalles.FirstOrDefault(i=> i.Id == inventarioDetalle.Id);

            if (inventarioDetalleDB != null)
            {
               
                inventarioDetalleDB.StockAnterior = inventarioDetalleDB.StockAnterior;
                inventarioDetalleDB.Cantidad = inventarioDetalle.Cantidad;


                db.SaveChanges();
            }
        }
    }
}
