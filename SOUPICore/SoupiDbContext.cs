using Microsoft.EntityFrameworkCore;
using SOUPIShared.Models; 


namespace SOUPICore;

public partial class SoupiDbContext : DbContext
{
    public SoupiDbContext()
    {
    }

    public SoupiDbContext(DbContextOptions<SoupiDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Project> Projects { get; set; } 
    public virtual DbSet<User> Users { get; set; } 
    public virtual DbSet<Notification> Notifications { get; set; } 
    public virtual DbSet<TeamMember> TeamMembers { get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(p => p.Id); 

            entity.ToTable("PROJECT");

            entity.HasIndex(p => p.Id).IsUnique();
            entity.HasIndex(p => new { p.CreatorId, p.Name }).IsUnique();

            entity.Property(p => p.GithubRepository).HasMaxLength(255);
            entity.Property(p => p.Image).HasMaxLength(255);
            entity.Property(p => p.Name).HasMaxLength(100);
            entity.Property(p => p.Description).HasMaxLength(255);

            entity.HasOne(p => p.Creator).WithMany(u => u.Projects)
                .HasForeignKey(p => p.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.ToTable("USER");

            entity.HasIndex(u => u.Id).IsUnique();
            entity.HasIndex(u => u.Login).IsUnique();

            entity.Property(e => e.Login).HasMaxLength(255);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.Id);

            entity.ToTable("NOTIFICATION");

            entity.HasIndex(n => n.Id).IsUnique();
            
            entity.Property(n => n.Message).HasMaxLength(255);

            entity.HasOne(n => n.Sender).WithMany(u => u.SentNotifications)
                .HasForeignKey(n => n.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(n => n.Receiver).WithMany(u => u.ReceivedNotifications)
                .HasForeignKey(n => n.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(n => n.Project).WithMany(p => p.Notifications)
                .HasForeignKey(n => n.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(tm => new { tm.UserId, tm.ProjectId });

            entity.ToTable("TEAMMEMBER");

            entity.HasIndex(tm => new { tm.UserId, tm.ProjectId }).IsUnique();

            entity.Property(tm => tm.Role).HasMaxLength(255);

            entity.HasOne(tm => tm.User).WithMany(u => u.TeamMembers)
                .HasForeignKey(tm => tm.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(tm => tm.Project).WithMany(p => p.TeamMembers)
                .HasForeignKey(tm => tm.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(tm => tm.Supervisor).WithMany(tm => tm.Subservient)
                .HasForeignKey(tm => new { tm.SupervisorUserId, tm.SupervisorProjectId})
                .OnDelete(DeleteBehavior.Restrict);
        }); 

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
