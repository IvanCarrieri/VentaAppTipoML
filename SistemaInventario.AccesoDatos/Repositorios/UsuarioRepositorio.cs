
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
    public class UsuarioRepositorio : Repositorio<Usuario>, IUsuarioRepositorio
    {
        private ApplicationDbContext db;

        public UsuarioRepositorio(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

       
    }
}
