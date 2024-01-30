using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Migrations;
using SistemaInventario.AccesoDatos.Repositorios.IRepositorios;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;
using System.Security.Claims;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DefinicionesEstaticas.RoleAdmin)]
    public class EmpresaController : Controller
    {
        private readonly IUnidadTrabajo unidadTrabajo;

        public EmpresaController(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }

        public async Task<IActionResult> Upsert()
        {
            EmpresaVM empresaVM = new EmpresaVM()
            {
                Empresa = new Empresa(),
                DepositoLista = unidadTrabajo.Inventario.ObtenerTodosDropDownList("Deposito")

            };



            empresaVM.Empresa = await unidadTrabajo.Empresa.ObtenerPrimero();

            if(empresaVM.Empresa == null) 
            
            {
                empresaVM.Empresa = new Empresa();
            }

            return View(empresaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(EmpresaVM empresaVM)
        {

            if(ModelState.IsValid)
            {
               

                var c = (ClaimsIdentity)User.Identity;
                var usuario = c.FindFirst(ClaimTypes.NameIdentifier);

                if (empresaVM.Empresa.Id == 0) //creamos empresa
                {
                    empresaVM.Empresa.CreadoPorUsuarioId = usuario.Value;
                    empresaVM.Empresa.ActualizadoPorUsuarioId = usuario.Value;
                    empresaVM.Empresa.FechaCreacion = DateTime.Now;
                    empresaVM.Empresa.FechaActualizacion = DateTime.Now;

                    await unidadTrabajo.Empresa.Agregar(empresaVM.Empresa);
                    TempData[DefinicionesEstaticas.Exitosa] = "Empresa grabada con exito";

                }
                else //Actualizar Empresa
                {
                    empresaVM.Empresa.ActualizadoPorUsuarioId = usuario.Value;
                    empresaVM.Empresa.FechaActualizacion = DateTime.Now;

                    unidadTrabajo.Empresa.Actualizar(empresaVM.Empresa);
                    TempData[DefinicionesEstaticas.Exitosa] = "Empresa grabada con exito";
                }

                await unidadTrabajo.Guardar();
                return RedirectToAction("Index", "Home", new {area="Inventario"});
            }


            TempData[DefinicionesEstaticas.Error] = "Error al grabar empresa";
            return View(empresaVM);

        }



    }
}
