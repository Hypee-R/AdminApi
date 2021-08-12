using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CorePMSAdmin.Models
{
    public partial class BDContext : DbContext
    {

        public BDContext()
        {
        }

        public BDContext(DbContextOptions<BDContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Usuarios> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuarios>(entity =>
            {
                entity.HasIndex(e => e.Correo)
                    .HasName("UQ__Usuarios__60695A1939F88B85")
                    .IsUnique();

                entity.Property(e => e.Contrasena)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Correo)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Estatus).HasDefaultValueSql("((1))");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("datetime")
                    .HasComputedColumnSql("(dateadd(minute,(5),'20210327'))");

                entity.Property(e => e.Sexo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Token)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Usuario)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });
        }
    }
}
