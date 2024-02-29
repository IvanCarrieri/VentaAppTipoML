using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorios.IRepositorios;
using SistemaInventario.Utilidades;
using SistemaInventario.Modelos;
using System.Security.Claims;
using System.Diagnostics.Eventing.Reader;

namespace SistemaInventario.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize]
    public class OrdenController : Controller
    {
        private readonly IUnidadTrabajo unidadTrabajo;

        [BindProperty]
        public OrdenDetalleVM ordenDetalleVM { get; set; }


        public OrdenController(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;   
        }

        public async Task<IActionResult> Index(string estado)
        {
            var c = (ClaimsIdentity)User.Identity;
            var usuario = c.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<Orden> ordenLista;

            if (User.IsInRole(DS.RoleAdmin)) //valido si es rol de admin
            {
                ordenLista = await unidadTrabajo.Orden.ObtenerTodos(/*o=>o.EstadoOrden != DS.EstadoPendiente,*/ incluirPropiedades: "Usuario");
            }
            else
            {
                ordenLista = await unidadTrabajo.Orden.ObtenerTodos(o=> o.UsuarioId == usuario.Value /*&& o.EstadoOrden != DS.EstadoPendiente*/, incluirPropiedades: "Usuario");
            }

            //validar estado
            switch (estado)
            {
                case "aprobado":
                    ordenLista = ordenLista.Where(o => o.EstadoOrden == DS.EstadoAprobado);
                    break;
                case "completado":
                    ordenLista = ordenLista.Where(o=>o.EstadoOrden == DS.EstadoEnviado);
                    break;
                default:
                    break;
            }


            return View(ordenLista);
        }

        public async Task<IActionResult> Detalle (int id)
        {
            ordenDetalleVM = new OrdenDetalleVM()
            {
                Orden = await unidadTrabajo.Orden.ObtenerPrimero(o => o.Id == id, incluirPropiedades: "Usuario"),

                OrdenDetalleLista = await unidadTrabajo.OrdenDetalle.ObtenerTodos(d => d.OrdenId == id, incluirPropiedades: "Producto")

            };

            return View(ordenDetalleVM);
        }

        [Authorize(Roles = DS.RoleAdmin)]
        public async Task<IActionResult> Procesar(int id)
        {
            Orden orden = await unidadTrabajo.Orden.ObtenerPrimero(o => o.Id == id);
            orden.EstadoOrden = DS.EstadoEnProceso;
            await unidadTrabajo.Guardar();

            TempData[DS.Exitosa] = "Orden cambiado a estado en proceso";

            //por ser un metodo y pasar un parametro escribir RedirecToAction
            //recordar lo que recibe el IActionResult Detalle que es el id
            return RedirectToAction("Detalle", new {id = id});
        }

        [HttpPost]
        [Authorize(Roles = DS.RoleAdmin)]
        public async Task<IActionResult> EnviarOrden(OrdenDetalleVM ordenDetalleVM)
        {
            Orden orden = await unidadTrabajo.Orden.ObtenerPrimero(o => o.Id == ordenDetalleVM.Orden.Id);
            orden.EstadoOrden = DS.EstadoEnviado;
            orden.Carrier = ordenDetalleVM.Orden.Carrier;
            orden.NumeroEnvio = ordenDetalleVM.Orden.NumeroEnvio;
            orden.FechaEnvio = DateTime.Now;

            await unidadTrabajo.Guardar();

            TempData[DS.Exitosa] = "Orden cambiado a estado enviado";

            //por ser un metodo y pasar un parametro escribir RedirecToAction
            //recordar lo que recibe el IActionResult Detalle que es el id
            return RedirectToAction("Detalle", new { id = ordenDetalleVM.Orden.Id});
            

        }

    }
}
