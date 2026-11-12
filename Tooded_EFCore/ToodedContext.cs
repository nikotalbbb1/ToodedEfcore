using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore
{
    public class ToodedContext : DbContext
    {
        public DbSet<Toode> Toodetabel { get; set; }
        public DbSet<Kategooria> Kategooriatabel { get; set; }
        public DbSet<Kasutaja> Kasutajad { get; set; }
        public DbSet<Klient> Kliendid { get; set; }
        public DbSet<Muuk> Muugid { get; set; }
        public DbSet<MuugiRida> MuugiRead { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\MSSQLLocalDB;Database=ToodedKassa;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kategooria>().ToTable("Kategooriatabel");

            modelBuilder.Entity<Toode>(e =>
            {
                e.ToTable("Toodetabel");
                e.Property(t => t.Hind).HasPrecision(18, 2);
                // Kategooriat ei saa kustutada, kui selles on tooteid (varem kustutas CASCADE ka tooted!)
                e.HasOne(t => t.Kategooria)
                 .WithMany(k => k.Tooded)
                 .HasForeignKey(t => t.KategooriaId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Kasutaja>(e =>
            {
                e.ToTable("Kasutajad");
                e.Property(k => k.Kasutajanimi).HasMaxLength(50);
                e.HasIndex(k => k.Kasutajanimi).IsUnique();
                e.Property(k => k.Roll).HasMaxLength(20);
            });

            modelBuilder.Entity<Klient>(e =>
            {
                e.ToTable("Kliendid");
                e.Property(k => k.Kliendikaart).HasMaxLength(30);
                e.HasIndex(k => k.Kliendikaart).IsUnique();
                e.Property(k => k.Boonus).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Muuk>(e =>
            {
                e.ToTable("Muugid");
                e.Property(m => m.Vahesumma).HasPrecision(18, 2);
                e.Property(m => m.BoonusKasutatud).HasPrecision(18, 2);
                e.Property(m => m.Kokku).HasPrecision(18, 2);
                e.Property(m => m.BoonusSaadud).HasPrecision(18, 2);
                e.HasOne(m => m.Kasutaja).WithMany().HasForeignKey(m => m.KasutajaId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(m => m.Klient).WithMany().HasForeignKey(m => m.KlientId).OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<MuugiRida>(e =>
            {
                e.ToTable("MuugiRead");
                e.Property(r => r.Hind).HasPrecision(18, 2);
                e.HasOne(r => r.Muuk).WithMany(m => m.Read).HasForeignKey(r => r.MuukId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(r => r.Toode).WithMany().HasForeignKey(r => r.ToodeId).OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
