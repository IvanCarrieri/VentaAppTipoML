using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Modelos;
using System.Reflection;

namespace SistemaInventario.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //El modelo deposito se agrega al dbcontext como una propiedad DbSet de tipo Deposito con nombre Depositos
        public DbSet<Deposito> Depositos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Producto> Productos { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<DepositoProducto> DepositoProductos { get; set; }

        public DbSet<Inventario> Inventarios { get; set; }

        public DbSet<InventarioDetalle> InventarioDetalles { get; set; }

        public DbSet<Kardex> Kardexs { get; set; }

        public DbSet<Empresa> Empresas { get; set; }

        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<Orden> Ordenes { get; set; }

        public DbSet<OrdenDetalle> OrdenesDetalles { get; set; }


        //aplicamos el fluent API antes de migrar los modelos(Clases de modelo)
        //sobreescribimos un metodo que ya existe en ApplicationDbcontext 
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}