
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
    public class DepositoRepositorio : Repositorio<Deposito>, IDepositoRepositorio
    {
        private ApplicationDbContext db;

        public DepositoRepositorio(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Actualizar(Deposito deposito)
        {
            var depositoBD = db.Depositos.FirstOrDefault(b => b.Id == deposito.Id);

            if(depositoBD != null)
            {
                depositoBD.Nombre = deposito.Nombre;
                depositoBD.Descripcion = deposito.Descripcion;
                depositoBD.Estado = deposito.Estado;
                db.SaveChanges();
            }
        }

    }
}
