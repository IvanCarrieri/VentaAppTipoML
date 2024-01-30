
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
    public class MarcaRepositorio : Repositorio<Marca>, IMarcaRepositorio
    {
        private ApplicationDbContext db;

        public MarcaRepositorio(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Actualizar(Marca marca)
        {
            var marcaBD = db.Marcas.FirstOrDefault(b => b.Id == marca.Id);

            if(marcaBD != null)
            {
                marcaBD.Nombre = marca.Nombre;
                marcaBD.Descripcion = marca.Descripcion;
                marcaBD.Estado = marca.Estado;
                db.SaveChanges();
            }
        }

    }
}
