using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PatientMedicalRecord.Models;

public partial class Doctor
{
    [Key]
    [Column("DoctorID")]
    public int DoctorId { get; set; }

    [StringLength(50)]
    public string? Specialization { get; set; }

    [StringLength(20)]
    public string? LicenseNumber { get; set; }

    [StringLength(50)]
    public string Username { get; set; } = null!;

    [InverseProperty("Doctor")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [InverseProperty("Doctor")]
    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public virtual User UsernameNavigation { get; set; } = null!;
}
