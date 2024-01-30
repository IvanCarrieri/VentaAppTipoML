using SistemaInventario.AccesoDatos.Repositorios.IRepositorios;
using SistemaInventario.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



//esta clase para que este accesible hay que agregar como servicio en porogram.cs
namespace SistemaInventario.AccesoDatos.Repositorios
{
    public class UnidadTrabajo : IUnidadTrabajo
    {

        private ApplicationDbContext db;
        public IDepositoRepositorio Deposito {  get; set; }

        public ICategoriaRepositorio Categoria { get; set; }

        public IMarcaRepositorio Marca { get; set; }
        public IProductoRepositorio Producto { get; set; }

        public IUsuarioRepositorio Usuario {  get; set; }

        public IDepositoProductoRepositorio DepositoProducto { get; set; }

        public IInventarioRepositorio Inventario {  get; set; }

        public IInventarioDetalleRepositorio InventarioDetalle { get; set; }

        public IKardexRepositorio Kardex { get; set; }

        public IEmpresaRepositorio Empresa { get; set; }
        public ICarritoRepositorio Carrito { get; set; }

        public IOrdenRepositorio Orden { get; set; }

        public IOrdenDetalleRepositorio OrdenDetalle { get; set; }

        public UnidadTrabajo(ApplicationDbContext db)
        {
            this.db = db;
            Deposito = new DepositoRepositorio(db);
            Categoria = new CategoriaRepositorio(db);
            Marca = new MarcaRepositorio(db);
            Producto = new ProductoRepositorio(db);
            Usuario = new UsuarioRepositorio(db);
            DepositoProducto = new DepositoProductoRepositorio (db);
            Inventario = new InventarioRepositorio(db);
            InventarioDetalle = new InventarioDetalleRepositorio(db);
            Kardex = new KardexRepositorio(db);
            Empresa = new EmpresaRepositorio(db);
            Carrito = new CarritoRepositorio(db);
            Orden = new OrdenRepositorio(db);
            OrdenDetalle = new OrdenDetalleRepositorio(db);

        }

        

        public void Dispose()
        {
            db.Dispose();
        }

        public async Task Guardar()
        {
            await db.SaveChangesAsync();
        }
    }
}
