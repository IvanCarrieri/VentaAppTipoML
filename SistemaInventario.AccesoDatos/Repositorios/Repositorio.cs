using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Repositorios.IRepositorio;
using SistemaInventario.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorios
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
        private readonly ApplicationDbContext db;

        private DbSet<T> dbSet;

        public Repositorio(ApplicationDbContext db)
        {
            this.db = db;
            this.dbSet = db.Set<T>();
        }

        public async Task Agregar(T entidad)
        {
            await dbSet.AddAsync(entidad);  //insert into
        }

        public async Task<T> Obtener(int id)
        {
            return await dbSet.FindAsync(id); //selct * from ID
        }

        public async Task<T> ObtenerString(string id)
        {
            return await dbSet.FindAsync(id); //selct * from ID
        }

        public async Task<T> ObtenerPrimero(System.Linq.Expressions.Expression<Func<T, bool>> filtro = null, string incluirPropiedades = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            if (filtro != null)
            {
                query = query.Where(filtro);  //select * from where

            }
            if (incluirPropiedades != null)
            {
                foreach (var item in incluirPropiedades.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item); //incluye categoria, marca)
                }
            }
          

            if (!isTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> ObtenerTodos(System.Linq.Expressions.Expression<Func<T, bool>> filtro = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string? incluirPropiedades = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            if (filtro != null)
            {
                query = query.Where(filtro);  //select * from where

            }
            if (incluirPropiedades != null)
            {
                foreach (var item in incluirPropiedades.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item); //incluye categoria, marca)
                }
            }
            if(orderBy != null)
            {
                query = orderBy(query);
            }

            if (!isTracking)
            {
                query= query.AsNoTracking();
            }
            return await query.ToListAsync();
        }

        public void Remover(T entidad)
        {
            dbSet.Remove(entidad);
        }

        public void RemoverRango(IEnumerable<T> entidad)
        {
           dbSet.RemoveRange(entidad);
        }
    }
}
