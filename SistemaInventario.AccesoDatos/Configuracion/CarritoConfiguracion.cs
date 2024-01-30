using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaInventario.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Configuracion
{
    public class CarritoConfiguracion : IEntityTypeConfiguration<Carrito>
    {
        public void Configure(EntityTypeBuilder<Carrito> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.UsuarioId).IsRequired();
            builder.Property(x => x.ProductoId).IsRequired();
            builder.Property(x => x.Cantidad).IsRequired();
 

            //relaciones, navegacion de uno a muchos.

            builder.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Producto).WithMany().HasForeignKey(x => x.ProductoId).OnDelete(DeleteBehavior.NoAction);

           


        }
    }

    
}
