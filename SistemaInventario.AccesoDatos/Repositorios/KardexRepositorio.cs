
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
    public class KardexRepositorio : Repositorio<Kardex>, IKardexRepositorio
    {
        private ApplicationDbContext db;

        public KardexRepositorio(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public async Task RegistrarKardex(int depositoProductoId, string tipo, string detalle, int stockAnterior, int cantidad, string usuarioId)
        {
           var depositoProducto = await db.DepositoProductos.Include(b=>b.Producto).FirstOrDefaultAsync(b=>b.Id == depositoProductoId);

            if (tipo == "Entrada")
            {
                Kardex kardex = new Kardex();
                kardex.DepositoProductoId = depositoProductoId;
                kardex.Tipo = tipo;
                kardex.Detalle = detalle;
                kardex.StockAnterior = stockAnterior;
                kardex.Cantidad = cantidad;
                kardex.Costo = depositoProducto.Producto.Costo;
                kardex.Stock = stockAnterior + cantidad;
                kardex.Total = kardex.Stock * kardex.Costo;
                kardex.UsuarioId = usuarioId;
                kardex.FechaRegistro = DateTime.Now;

                await db.Kardexs.AddAsync(kardex);
                await db.SaveChangesAsync();
            }

            if (tipo == "Salida")
            {
                Kardex kardex = new Kardex();
                kardex.DepositoProductoId = depositoProductoId;
                kardex.Tipo = tipo;
                kardex.Detalle = detalle;
                kardex.StockAnterior = stockAnterior;
                kardex.Cantidad = cantidad;
                kardex.Costo = depositoProducto.Producto.Costo;
                kardex.Stock = stockAnterior - cantidad;
                kardex.Total = kardex.Stock * kardex.Costo;
                kardex.UsuarioId = usuarioId;
                kardex.FechaRegistro = DateTime.Now;

                await db.Kardexs.AddAsync(kardex);
                await db.SaveChangesAsync();
            }

        }
    }
}
