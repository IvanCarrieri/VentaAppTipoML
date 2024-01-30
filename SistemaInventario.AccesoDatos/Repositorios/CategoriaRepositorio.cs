
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
    public class CategoriaRepositorio : Repositorio<Categoria>, ICategoriaRepositorio
    {
        private ApplicationDbContext db;

        public CategoriaRepositorio(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Actualizar(Categoria categoria)
        {
            var categoriaBD = db.Categorias.FirstOrDefault(b => b.Id == categoria.Id);

            if(categoriaBD != null)
            {
                categoriaBD.Nombre = categoria.Nombre;
                categoriaBD.Descripcion = categoria.Descripcion;
                categoriaBD.Estado = categoria.Estado;
                db.SaveChanges();
            }
        }

    }
}
