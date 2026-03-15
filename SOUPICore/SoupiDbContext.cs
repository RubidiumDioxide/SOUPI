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
    public virtual DbSet<Job> Jobs { get; set; } 
    public virtual DbSet<JobSequence> JobSequences { get; set; } 
    public virtual DbSet<Assignment> Assignments { get; set; } 
    public virtual DbSet<Activity> Activities { get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity => 
        {
            entity.HasKey(p => p.Id); 

            entity.ToTable("PROJECT");

            entity.HasIndex(p => p.Id).IsUnique();
            entity.HasIndex(p => new { p.CreatorId, p.Name }).IsUnique();

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
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.Id);

            entity.ToTable("NOTIFICATION");

            entity.HasIndex(n => n.Id).IsUnique();
            
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
            entity.HasKey(tm => tm.Id); 

            entity.ToTable("TEAMMEMBER");

            entity.HasIndex(tm => tm.Id).IsUnique(); 
            entity.HasIndex(tm => new { tm.UserId, tm.ProjectId }).IsUnique();

            entity.HasOne(tm => tm.User).WithMany(u => u.TeamMembers)
                .HasForeignKey(tm => tm.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(tm => tm.Project).WithMany(p => p.TeamMembers)
                .HasForeignKey(tm => tm.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(tm => tm.Supervisor).WithMany(tm => tm.Subservient)
                .HasForeignKey(tm => tm.SupervisorId) 
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(j => j.Id);

            entity.ToTable("JOB");

            entity.HasIndex(j => j.Id).IsUnique(); 
            
            entity.HasOne(j => j.Project).WithMany(p => p.Jobs)
                .HasForeignKey(j => j.ProjectId)
                .OnDelete(DeleteBehavior.Restrict); 
            entity.HasOne(j => j.Creator).WithMany(u => u.CreatedJobs)
                .HasForeignKey(j => j.CreatorId) 
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(j => j.ParentJob).WithMany(t => t.ChildJobs)
                .HasForeignKey(j => j.ParentJobId) 
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.ToTable("ASSIGNMENT");

            entity.HasIndex(a => a.Id).IsUnique(); 
            entity.HasIndex(a => new { a.TeamMemberId, a.JobId }).IsUnique(); 

            entity.HasOne(a => a.TeamMember).WithMany(tm => tm.Assignments)
                .HasForeignKey(a => a.TeamMemberId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(a => a.Job).WithMany(tm => tm.Assignments)
                .HasForeignKey(a => a.JobId) 
                .OnDelete(DeleteBehavior.Restrict); 
        });

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.ToTable("ACTIVITY");

            entity.HasIndex(a => a.Id).IsUnique();

            entity.HasOne(a => a.Assignment).WithMany(a => a.Activities) 
                .HasForeignKey(a => a.AssignmentId) 
                .OnDelete(DeleteBehavior.Restrict); 
        });

        modelBuilder.Entity<JobSequence>(entity =>
        {
            entity.HasKey(js => js.Id);

            entity.ToTable("JOBSEQUENCE");

            entity.HasIndex(js => new { js.FirstJobId, js.SecondJobId }).IsUnique();

            entity.HasOne(js => js.FirstJob).WithMany(j => j.NextJobSequences)
                .HasForeignKey(js => js.FirstJobId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(js => js.SecondJob).WithMany(j => j.PreviousJobSequences)
                .HasForeignKey(js => js.SecondJobId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Project>()
            .Property(i => i.CreationDateTime)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<Notification>()
            .Property(i => i.CreationDateTime)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<Job>()
            .Property(i => i.CreationDateTime)
            .HasDefaultValueSql("GETDATE()");
    }
}
