using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Proyec_tecn.Moldels;

namespace Proyec_tecn.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<CompraCab> CompraCabs { get; set; }
        public DbSet<CompraDet> CompraDets { get; set; }
        public DbSet<VentaCab> VentaCabs { get; set; }
        public DbSet<VentaDet> VentaDets { get; set; }
        public DbSet<MovimientoCab> MovimientoCabs { get; set; }
        public DbSet<MovimientoDet> MovimientoDets { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de precisión para decimales

            modelBuilder.Entity<Producto>()
                .Property(p => p.Costo)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Producto>()
                .Property(p => p.PrecioVenta)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CompraCab>()
                .Property(c => c.SubTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CompraCab>()
                .Property(c => c.Igv)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CompraCab>()
                .Property(c => c.Total)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CompraDet>()
                .Property(cd => cd.Precio)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CompraDet>()
                .Property(cd => cd.Sub_Total)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CompraDet>()
                .Property(cd => cd.Igv)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CompraDet>()
                .Property(cd => cd.Total)
                .HasPrecision(18, 2);

            // Configuraciones similares para VentaCab y VentaDet...

            modelBuilder.Entity<VentaCab>()
            .Property(v => v.SubTotal)
            .HasPrecision(18, 2);

            modelBuilder.Entity<VentaCab>()
                .Property(v => v.Igv)
                .HasPrecision(18, 2);

            modelBuilder.Entity<VentaCab>()
                .Property(v => v.Total)
                .HasPrecision(18, 2);

            // Configuraciones para VentaDet

            modelBuilder.Entity<VentaDet>()
                .Property(vd => vd.Precio)
                .HasPrecision(18, 2);

            modelBuilder.Entity<VentaDet>()
                .Property(vd => vd.Sub_Total)
                .HasPrecision(18, 2);

            modelBuilder.Entity<VentaDet>()
                .Property(vd => vd.Igv)
                .HasPrecision(18, 2);

            modelBuilder.Entity<VentaDet>()
                .Property(vd => vd.Total)
                .HasPrecision(18, 2);




            // Relaciones
            modelBuilder.Entity<CompraDet>()
                .HasOne(cd => cd.CompraCab)
                .WithMany(cc => cc.CompraDets)
                .HasForeignKey(cd => cd.Id_CompraCab);

            modelBuilder.Entity<CompraDet>()
                .HasOne(cd => cd.Producto)
                .WithMany(p => p.CompraDets)
                .HasForeignKey(cd => cd.Id_producto);

            modelBuilder.Entity<VentaDet>()
                .HasOne(vd => vd.VentaCab)
                .WithMany(vc => vc.VentaDets)
                .HasForeignKey(vd => vd.Id_VentaCab);

            modelBuilder.Entity<VentaDet>()
                .HasOne(vd => vd.Producto)
                .WithMany(p => p.VentaDets)
                .HasForeignKey(vd => vd.Id_producto);

            modelBuilder.Entity<MovimientoDet>()
                .HasOne(md => md.MovimientoCab)
                .WithMany(mc => mc.MovimientoDets)
                .HasForeignKey(md => md.Id_movimientocab);

            modelBuilder.Entity<MovimientoDet>()
                .HasOne(md => md.Producto)
                .WithMany(p => p.MovimientoDets)
                .HasForeignKey(md => md.Id_Producto);

            // Datos semilla
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Usuario por defecto
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin123",
                    Email = "admin@empresa.com",
                    FechaCreacion = DateTime.Now,
                    Activo = true
                }
            );
        }
    }
}

