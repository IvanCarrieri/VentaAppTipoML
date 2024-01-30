using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SistemaInventario.AccesoDatos.Repositorios.IRepositorios;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DefinicionesEstaticas.RoleAdmin)]
    public class DepositoController : Controller
    {

        private readonly IUnidadTrabajo unidadTrabajo;



        public DepositoController(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            IEnumerable<Deposito> depositosLista = await unidadTrabajo.Deposito.ObtenerTodos();



            return View(depositosLista);



        }






        [HttpGet]

        public async Task<IActionResult> Delete(int? id)
        {

            Deposito deposito = await unidadTrabajo.Deposito.Obtener(id.GetValueOrDefault());

            if (deposito == null)
            {
                return NotFound("Error al borrar o el depósito no existe.");
            }

            return View(deposito);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(int id)
        {

            Deposito deposito = await unidadTrabajo.Deposito.Obtener(id);
            unidadTrabajo.Deposito.Remover(deposito);
            TempData[DefinicionesEstaticas.Exitosa] = "Depósito borrado exitosamente";
            await unidadTrabajo.Guardar();




            return RedirectToAction("Index");

        }



        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            Deposito deposito = new Deposito();

            if (id == null)
            {
                //crear nuevo deposito
                deposito.Estado = true;
                return View(deposito);
            }

            deposito = await unidadTrabajo.Deposito.Obtener(id.GetValueOrDefault());
            if (deposito == null)
            {
                return NotFound();

            }
            return View(deposito);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Deposito deposito)
        {
            if (ModelState.IsValid)
            {

                if (await ValidarNombre(deposito.Nombre, deposito.Id))
                {

                    TempData[DefinicionesEstaticas.Error] = $"Error: existe un depósito con el nombre {deposito.Nombre}";
                    return View(deposito);

                }

                if (deposito.Id == 0)
                {
                    await unidadTrabajo.Deposito.Agregar(deposito);
                    TempData[DefinicionesEstaticas.Exitosa] = "Depósito creado exitosamente";
                }
                else
                {
                    unidadTrabajo.Deposito.Actualizar(deposito);
                    TempData[DefinicionesEstaticas.Exitosa] = "Depósito actualizado exitosamente";
                }
                await unidadTrabajo.Guardar();
                return RedirectToAction("Index");

            }
            TempData[DefinicionesEstaticas.Error] = "Error al actualizar o crear Depósito";
            return View(deposito);
        }




        public async Task<bool> ValidarNombre(string nombre, int id = 0)
        {

            IEnumerable<Deposito> depositosLista = await unidadTrabajo.Deposito.ObtenerTodos();

            bool validar = false;

            if (id == 0)
            {
                validar = depositosLista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            }
            else
            {
                validar = depositosLista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim() && b.Id != id);
            }

            if (validar == true)
            {
                return true;
            }

            return false;
        }


    }
}
