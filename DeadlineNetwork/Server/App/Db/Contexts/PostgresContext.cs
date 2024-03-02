using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Server.App.Db.Contexts;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Descipline> Desciplines { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserCredential> UserCredentials { get; set; }

    public virtual DbSet<UserGroup> UserGroups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Descipline>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("desciplines_pkey");

            entity.ToTable("desciplines");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comment).HasColumnName("comment").IsRequired();
            entity.Property(e => e.GroupId)
                .ValueGeneratedOnAdd()
                .HasColumnName("group_id");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired();

            entity.HasOne(d => d.Group).WithMany(p => p.Desciplines)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("desciplines_group_id_fkey");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("groups_pkey");

            entity.ToTable("groups");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired();
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").IsRequired();
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tasks_pkey");

            entity.ToTable("tasks");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comment).HasColumnName("comment").IsRequired();
            entity.Property(e => e.Created)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created").IsRequired();
            entity.Property(e => e.Deadline)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deadline").IsRequired();
            entity.Property(e => e.DesciplineId)
                .ValueGeneratedOnAdd()
                .HasColumnName("descipline_id");
            entity.Property(e => e.WhoAdded)
                .ValueGeneratedOnAdd()
                .HasColumnName("who_added");

            entity.HasOne(d => d.Descipline).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.DesciplineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tasks_descipline_id_fkey");

            entity.HasOne(d => d.WhoAddedNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.WhoAdded)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tasks_who_added_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired();
        });

        modelBuilder.Entity<UserCredential>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_credentials_pkey");

            entity.ToTable("user_credentials");

            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd()
                .HasColumnName("user_id");
            entity.Property(e => e.LoginHash).HasColumnName("login_hash")
                .IsRequired();
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash")
                .IsRequired();

            entity.HasOne(d => d.User).WithOne(p => p.UserCredential)
                .HasForeignKey<UserCredential>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_credentials_user_id_fkey");
        });

        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity
                .HasKey(e=>new{e.UserId,e.GroupId});
            entity
                .ToTable("user_group");

            entity.Property(e => e.GroupId)
                .ValueGeneratedOnAdd()
                .HasColumnName("group_id");
            entity.Property(e => e.IsOwner).HasColumnName("is_owner")
                .IsRequired()
                .HasDefaultValue(false);
            
            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd()
                .HasColumnName("user_id");

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
