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
    public class DepositoProductoConfiguracion : IEntityTypeConfiguration<DepositoProducto>
    {
        public void Configure(EntityTypeBuilder<DepositoProducto> builder)
        {
            builder.Property(x => x.Id).IsRequired();
           builder.Property(x => x.Cantidad).IsRequired();

            //no se pone la navegacion, solo la propiedad

            builder.Property(x => x.ProductoId).IsRequired();
            builder.Property(x => x.DepositoId).IsRequired();
          

            //relaciones, navegacion de uno a muchos.

            builder.HasOne(x => x.Deposito).WithMany().HasForeignKey(x => x.DepositoId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Producto).WithMany().HasForeignKey(x => x.ProductoId).OnDelete(DeleteBehavior.NoAction);
            


        }
    }

    
}
