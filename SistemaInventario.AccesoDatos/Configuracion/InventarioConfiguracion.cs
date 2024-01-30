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
    public class InventarioConfiguracion : IEntityTypeConfiguration<Inventario>
    {
        public void Configure(EntityTypeBuilder<Inventario> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.UsuarioId).IsRequired();
            builder.Property(x => x.FechaInicial).IsRequired();
            builder.Property(x => x.FechaFinal).IsRequired();
            builder.Property(x => x.DepositoId).IsRequired();
            builder.Property(x => x.Estado).IsRequired();


            builder.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Deposito).WithMany().HasForeignKey(x => x.DepositoId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}




