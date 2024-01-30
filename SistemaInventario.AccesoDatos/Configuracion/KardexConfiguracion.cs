using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaInventario.Modelos;

namespace SistemaInventario.AccesoDatos.Configuracion
{
    public class KardexConfiguracion : IEntityTypeConfiguration<Kardex>
    {
        public void Configure(EntityTypeBuilder<Kardex> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.DepositoProductoId).IsRequired();
            builder.Property(x => x.Tipo).IsRequired();
            builder.Property(x => x.Detalle).IsRequired();
            builder.Property(x => x.StockAnterior).IsRequired();
            builder.Property(x => x.Cantidad).IsRequired();
            builder.Property(x => x.Costo).IsRequired();
            builder.Property(x => x.Stock).IsRequired();
            builder.Property(x => x.Total).IsRequired();
            builder.Property(x => x.UsuarioId).IsRequired();
            builder.Property(x => x.FechaRegistro).IsRequired();


            builder.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.DepositoProducto).WithMany().HasForeignKey(x => x.DepositoProductoId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}




