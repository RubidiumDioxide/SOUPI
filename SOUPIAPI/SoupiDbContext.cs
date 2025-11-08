using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SOUPIShared.Models; 
using Task = SOUPIShared.Models.Task; 


namespace SOUPIAPI;

public partial class SoupiDbContext : DbContext
{
    public SoupiDbContext()
    {
    }

    public SoupiDbContext(DbContextOptions<SoupiDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TeamMember> TeamMembers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Trusted_Connection=True;database=soupi_db;server=(local);Encrypt=false;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NOTIFICA__3214EC07F1801193");

            entity.ToTable("NOTIFICATION");

            entity.HasIndex(e => e.Id, "UQ__NOTIFICA__3214EC06E7C3CC71").IsUnique();

            entity.Property(e => e.Link).HasMaxLength(255);
            entity.Property(e => e.Text).HasMaxLength(255);

            entity.HasOne(d => d.Reciever).WithMany(p => p.NotificationRecievers)
                .HasForeignKey(d => d.RecieverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NOTIFICAT__Recie__1D7B6025");

            entity.HasOne(d => d.Sender).WithMany(p => p.NotificationSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NOTIFICAT__Sende__1E6F845E");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PROJECT__3214EC071F17FB7B");

            entity.ToTable("PROJECT");

            entity.HasIndex(e => e.Id, "UQ__PROJECT__3214EC068EF6EC48").IsUnique();

            entity.HasIndex(e => e.Name, "UQ__PROJECT__737584F6FA07F121").IsUnique();

            entity.Property(e => e.GithubRepository).HasMaxLength(255);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Creator).WithMany(p => p.Projects)
                .HasForeignKey(d => d.CreatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PROJECT__Creator__19AACF41");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TASK__3214EC0746E9210A");

            entity.ToTable("TASK");

            entity.HasIndex(e => e.Id, "UQ__TASK__3214EC06A8795D98").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValue("0");

            entity.HasOne(d => d.Assignee).WithMany(p => p.TaskAssignees)
                .HasForeignKey(d => d.AssigneeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TASK__AssigneeId__2057CCD0");

            entity.HasOne(d => d.Creator).WithMany(p => p.TaskCreators)
                .HasForeignKey(d => d.CreatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TASK__CreatorId__1F63A897");

            entity.HasOne(d => d.ParentTask).WithMany(p => p.InverseParentTask)
                .HasForeignKey(d => d.ParentTaskId)
                .HasConstraintName("FK__TASK__ParentTask__214BF109");

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TASK__ProjectId__22401542");

            entity.HasMany(d => d.PreceedingTasks).WithMany(p => p.SubsequentTasks)
                .UsingEntity<Dictionary<string, object>>(
                    "TaskSequence",
                    r => r.HasOne<Task>().WithMany()
                        .HasForeignKey("PreceedingTaskId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__TASK_SEQU__Prece__2334397B"),
                    l => l.HasOne<Task>().WithMany()
                        .HasForeignKey("SubsequentTaskId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__TASK_SEQU__Subse__24285DB4"),
                    j =>
                    {
                        j.HasKey("PreceedingTaskId", "SubsequentTaskId").HasName("PK__TASK_SEQ__4D84655AC93CD2B9");
                        j.ToTable("TASK_SEQUENCE");
                    });

            entity.HasMany(d => d.SubsequentTasks).WithMany(p => p.PreceedingTasks)
                .UsingEntity<Dictionary<string, object>>(
                    "TaskSequence",
                    r => r.HasOne<Task>().WithMany()
                        .HasForeignKey("SubsequentTaskId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__TASK_SEQU__Subse__24285DB4"),
                    l => l.HasOne<Task>().WithMany()
                        .HasForeignKey("PreceedingTaskId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__TASK_SEQU__Prece__2334397B"),
                    j =>
                    {
                        j.HasKey("PreceedingTaskId", "SubsequentTaskId").HasName("PK__TASK_SEQ__4D84655AC93CD2B9");
                        j.ToTable("TASK_SEQUENCE");
                    });
        });

        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TEAM_MEM__3214EC07ECEA17CC");

            entity.ToTable("TEAM_MEMBER");

            entity.HasIndex(e => e.Id, "UQ__TEAM_MEM__3214EC06D6FDDC35").IsUnique();

            entity.Property(e => e.Role).HasMaxLength(255);

            entity.HasOne(d => d.Manager).WithMany(p => p.TeamMemberManagers)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK__TEAM_MEMB__Manag__1C873BEC");

            entity.HasOne(d => d.Project).WithMany(p => p.TeamMembers)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TEAM_MEMB__Proje__1B9317B3");

            entity.HasOne(d => d.Used).WithMany(p => p.TeamMemberUseds)
                .HasForeignKey(d => d.UsedId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TEAM_MEMB__UsedI__1A9EF37A");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__USER__3214EC0717B947BF");

            entity.ToTable("USER");

            entity.HasIndex(e => e.Id, "UQ__USER__3214EC065F3239DF").IsUnique();

            entity.HasIndex(e => e.Login, "UQ__USER__737584F663513405").IsUnique();

            entity.Property(e => e.Login).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
