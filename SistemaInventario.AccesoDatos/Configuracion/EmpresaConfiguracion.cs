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
    public class EmpresaConfiguracion : IEntityTypeConfiguration<Empresa>
    {
        public void Configure(EntityTypeBuilder<Empresa> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.Nombre).IsRequired().HasMaxLength(80);
            builder.Property(x => x.Descripcion).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Pais).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Ciudad).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Direccion).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Telefono).IsRequired().HasMaxLength(80);
            builder.Property(x => x.FechaCreacion);
            builder.Property(x => x.FechaActualizacion); 


            //no se pone la navegacion, solo la propiedad

            builder.Property(x => x.DepositoVentaId).IsRequired();
            builder.Property(x => x.CreadoPorUsuarioId).IsRequired(false);
            builder.Property(x => x.ActualizadoPorUsuarioId).IsRequired(false); 
            

            //relaciones, navegacion de uno a muchos.

            builder.HasOne(x => x.Deposito).WithMany().HasForeignKey(x => x.DepositoVentaId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.CreadoPor).WithMany().HasForeignKey(x => x.CreadoPorUsuarioId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.ActualizadoPor).WithMany().HasForeignKey(x => x.ActualizadoPorUsuarioId).OnDelete(DeleteBehavior.NoAction);


        }
    }

    
}
