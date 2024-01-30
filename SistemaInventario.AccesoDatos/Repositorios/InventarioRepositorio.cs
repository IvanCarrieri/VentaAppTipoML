
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
    public class InventarioRepositorio : Repositorio<Inventario>, IInventarioRepositorio
    {
        private ApplicationDbContext db;

        public InventarioRepositorio(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

       

        public void Actualizar(Inventario inventario)
        {
            var inventarioDB = db.Inventarios.FirstOrDefault(i=> i.Id == inventario.Id);

            if (inventarioDB != null)
            {
                inventarioDB.DepositoId = inventario.DepositoId;
                inventarioDB.FechaFinal = inventario.FechaFinal;
                inventarioDB.Estado = inventario.Estado;


                db.SaveChanges();
            }
        }

        public IEnumerable<SelectListItem> ObtenerTodosDropDownList(string obj)
        {

            if (obj == "Deposito")
            {
                return db.Depositos.Where(d => d.Estado == true).Select(d => new SelectListItem
                {
                    Text = d.Nombre,
                    Value = d.Id.ToString()
                });
            }
            return null;
        }
    }
}
