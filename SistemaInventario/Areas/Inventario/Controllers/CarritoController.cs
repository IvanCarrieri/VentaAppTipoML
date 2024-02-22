using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorios.IRepositorios;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;
using System.Security.Claims;

namespace SistemaInventario.Areas.Inventario.Controllers
{ 
    [Area("Inventario")]
    public class CarritoController : Controller
    {
       

        private readonly IUnidadTrabajo unidadTrabajo;

        [BindProperty]
        public CarritoVM carritoVM { get; set; }


        public CarritoController(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            //capturar el usuario
            var c = (ClaimsIdentity)User.Identity;
            var usuario = c.FindFirst(ClaimTypes.NameIdentifier);

            carritoVM = new CarritoVM();
            carritoVM.Orden = new Orden();
            carritoVM.CarritoLista = await unidadTrabajo.Carrito.ObtenerTodos(c => c.UsuarioId == usuario.Value, incluirPropiedades: "Producto");

            carritoVM.Orden.TotalOrden = 0;
            carritoVM.Orden.UsuarioId = usuario.Value;

            //recorro lista
            foreach (var item in carritoVM.CarritoLista)
            {
                item.Precio = item.Producto.Precio; //muestro el precio actual del producto
                carritoVM.Orden.TotalOrden += (item.Precio * item.Cantidad);

            }
            //cuando retornamos a la vista el carritoVM ya esta lleno con lo que nos interesa
            return View(carritoVM);
        }

        public async Task<IActionResult> mas(int carroId)
        {
            //obtengo el registro
            var carrito = await unidadTrabajo.Carrito.ObtenerPrimero(c=>c.Id == carroId);
            carrito.Cantidad += 1;

            await unidadTrabajo.Guardar();

            //esto es un metodo(funcion) no hace return view.
            return RedirectToAction("Index");

            
        }

        public async Task <IActionResult> menos(int carroId)
        {
            //obtengo el registro
            var carrito = await unidadTrabajo.Carrito.ObtenerPrimero(c => c.Id == carroId);
           
            
            if(carrito.Cantidad == 1)
            {
                //removemos el registro y actualizamos la sesion
                //capturo la lista de productos
                var carritoLista = await unidadTrabajo.Carrito.ObtenerTodos(c => c.UsuarioId == carrito.UsuarioId);

                var numProductos = carritoLista.Count();


                unidadTrabajo.Carrito.Remover(carrito);
                await unidadTrabajo.Guardar();
                //actualizo la sesion 
                HttpContext.Session.SetInt32(DefinicionesEstaticas.SesionCarrito, numProductos - 1);

            }
            else
            {
                  carrito.Cantidad -= 1;
                await unidadTrabajo.Guardar();
            }
           

          

            //esto es un metodo(funcion) no hace return view.
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> remover(int carroId)
        {

            var carrito = await unidadTrabajo.Carrito.ObtenerPrimero(c => c.Id == carroId);

            //removemos el registro y actualizamos la sesion
            //capturo la lista de productos
            var carritoLista = await unidadTrabajo.Carrito.ObtenerTodos(c => c.UsuarioId == carrito.UsuarioId);

            var numProductos = carritoLista.Count();


            //borro
            unidadTrabajo.Carrito.Remover(carrito);

              await unidadTrabajo.Guardar();


            //actualizo la sesion 
            HttpContext.Session.SetInt32(DefinicionesEstaticas.SesionCarrito, numProductos - 1);

           
            //esto es un metodo(funcion) no hace return view.
            return RedirectToAction("Index");
        }

    }
}
