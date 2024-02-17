using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Server.App.Db.Contexts;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Descipline> Desciplines { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserCredential> UserCredentials { get; set; }

    public virtual DbSet<UserGroup> UserGroups { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning
        => optionsBuilder.UseNpgsql("PLACE YOUR CONNECTION STRING HERE");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Descipline>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("desciplines_pkey");

            entity.ToTable("desciplines");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Name).HasColumnName("name");

            entity.HasOne(d => d.Group).WithMany(p => p.Desciplines)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("desciplines_group_id_fkey");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("groups_pkey");

            entity.ToTable("groups");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tasks_pkey");

            entity.ToTable("tasks");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.Created)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created");
            entity.Property(e => e.Deadline)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deadline");
            entity.Property(e => e.DesciplineId).HasColumnName("descipline_id");
            entity.Property(e => e.WhoAdded).HasColumnName("who_added");

            entity.HasOne(d => d.Descipline).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.DesciplineId)
                .HasConstraintName("tasks_descipline_id_fkey");

            entity.HasOne(d => d.WhoAddedNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.WhoAdded)
                .HasConstraintName("tasks_who_added_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<UserCredential>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_credentials_pkey");

            entity.ToTable("user_credentials");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.LoginHash).HasColumnName("login_hash");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");

            entity.HasOne(d => d.User).WithOne(p => p.UserCredential)
                .HasForeignKey<UserCredential>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_credentials_user_id_fkey");
        });

        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("user_group");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IsOwner).HasColumnName("is_owner");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Group).WithMany()
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_group_group_id_fkey");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_group_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
