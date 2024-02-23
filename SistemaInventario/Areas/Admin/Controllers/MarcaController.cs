using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SistemaInventario.AccesoDatos.Repositorios.IRepositorios;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.RoleAdmin)]
    public class MarcaController : Controller
    {

        private readonly IUnidadTrabajo unidadTrabajo;



        public MarcaController(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            IEnumerable<Marca> marcasLista = await unidadTrabajo.Marca.ObtenerTodos();

            

            return View(marcasLista);

        }


        [HttpGet]

        public async Task<IActionResult> Delete(int? id)
        {

            Marca marca = await unidadTrabajo.Marca.Obtener(id.GetValueOrDefault());

            if (marca == null)
            {
                return NotFound("Error al borrar o la marca no existe.");
            }

            return View(marca);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            Marca marca = await unidadTrabajo.Marca.Obtener(id);
            unidadTrabajo.Marca.Remover(marca);
            TempData[DS.Exitosa] = "Marca borrada exitosamente";
            await unidadTrabajo.Guardar();

            return RedirectToAction("Index");
        }

       

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            Marca marca = new Marca();

            if (id == null)
            {
                //crear nuevo marca
                marca.Estado = true;
                return View(marca);
            }

            marca = await unidadTrabajo.Marca.Obtener(id.GetValueOrDefault());
            if (marca == null)
            {
                return NotFound();

            }
            return View(marca);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Marca marca)
        {
            if (ModelState.IsValid)
            {
                if (await ValidarNombre(marca.Nombre, marca.Id))
                {

                    TempData[DS.Error] = $"Error: existe una categoría con el nombre {marca.Nombre}";
                    return View(marca);

                }

                if (marca.Id == 0)
                {
                    await unidadTrabajo.Marca.Agregar(marca);
                    TempData[DS.Exitosa] = "Marca creada exitosamente";
                }
                else
                {
                    unidadTrabajo.Marca.Actualizar(marca);
                    TempData[DS.Exitosa] = "Marca actualizada exitosamente";
                }


                await unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));

            }
            TempData[DS.Error] = "Error al actualizar o crear Marca";
            return View(marca);
        }


        public async Task<bool> ValidarNombre(string nombre, int id = 0)
        {

            IEnumerable<Marca> marcasLista = await unidadTrabajo.Marca.ObtenerTodos();

            bool validar = false;

            if (id == 0)
            {
                validar = marcasLista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            }
            else
            {
                validar = marcasLista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim() && b.Id != id);
            }

            if (validar == true)
            {
                return true;
            }

            return false;
        }



    }
}
