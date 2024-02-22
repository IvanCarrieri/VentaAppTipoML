using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorios;
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


        public async Task<IActionResult> Comprar()
        {
            //capturar el usuario
            var c = (ClaimsIdentity)User.Identity;
            var usuario = c.FindFirst(ClaimTypes.NameIdentifier);


            carritoVM = new CarritoVM();
            carritoVM.Orden = new Orden();

            carritoVM.Empresa = await unidadTrabajo.Empresa.ObtenerPrimero();

            carritoVM.CarritoLista = await unidadTrabajo.Carrito.ObtenerTodos(c => c.UsuarioId == usuario.Value, incluirPropiedades: "Producto");



            carritoVM.Orden.TotalOrden = 0;
            carritoVM.Orden.Usuario = await unidadTrabajo.Usuario.ObtenerPrimero(u=>u.Id == usuario.Value);

            foreach(var item in carritoVM.CarritoLista)
            {
                item.Precio = item.Producto.Precio; //muestro el precio actual del producto
                carritoVM.Orden.TotalOrden += (item.Precio * item.Cantidad);


                
            }


            carritoVM.Orden.NombreCliente = carritoVM.Orden.Usuario.Nombres + " " + carritoVM.Orden.Usuario.Apellidos;
            carritoVM.Orden.Telefono = carritoVM.Orden.Usuario.PhoneNumber;
            carritoVM.Orden.Direccion = carritoVM.Orden.Usuario.Direccion;
            carritoVM.Orden.Pais = carritoVM.Orden.Usuario.Pais;
            carritoVM.Orden.Ciudad = carritoVM.Orden.Usuario.Ciudad;

            foreach (var item in carritoVM.CarritoLista)
            {
                //controlar stock

                //capturo el stock
                var producto = await unidadTrabajo.DepositoProducto.ObtenerPrimero(d => d.Producto.Id == item.ProductoId &&
                d.DepositoId == carritoVM.Empresa.DepositoVentaId);

                if (item.Cantidad > producto.Cantidad)
                {

                    TempData[DefinicionesEstaticas.Error] = "La cantidad de producto " + item.Producto.Descripcion + " , excede al stock actual: " + producto.Cantidad;

                    return RedirectToAction("Index");
                }


            }


            
            return View(carritoVM);
        }


    }
}
