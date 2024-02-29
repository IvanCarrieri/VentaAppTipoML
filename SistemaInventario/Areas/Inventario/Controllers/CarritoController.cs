using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorios;
using SistemaInventario.AccesoDatos.Repositorios.IRepositorios;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;
using Stripe.Checkout;
using System.Security.Claims;

namespace SistemaInventario.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    public class CarritoController : Controller
    {


        private readonly IUnidadTrabajo unidadTrabajo;
        private string webUrl;
        private readonly IConfiguration configuration;



        [BindProperty]
        public CarritoVM carritoVM { get; set; }


        public CarritoController(IUnidadTrabajo unidadTrabajo, IConfiguration configuration)
        {

            this.unidadTrabajo = unidadTrabajo;
            webUrl = configuration.GetValue<string>("DomainUrls:WEB_URL:");
            this.configuration = configuration;
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
            var carrito = await unidadTrabajo.Carrito.ObtenerPrimero(c => c.Id == carroId);
            carrito.Cantidad += 1;

            await unidadTrabajo.Guardar();

            //esto es un metodo(funcion) no hace return view.
            return RedirectToAction("Index");


        }

        public async Task<IActionResult> menos(int carroId)
        {
            //obtengo el registro
            var carrito = await unidadTrabajo.Carrito.ObtenerPrimero(c => c.Id == carroId);


            if (carrito.Cantidad == 1)
            {
                //removemos el registro y actualizamos la sesion
                //capturo la lista de productos
                var carritoLista = await unidadTrabajo.Carrito.ObtenerTodos(c => c.UsuarioId == carrito.UsuarioId);

                var numProductos = carritoLista.Count();


                unidadTrabajo.Carrito.Remover(carrito);
                await unidadTrabajo.Guardar();
                //actualizo la sesion 
                HttpContext.Session.SetInt32(DS.SesionCarrito, numProductos - 1);

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
            HttpContext.Session.SetInt32(DS.SesionCarrito, numProductos - 1);


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
            carritoVM.Orden.Usuario = await unidadTrabajo.Usuario.ObtenerPrimero(u => u.Id == usuario.Value);

            foreach (var item in carritoVM.CarritoLista)
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

                    TempData[DS.Error] = "La cantidad de producto " + item.Producto.Descripcion + " , excede al stock actual: " + producto.Cantidad;

                    return RedirectToAction("Index");
                }


            }



            return View(carritoVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Comprar(CarritoVM carritoVM)
        {

            //capturar el usuario
            var c = (ClaimsIdentity)User.Identity;
            var usuario = c.FindFirst(ClaimTypes.NameIdentifier);

            carritoVM.CarritoLista = await unidadTrabajo.Carrito.ObtenerTodos(c => c.UsuarioId == usuario.Value, incluirPropiedades: "Producto");
            carritoVM.Empresa = await unidadTrabajo.Empresa.ObtenerPrimero();

            carritoVM.Orden.TotalOrden = 0;
            carritoVM.Orden.UsuarioId = usuario.Value;
            carritoVM.Orden.FechaOrden = DateTime.Now;

            foreach (var item in carritoVM.CarritoLista)
            {
                item.Precio = item.Producto.Precio;
                carritoVM.Orden.TotalOrden += (item.Precio + item.Cantidad);

            }

            //controlar stock
            foreach (var item in carritoVM.CarritoLista)
            {
                var producto = await unidadTrabajo.DepositoProducto.ObtenerPrimero(d => d.Producto.Id == item.ProductoId &&
                d.DepositoId == carritoVM.Empresa.DepositoVentaId);

                if (item.Cantidad > producto.Cantidad)
                {

                    TempData[DS.Error] = "La cantidad de producto " + item.Producto.Descripcion + " , excede al stock actual: " + producto.Cantidad;

                    return RedirectToAction("Index");
                }

            }

            carritoVM.Orden.EstadoOrden = DS.EstadoPendiente;
            carritoVM.Orden.EstadoPago = DS.PagoPendiente;

            await unidadTrabajo.Orden.Agregar(carritoVM.Orden);
            await unidadTrabajo.Guardar();

            //Grabar Detalle de Orden 
            foreach (var item in carritoVM.CarritoLista)
            {
                OrdenDetalle ordenDetalle = new OrdenDetalle()
                {
                    ProductoId = item.Producto.Id,
                    OrdenId = carritoVM.Orden.Id,
                    Precio = item.Precio,
                    Cantidad = item.Cantidad,
                };

                await unidadTrabajo.OrdenDetalle.Agregar(ordenDetalle);
                await unidadTrabajo.Guardar();

            }

            //stripe. sistema de pagos


            var usuario2 = await unidadTrabajo.Usuario.ObtenerPrimero(u => u.Id == usuario.Value);
            var options = new SessionCreateOptions
            {
                //SuccessUrl = webUrl + $"inventario/carrito/ordenConfirmacion?id={carritoVM.Orden.Id}",
                //CancelUrl = webUrl + "inventario/carrito/index",

                SuccessUrl = $"{configuration["DomainUrls:WEB_URL"]}inventario/carrito/ordenConfirmacion?id={carritoVM.Orden.Id}",
                CancelUrl = $"{configuration["DomainUrls:WEB_URL"]}inventario/carrito/index",


                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = usuario2.Email
            };
            foreach (var item in carritoVM.CarritoLista)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(item.Precio * 100),  // $20  => 200
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Producto.Descripcion
                        }
                    },
                    Quantity = item.Cantidad
                };
                options.LineItems.Add(sessionLineItem);
            }
            var service = new SessionService();
            Session session = service.Create(options);
            unidadTrabajo.Orden.ActualizarPagoStripe(carritoVM.Orden.Id, session.Id, session.PaymentIntentId);
            await unidadTrabajo.Guardar();

            Response.Headers.Add("Location", session.Url);  // Redirecciona a Stripe

            return new StatusCodeResult(303);

            
        }

        public async Task<IActionResult> ordenConfirmacion(int id)
        {

            var orden = await unidadTrabajo.Orden.ObtenerPrimero(o => o.Id == id, incluirPropiedades: "Usuario");
            var service = new SessionService();
            Session session = service.Get(orden.SessionId);

            var carrito = await unidadTrabajo.Carrito.ObtenerTodos(c => c.UsuarioId == orden.UsuarioId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                unidadTrabajo.Orden.ActualizarPagoStripe(id, session.Id, session.PaymentIntentId);
                unidadTrabajo.Orden.ActualizarEstado(id, DS.EstadoAprobado, DS.PagAprobado);
                await unidadTrabajo.Guardar();


                //Disminuir stock
                var empresa = await unidadTrabajo.Empresa.ObtenerPrimero();
                foreach(var item in carrito)
                {
                    DepositoProducto depositoProducto = new DepositoProducto();
                    depositoProducto = await unidadTrabajo.DepositoProducto.ObtenerPrimero(d=>d.ProductoId == item.ProductoId && d.DepositoId == empresa.DepositoVentaId);
                    await unidadTrabajo.Kardex.RegistrarKardex(depositoProducto.Id, "Salida", "Venta Orden# " + id, depositoProducto.Cantidad, item.Cantidad, orden.UsuarioId);


                    depositoProducto.Cantidad -= item.Cantidad;
                    await unidadTrabajo.Guardar();


                }



            }

            //borrar carrito
            
            List<Carrito> carritosLista = new List<Carrito>();
            carritosLista = carrito.ToList();
            unidadTrabajo.Carrito.RemoverRango(carritosLista);
            await unidadTrabajo.Guardar();
            HttpContext.Session.SetInt32(DS.SesionCarrito, 0);

            return View(id);
        }

    }
}
