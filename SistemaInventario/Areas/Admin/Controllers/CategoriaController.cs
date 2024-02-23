using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SistemaInventario.AccesoDatos.Repositorios.IRepositorios;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.RoleAdmin)]
    public class CategoriaController : Controller
    {

        private readonly IUnidadTrabajo unidadTrabajo;



        public CategoriaController(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            IEnumerable<Categoria> categoriasLista = await unidadTrabajo.Categoria.ObtenerTodos();

            

            return View(categoriasLista);

        }


        [HttpGet]

        public async Task<IActionResult> Delete(int? id)
        {

            Categoria categoria = await unidadTrabajo.Categoria.Obtener(id.GetValueOrDefault());

            if (categoria == null)
            {
                return NotFound("Error al borrar o la categoría no existe.");
            }

            return View(categoria);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            Categoria categoria = await unidadTrabajo.Categoria.Obtener(id);
            unidadTrabajo.Categoria.Remover(categoria);
            TempData[DS.Exitosa] = "Categoría borrada exitosamente";
            await unidadTrabajo.Guardar();

            return RedirectToAction("Index");
        }

       

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            Categoria categoria = new Categoria();

            if (id == null)
            {
                //crear nuevo deposito
                categoria.Estado = true;
                return View(categoria);
            }

            categoria = await unidadTrabajo.Categoria.Obtener(id.GetValueOrDefault());
            if (categoria == null)
            {
                return NotFound();

            }
            return View(categoria);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                if (await ValidarNombre(categoria.Nombre, categoria.Id))
                {

                    TempData[DS.Error] = $"Error: existe una categoría con el nombre {categoria.Nombre}";
                    return View(categoria);

                }

                if (categoria.Id == 0)
                {
                    await unidadTrabajo.Categoria.Agregar(categoria);
                    TempData[DS.Exitosa] = "Categoría creada exitosamente";
                }
                else
                {
                    unidadTrabajo.Categoria.Actualizar(categoria);
                    TempData[DS.Exitosa] = "Categoría actualizada exitosamente";
                }


                await unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));

            }
            TempData[DS.Error] = "Error al actualizar o crear Depósito";
            return View(categoria);
        }


        public async Task<bool> ValidarNombre(string nombre, int id = 0)
        {

            IEnumerable<Categoria> categoriasLista = await unidadTrabajo.Categoria.ObtenerTodos();

            bool validar = false;

            if (id == 0)
            {
                validar = categoriasLista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            }
            else
            {
                validar = categoriasLista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim() && b.Id != id);
            }

            if (validar == true)
            {
                return true;
            }

            return false;
        }









        //#endregion



    }
}
