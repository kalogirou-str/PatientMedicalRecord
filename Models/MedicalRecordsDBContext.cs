using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PatientMedicalRecord.Models;

public partial class MedicalRecordsDBContext : DbContext
{
    public MedicalRecordsDBContext()
    {
    }

    public MedicalRecordsDBContext(DbContextOptions<MedicalRecordsDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-N0QM39O5;Database=MedicalRecordsDB;Trusted_Connection=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admins__719FE4E82D97A655");

            entity.HasOne(d => d.UsernameNavigation).WithMany(p => p.Admins)
                .HasPrincipalKey(p => p.Username)
                .HasForeignKey(d => d.Username)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Admins__Username__70DDC3D8");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => new { e.PatientId, e.DoctorId, e.AppointmentDate }).HasName("PK__Appointm__FB3EAFC70F094A65");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Docto__6A30C649");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__Patie__693CA210");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__Doctors__2DC00EDF5A767AD4");

            entity.HasOne(d => d.UsernameNavigation).WithMany(p => p.Doctors)
                .HasPrincipalKey(p => p.Username)
                .HasForeignKey(d => d.Username)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Doctors__Usernam__6383C8BA");
        });

        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("PK__MedicalR__FBDF78C912BE5B0A");

            entity.HasOne(d => d.Doctor).WithMany(p => p.MedicalRecords).HasConstraintName("FK__MedicalRe__Docto__6E01572D");

            entity.HasOne(d => d.Patient).WithMany(p => p.MedicalRecords).HasConstraintName("FK__MedicalRe__Patie__6D0D32F4");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patient__970EC346E1A6A795");

            entity.HasOne(d => d.UsernameNavigation).WithMany(p => p.Patients)
                .HasPrincipalKey(p => p.Username)
                .HasForeignKey(d => d.Username)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Patient__Usernam__66603565");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC45CBE536");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
