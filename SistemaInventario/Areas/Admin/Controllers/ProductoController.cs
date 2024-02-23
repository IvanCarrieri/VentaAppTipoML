using Microsoft.AspNetCore.Mvc;

using SistemaInventario.AccesoDatos.Repositorios.IRepositorios;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.RoleAdmin + "," + DS.RoleInventario )]
    public class ProductoController : Controller
    {

        private readonly IUnidadTrabajo unidadTrabajo;
        private readonly IWebHostEnvironment webHostEnvironment;  //para acceder al root donde estan las imagenes


        public ProductoController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment webHostEnvironment)
        {
            this.unidadTrabajo = unidadTrabajo;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            IEnumerable<Producto> productosLista = await unidadTrabajo.Producto.ObtenerTodos(incluirPropiedades: "Categoria,Marca");
            return View(productosLista);
        }




        [HttpGet]    //que me llama a la vista
        public async Task<IActionResult> Upsert(int? id)
        {
            //asi inicializo una clase donde debo inicializar dentro propiedades de tipo clase o tipi list o IEnumerable
            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto(),
                CategoriasLista = unidadTrabajo.Producto.ObtenerTodosDropDownList("Categoria"),
                MarcasLista = unidadTrabajo.Producto.ObtenerTodosDropDownList("Marca"),
                PadresLista = unidadTrabajo.Producto.ObtenerTodosDropDownList("Producto")

            };

            if (id == null)
            {
                //crear nuevo producto
                productoVM.Producto.Estado = true;
                return View(productoVM);
            }

            productoVM.Producto = await unidadTrabajo.Producto.Obtener(id.GetValueOrDefault());


            if (productoVM.Producto == null)
            {
                return NotFound();

            }
            return View(productoVM);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductoVM productoVM)
        {

            if (ModelState.IsValid)
            {
                var archivo = HttpContext.Request.Form.Files;
                string webRuta = webHostEnvironment.WebRootPath;



                if (productoVM.Producto.Id == 0)
                {
                    //crear nuevo producto

                    //guardar imagen en aplicacion 
                    string upload = webRuta + DS.ImagenRuta;
                    string nombreArchivo = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(archivo[0].FileName);

                    using
                    (var archivoAGrabar = new FileStream(Path.Combine(upload, nombreArchivo + extension), FileMode.Create))
                    {
                        archivo[0].CopyTo(archivoAGrabar);
                    }



                    productoVM.Producto.ImagenUrl = nombreArchivo + extension;

                    await unidadTrabajo.Producto.Agregar(productoVM.Producto);
                    TempData[DS.Exitosa] = "Producto creado exitosamente";
                }
                else
                {

                    //actualizar
                    Producto productoObj = await unidadTrabajo.Producto.ObtenerPrimero(p => p.Id == productoVM.Producto.Id, isTracking: false);

                    if (archivo.Count > 0)
                    {
                        string upload = webRuta + DS.ImagenRuta;
                        string nombreArchivo = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(archivo[0].FileName);

                        //borro imagen anterior
                        var archivoAnterior = Path.Combine(upload, productoObj.ImagenUrl);
                        if (System.IO.File.Exists(archivoAnterior))
                        {
                            System.IO.File.Delete(archivoAnterior);
                        }

                        using
                       (var archivoAGrabar = new FileStream(Path.Combine(upload, nombreArchivo + extension), FileMode.Create))
                        {
                            archivo[0].CopyTo(archivoAGrabar);
                        }

                        productoVM.Producto.ImagenUrl = nombreArchivo + extension;

                    }// caso contrario 
                    else
                    {
                        productoVM.Producto.ImagenUrl = productoObj.ImagenUrl;
                    }

                    unidadTrabajo.Producto.Actualizar(productoVM.Producto);
                    TempData[DS.Exitosa] = "Producto actualizado exitosamente";
                }



                if (await ValidarNumeroSerie(productoVM.Producto.NumeroSerie, productoVM.Producto.Id))
                {

                    TempData[DS.Error] = $"Error: existe un producto con el número de serie {productoVM.Producto.NumeroSerie}";
                    return View(productoVM);

                }

                await unidadTrabajo.Guardar();
                //para que se ejecute el IActionResult del index
                return RedirectToAction("Index");
            }


            TempData[DS.Error] = "Error al actualizar o crear un Producto";
            productoVM.CategoriasLista = unidadTrabajo.Producto.ObtenerTodosDropDownList("Categoria");
            productoVM.MarcasLista = unidadTrabajo.Producto.ObtenerTodosDropDownList("Marca");
            productoVM.PadresLista = unidadTrabajo.Producto.ObtenerTodosDropDownList("Producto");


            return View(productoVM);
        }





        public async Task<bool> ValidarNumeroSerie(string numeroSerie, int id = 0)
        {

            IEnumerable<Producto> productosLista = await unidadTrabajo.Producto.ObtenerTodos();

            bool validar = false;

            if (id == 0)
            {
                validar = productosLista.Any(b => b.NumeroSerie.ToLower().Trim() == numeroSerie.ToLower().Trim());
            }
            else
            {
                validar = productosLista.Any(b => b.NumeroSerie.ToLower().Trim() == numeroSerie.ToLower().Trim() && b.Id != id);
            }

            if (validar == true)
            {
                return true;
            }

            return false;
        }

        [HttpGet]

        public async Task<IActionResult> Delete(int? id)
        {

            Producto producto = await unidadTrabajo.Producto.Obtener(id.GetValueOrDefault());

            if (producto == null)
            {
                return NotFound("Error al borrar o el producto no existe.");
            }

            return View(producto);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            Producto producto = await unidadTrabajo.Producto.Obtener(id);

            string upload = webHostEnvironment.WebRootPath + DS.ImagenRuta;  //obtiene toda la ruta donde estan las imagenes
            var archivo = Path.Combine(upload, producto.ImagenUrl);

            if(System.IO.File.Exists(archivo))
            {
                System.IO.File.Delete(archivo);
            }

            unidadTrabajo.Producto.Remover(producto);
            TempData[DS.Exitosa] = "Producto borrado exitosamente";
            await unidadTrabajo.Guardar();

            return RedirectToAction("Index");
        }





    }
}
