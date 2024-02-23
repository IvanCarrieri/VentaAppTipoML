using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using SistemaInventario.AccesoDatos.Migrations;
using SistemaInventario.AccesoDatos.Repositorios.IRepositorios;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace SistemaInventario.Areas.Inventario.Controllers
{

    [Area("Inventario")]
    [Authorize(Roles = DS.RoleAdmin + "," + DS.RoleInventario)]
    public class InventarioController : Controller
    {
        private readonly IUnidadTrabajo unidadTrabajo;

        [BindProperty]
        private InventarioVM inventarioVM { get; set; }



        public InventarioController(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }

        public async Task<IActionResult> Index()
        {

            IEnumerable<DepositoProducto> InventarioLista = await unidadTrabajo.DepositoProducto.ObtenerTodos(incluirPropiedades: "Deposito,Producto");


            return View(InventarioLista);
        }


        public IActionResult NuevoInventario()
        {

            //asi inicializo una clase donde debo inicializar dentro propiedades de tipo clase o tipi list o IEnumerable
            inventarioVM = new InventarioVM()
            {
                Inventario = new Modelos.Inventario(),
                DepositoLista = unidadTrabajo.Inventario.ObtenerTodosDropDownList("Deposito")
            };

            inventarioVM.Inventario.Estado = false;

            //obtener y capturar el Id del Usuario desde la sesion que esta abierta
            var u = (ClaimsIdentity)User.Identity;
            var usuario = u.FindFirst(ClaimTypes.NameIdentifier);
            inventarioVM.Inventario.UsuarioId = usuario.Value;
            inventarioVM.Inventario.FechaInicial = DateTime.Now;
            inventarioVM.Inventario.FechaFinal = DateTime.Now;


            return View(inventarioVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NuevoInventario(InventarioVM inventarioVM) //teniendo el mismo nombre que el get, por medio de un submit en la misma vista, llama al metodo.
        {
            if (ModelState.IsValid)
            {
                inventarioVM.Inventario.FechaInicial = DateTime.Now;
                inventarioVM.Inventario.FechaFinal = DateTime.Now;
                await unidadTrabajo.Inventario.Agregar(inventarioVM.Inventario);
                await unidadTrabajo.Guardar();

                return RedirectToAction("DetalleInventario", new { id = inventarioVM.Inventario.Id });
            }

            inventarioVM.DepositoLista = unidadTrabajo.Inventario.ObtenerTodosDropDownList("Deposito");
            return View(inventarioVM);
        }

        public async Task<IActionResult> DetalleInventario(int id)
        {
            inventarioVM = new InventarioVM();
            inventarioVM.Inventario = await unidadTrabajo.Inventario.ObtenerPrimero(i => i.Id == id, incluirPropiedades: "Deposito");
            inventarioVM.InventarioDetalles = await unidadTrabajo.InventarioDetalle.ObtenerTodos(d => d.InventarioId == id, incluirPropiedades: "Inventario,Producto.Marca");

            return View(inventarioVM);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetalleInventario(int inventarioId, int productoId, int cantidadId)
        {
            if (productoId.ToString() == "" || productoId == 0)
            {
                TempData[DS.Error] = $"Error: Seleccione un producto";
                return RedirectToAction("DetalleInventario", new { id = inventarioId });
            }
            if (cantidadId.ToString() == "" || cantidadId < 1)
            {
                TempData[DS.Error] = $"Error: Ingrese una cantidad correcta";
                return RedirectToAction("DetalleInventario", new { id = inventarioId });
            }



            inventarioVM = new InventarioVM();
            inventarioVM.Inventario = await unidadTrabajo.Inventario.ObtenerPrimero(i => i.Id == inventarioId);


            var depositoProducto = await unidadTrabajo.DepositoProducto.ObtenerPrimero(d => d.ProductoId == productoId && d.DepositoId == inventarioVM.Inventario.DepositoId);

            var detalle = await unidadTrabajo.InventarioDetalle.ObtenerPrimero(d => d.InventarioId == inventarioId && d.ProductoId == productoId);

            if (detalle == null)
            {
                inventarioVM.InventarioDetalle = new InventarioDetalle();

                inventarioVM.InventarioDetalle.ProductoId = productoId;
                inventarioVM.InventarioDetalle.InventarioId = inventarioId;
                if (depositoProducto != null)
                {
                    inventarioVM.InventarioDetalle.StockAnterior = depositoProducto.Cantidad;
                }
                else
                {
                    inventarioVM.InventarioDetalle.StockAnterior = 0;
                }
                inventarioVM.InventarioDetalle.Cantidad = cantidadId;

                await unidadTrabajo.InventarioDetalle.Agregar(inventarioVM.InventarioDetalle);
                await unidadTrabajo.Guardar();
            }
            else
            {
                detalle.Cantidad += cantidadId;
                await unidadTrabajo.Guardar();
            }

            return RedirectToAction("DetalleInventario", new { id = inventarioId });
        }

        public async Task<IActionResult> Mas(int id) //Recibe el Id de InventarioDetalle
        {
            inventarioVM = new InventarioVM();
            var InventarioDetalle = await unidadTrabajo.InventarioDetalle.Obtener(id);
            inventarioVM.Inventario = await unidadTrabajo.Inventario.Obtener(InventarioDetalle.InventarioId);



            InventarioDetalle.Cantidad += 1;
            await unidadTrabajo.Guardar();

            return RedirectToAction("DetalleInventario", new { id = inventarioVM.Inventario.Id });
        }



        public async Task<IActionResult> Menos(int id) //Recibe el Id de InventarioDetalle
        {
            inventarioVM = new InventarioVM();
            var InventarioDetalle = await unidadTrabajo.InventarioDetalle.Obtener(id);
            inventarioVM.Inventario = await unidadTrabajo.Inventario.Obtener(InventarioDetalle.InventarioId);

            if (InventarioDetalle.Cantidad == 1)
            {
                unidadTrabajo.InventarioDetalle.Remover(InventarioDetalle);
                await unidadTrabajo.Guardar();
            }
            else
            {
                InventarioDetalle.Cantidad -= 1;
                await unidadTrabajo.Guardar();
            }


            return RedirectToAction("DetalleInventario", new { id = inventarioVM.Inventario.Id });
        }

        public async Task<IActionResult> GenerarStock(int id)
        {
            var inventario = await unidadTrabajo.Inventario.Obtener(id);
            var InventarioDetalleLista = await unidadTrabajo.InventarioDetalle.ObtenerTodos(d => d.InventarioId == id);

            var u = (ClaimsIdentity)User.Identity;
            var usuario = u.FindFirst(ClaimTypes.NameIdentifier);

            foreach (var item in InventarioDetalleLista)
            {
                var depositoProducto = new DepositoProducto();
                depositoProducto = await unidadTrabajo.DepositoProducto.ObtenerPrimero(b => b.ProductoId == item.ProductoId && b.DepositoId == inventario.DepositoId);

                if (depositoProducto != null)// si el registro existe , hay que actualizar las cantidades
                {
                    await unidadTrabajo.Kardex.RegistrarKardex(depositoProducto.Id, "Entrada", "Registro de inventario", depositoProducto.Cantidad, item.Cantidad, usuario.Value);
                    depositoProducto.Cantidad += item.Cantidad;
                    await unidadTrabajo.Guardar();
                }
                else //si el registro de stock no existe hay que crearlo
                {


                    depositoProducto = new DepositoProducto();
                    depositoProducto.DepositoId = inventario.DepositoId;
                    depositoProducto.ProductoId = item.ProductoId;
                    depositoProducto.Cantidad = item.Cantidad;

                    await unidadTrabajo.DepositoProducto.Agregar(depositoProducto);
                    await unidadTrabajo.Guardar();

                    await unidadTrabajo.Kardex.RegistrarKardex(depositoProducto.Id, "Entrada", "Inventario inicial", 0, item.Cantidad, usuario.Value);
                }
            }

            inventario.Estado = true;
            inventario.FechaFinal = DateTime.Now;
            await unidadTrabajo.Guardar();

            TempData[DS.Exitosa] = "Stock Generado con éxito";

            return RedirectToAction("Index");

        }

        public IActionResult KardexProducto()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult KardexProducto(string fechaInicioId, string fechaFinalId, int productoId)
        {
            return RedirectToAction("KardexProductoResultado", new { fechaInicioId, fechaFinalId, productoId });
        }

        public async Task<IActionResult> KardexProductoResultado(string fechaInicioId, string fechaFinalId, int productoId)
        {
            KardexVM kardexVM = new KardexVM();
            kardexVM.Producto = new Producto();
            kardexVM.Producto = await unidadTrabajo.Producto.Obtener(productoId);

            kardexVM.FechaInicio = DateTime.Parse(fechaInicioId);
            kardexVM.FechaFinal = DateTime.Parse(fechaFinalId).AddHours(23).AddMinutes(59);

            kardexVM.KardexLista = await unidadTrabajo.Kardex.ObtenerTodos(k=> k.DepositoProducto.ProductoId == productoId && (k.FechaRegistro >= kardexVM.FechaInicio && k.FechaRegistro <= kardexVM.FechaFinal), incluirPropiedades: "DepositoProducto,DepositoProducto.Producto,DepositoProducto.Deposito", orderBy: o=> o.OrderBy(o=>o.FechaRegistro));

            return View(kardexVM);

        }

        public async Task<IActionResult> ImprimirKardex(string fechaInicio, string fechaFinal, int productoId)
        {
            KardexVM kardexVM = new KardexVM();
            kardexVM.Producto = new Producto();
            kardexVM.Producto = await unidadTrabajo.Producto.Obtener(productoId);

            kardexVM.FechaInicio = DateTime.Parse(fechaInicio);
            kardexVM.FechaFinal = DateTime.Parse(fechaFinal).AddHours(23).AddMinutes(59); 
            kardexVM.KardexLista = await unidadTrabajo.Kardex.ObtenerTodos(k => k.DepositoProducto.ProductoId == productoId && (k.FechaRegistro >= kardexVM.FechaInicio && k.FechaRegistro <= kardexVM.FechaFinal), incluirPropiedades: "DepositoProducto,DepositoProducto.Producto,DepositoProducto.Deposito", orderBy: o => o.OrderBy(o => o.FechaRegistro));

            return new ViewAsPdf("ImprimirKardex", kardexVM)
            {
                FileName = "KardexProducto.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"

            };

        }



        [HttpGet]
        public async Task<IActionResult> BuscarProducto(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {

                var listaProductos = await unidadTrabajo.Producto.ObtenerTodos(p => p.Estado == true);
                var data = listaProductos.Where(x => x.NumeroSerie.Contains(term, StringComparison.OrdinalIgnoreCase) || x.Descripcion.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();
                return Ok(data);

            }

            return Ok();
        }


    }
}
