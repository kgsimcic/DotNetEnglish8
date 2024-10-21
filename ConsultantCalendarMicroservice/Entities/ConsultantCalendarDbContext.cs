using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ConsultantCalendarMicroservice.Entities;

public partial class ConsultantCalendarDbContext : DbContext
{
    public ConsultantCalendarDbContext()
    {
    }

    public ConsultantCalendarDbContext(DbContextOptions<ConsultantCalendarDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Consultant> Consultants { get; set; }

    public virtual DbSet<ConsultantCalendar> ConsultantCalendars { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Consultant>(entity =>
        {
            entity.ToTable("Consultant");

            entity.Property(e => e.Fname)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("FName");
            entity.Property(e => e.Lname)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("LName");
            entity.Property(e => e.Speciality)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ConsultantCalendar>(entity =>
        {
            entity.ToTable("ConsultantCalendar");

            entity.Property(e => e.Date).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
