
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
    public class ProductoRepositorio : Repositorio<Producto>, IProductoRepositorio
    {
        private ApplicationDbContext db;

        public ProductoRepositorio(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Actualizar(Producto producto)
        {
            var productoBD = db.Productos.FirstOrDefault(b => b.Id == producto.Id);

            if(productoBD != null)
            {
               
                if(producto.ImagenUrl != null)
                {
                    productoBD.ImagenUrl = producto.ImagenUrl;
                }

                productoBD.NumeroSerie = producto.NumeroSerie;
                productoBD.Descripcion = producto.Descripcion;
                productoBD.Estado = producto.Estado;
                productoBD.Precio = producto.Precio;
                productoBD.Costo = producto.Costo;
                productoBD.CategoriaId = producto.CategoriaId;
                productoBD.MarcaId = producto.MarcaId;
                productoBD.PadreId = producto.PadreId;


                db.SaveChanges();
            }
        }

        public IEnumerable<SelectListItem> ObtenerTodosDropDownList(string obj)
        {
           if (obj == "Categoria")
            {
                return db.Categorias.Where(c => c.Estado == true).Select(c => new SelectListItem
                {
                    Text = c.Nombre,
                    Value = c.Id.ToString()
                }) ;
            }
            if (obj == "Marca")
            {
                return db.Marcas.Where(m => m.Estado == true).Select(m => new SelectListItem
                {
                    Text = m.Nombre,
                    Value = m.Id.ToString()
                });
            }

            if (obj == "Producto")
            {
                return db.Productos.Where(m => m.Estado == true).Select(m => new SelectListItem
                {
                    Text = m.Descripcion,
                    Value = m.Id.ToString()
                });
            }

            return null;
        }
    }
}
