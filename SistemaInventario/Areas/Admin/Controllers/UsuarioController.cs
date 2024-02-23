using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Repositorios.IRepositorios;
using SistemaInventario.Data;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = DS.RoleAdmin)]
    public class UsuarioController : Controller
    {

        private readonly IUnidadTrabajo unidadTrabajo;
        private readonly ApplicationDbContext context;

        public UsuarioController(IUnidadTrabajo unidadTrabajo, ApplicationDbContext context)
        {
            this.unidadTrabajo = unidadTrabajo;
            this.context = context;
        }

       

        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarioLista = await unidadTrabajo.Usuario.ObtenerTodos();
            var userRole = await context.UserRoles.ToListAsync();
            var roles = await context.Roles.ToListAsync();

            foreach(var usuario in usuarioLista) 
            
            {
                var RoleId = userRole.FirstOrDefault(u=>u.UserId == usuario.Id).RoleId;
                usuario.Role = roles.FirstOrDefault(u => u.Id == RoleId).Name;

               

            }
                 return View(usuarioLista);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BloquearDesbloquear(string id)
        {
            Usuario usuario = await unidadTrabajo.Usuario.ObtenerPrimero(u => u.Id == id);

            //Usuario usuario = await unidadTrabajo.Usuario.ObtenerString(id);

            if (usuario == null)
            {
                TempData[DS.Error] = "Error de usuario";
                return View();
            }
            if(usuario.LockoutEnd != null && usuario.LockoutEnd > DateTime.Now)
            {
                usuario.LockoutEnd = DateTime.Now;
            }
            else
            {
                usuario.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            await unidadTrabajo.Guardar();
            TempData[DS.Exitosa] = "Operación exitosa";
            return RedirectToAction("Index");
        }

        




    }
}
