using Microsoft.EntityFrameworkCore;


namespace LessonFive.Models
{
    public partial class Context : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public Context() { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine)
                          .UseLazyLoadingProxies()
                          .UseNpgsql("Host=localhost;Username=postgres;Password=example;Database=Chat_v1");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(x => x.Id).HasName("messages_pkey");
                entity.ToTable("messages");
                entity.Property(x => x.Id).HasColumnName("id");
                entity.Property(x => x.Text).HasColumnName("text");
                entity.Property(x => x.FromUserId).HasColumnName("from_user_id");
                entity.Property(x => x.ToUserId).HasColumnName("to_user_id");

                entity.HasOne(d => d.FromUser)
                      .WithMany(p => p.FromMessages)
                      .HasForeignKey(d => d.FromUserId)
                      .HasConstraintName("messages_from_user_id_fkey");

                entity.HasOne(d => d.ToUser)
                      .WithMany(p => p.ToMessages)
                      .HasForeignKey(d => d.ToUserId)
                      .HasConstraintName("messages_to_user_id_fkey");
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id).HasName("users_pkey");
                entity.ToTable("users");
                entity.Property(x => x.Id).HasColumnName("id");
                entity.Property(x => x.Name).HasMaxLength(255).HasColumnName("name");
            });
            base.OnModelCreating(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}
