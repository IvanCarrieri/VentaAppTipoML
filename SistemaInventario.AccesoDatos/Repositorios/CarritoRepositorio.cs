
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
    public class CarritoRepositorio : Repositorio<Carrito>, ICarritoRepositorio
    {
        private ApplicationDbContext db;

        public CarritoRepositorio(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        /* public void Actualizar(Carrito carrito)
         {
             var carritoBD = db.Carritos.FirstOrDefault(b => b.Id == carrito.Id);

             if(carritoBD != null)
             {

                 carritoBD.UsuarioId = carrito.UsuarioId;
                 carritoBD.ProductoId = carrito.ProductoId;
                 carritoBD.Cantidad = carrito.Cantidad;

                 //aqui no van las propiedades Foreingkeys 
                 db.SaveChanges();
             }
         }*/

        //Otra forma mas simple para actualizar:
        public void Actualizar(Carrito carrito)
        {
            db.Update(carrito);
        }




    }
}
