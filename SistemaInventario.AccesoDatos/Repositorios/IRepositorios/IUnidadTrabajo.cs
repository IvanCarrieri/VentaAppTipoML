using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorios.IRepositorios
{
    public interface IUnidadTrabajo: IDisposable
    {
        IDepositoRepositorio Deposito {  get; }
        ICategoriaRepositorio Categoria { get; }

        IMarcaRepositorio Marca { get; }

        IProductoRepositorio Producto { get; }

        IUsuarioRepositorio Usuario { get; }

        IDepositoProductoRepositorio DepositoProducto { get; }

        IInventarioRepositorio Inventario { get; }

        IInventarioDetalleRepositorio InventarioDetalle { get; }

        IKardexRepositorio Kardex { get; }

        IEmpresaRepositorio Empresa { get; }

        ICarritoRepositorio Carrito { get; }

        IOrdenRepositorio Orden { get; }

        IOrdenDetalleRepositorio OrdenDetalle { get; }

        Task Guardar();
    }
}
