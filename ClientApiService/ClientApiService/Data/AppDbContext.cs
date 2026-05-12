namespace ClientApiService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<RequestResult> RequestResults { get; set; }

        public DbSet<Lead> Leads { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<RequestResult>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Client).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Kladr).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ResponseDate).IsRequired();
            });

            // (конфигурация Lead):
            modelBuilder.Entity<Lead>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasDefaultValue("Pending");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}   