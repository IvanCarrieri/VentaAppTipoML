using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorios.IRepositorios;
using SistemaInventario.Modelos;
using SistemaInventario.Models;
using System.Diagnostics;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnidadTrabajo unidadTrabajo;
        private const int CantidadCardsPorPag = 12;
        private CarritoVM carritoVM { get; set; }

        public HomeController(ILogger<HomeController> logger, IUnidadTrabajo unidadTrabajo)
        {
            this._logger = logger;
            this.unidadTrabajo = unidadTrabajo;
        }



        public async Task<IActionResult> Index(int pagina = 1, string busqueda ="")
        {

            // Controlar sesion
            var c = (ClaimsIdentity)User.Identity;
            var usuario = c.FindFirst(ClaimTypes.NameIdentifier);

            if (usuario != null)
            {
                var carritoLista = await unidadTrabajo.Carrito.ObtenerTodos(c => c.UsuarioId == usuario.Value);
                var numeroProductos = carritoLista.Count();
                HttpContext.Session.SetInt32(DefinicionesEstaticas.SesionCarrito, numeroProductos);
            }


            //incluyo paginación de cards

            // Obtiene la lista completa de productos
            IEnumerable<Producto> listaProductos = await unidadTrabajo.Producto.ObtenerTodos();

            if (!string.IsNullOrEmpty(busqueda))
            {
                listaProductos = listaProductos.Where(p => p.Descripcion.Contains(busqueda));
            }

            // Calcula la cantidad total de páginas
            int TotalPaginas = (int)Math.Ceiling((double)listaProductos.Count() / CantidadCardsPorPag);

            // Asegura que la página solicitada esté dentro de los límites
            pagina = Math.Max(1, Math.Min(pagina, TotalPaginas));

            // Calcula el índice inicial y final de las tarjetas para la página actual
            int indiceInicial = (pagina - 1) * CantidadCardsPorPag;
            int indiceFinal = Math.Min(indiceInicial + CantidadCardsPorPag - 1, listaProductos.Count() - 1);

            // Obtiene solo las tarjetas necesarias para la página actual
            List<Producto> ProductosMostrados = listaProductos.Skip(indiceInicial).Take(CantidadCardsPorPag).ToList();

            // Pasa los datos a la vista
            ViewBag.Productos = ProductosMostrados;
            ViewBag.TotalPaginas = TotalPaginas;
            ViewBag.pagina = pagina;
            ViewBag.Busqueda = busqueda;



            return View(ProductosMostrados);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Detalle(int id)
        {
            carritoVM = new CarritoVM();
            carritoVM.Producto = await unidadTrabajo.Producto.ObtenerPrimero(p => p.Id == id, incluirPropiedades: "Marca,Categoria"); 
            carritoVM.Empresa = await unidadTrabajo.Empresa.ObtenerPrimero();

            var depositoProducto = await unidadTrabajo.DepositoProducto.ObtenerPrimero(b => b.ProductoId == id && b.DepositoId == carritoVM.Empresa.DepositoVentaId);
            
            if(depositoProducto == null)
            {
                carritoVM.Stock = 0;
            }
            else
            {
                carritoVM.Stock = depositoProducto.Cantidad;
            }


            carritoVM.Carrito = new Carrito()
            {
                Producto = carritoVM.Producto,
                ProductoId = carritoVM.Producto.Id,
            };

            return View(carritoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Detalle(CarritoVM carritoVM)
        {
            //asi se captura el usuario
            var c = (ClaimsIdentity)User.Identity;
            var usuario = c.FindFirst(ClaimTypes.NameIdentifier);
            carritoVM.Carrito.UsuarioId = usuario.Value;

            Carrito carroBD = await unidadTrabajo.Carrito.ObtenerPrimero(c=>c.UsuarioId == usuario.Value && c.ProductoId == carritoVM.Carrito.ProductoId);

            if(carroBD == null)
            {
                await unidadTrabajo.Carrito.Agregar(carritoVM.Carrito);
            }
            else
            {
                carroBD.Cantidad += carritoVM.Carrito.Cantidad;
                unidadTrabajo.Carrito.Actualizar(carroBD);
            }

            await unidadTrabajo.Guardar();
            TempData[DefinicionesEstaticas.Exitosa] = "Producto agregado al carrito de compras";

            //agregar valor a la sesion
            var carritoLista = await unidadTrabajo.Carrito.ObtenerTodos(c => c.UsuarioId == usuario.Value);
            var numeroProductos = carritoLista.Count();
            HttpContext.Session.SetInt32(DefinicionesEstaticas.SesionCarrito, numeroProductos);


            return RedirectToAction("Index");



        }

    }
}






