using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SOUPIShared.Models; 


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

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id); 

            entity.ToTable("PROJECT");

            entity.HasIndex(e => e.Id).IsUnique();

            entity.HasIndex(e => e.Name).IsUnique();

            entity.Property(e => e.GithubRepository).HasMaxLength(255);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);

            entity.HasOne(d => d.Creator).WithMany(p => p.Projects)
                .HasForeignKey(d => d.CreatorId)
                .OnDelete(DeleteBehavior.ClientSetNull); 
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("USER");

            entity.HasIndex(e => e.Id).IsUnique();

            entity.HasIndex(e => e.Login).IsUnique();

            entity.Property(e => e.Login).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
