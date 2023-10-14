using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PatientMedicalRecord.Models;

[PrimaryKey("PatientId", "DoctorId", "AppointmentDate")]
public partial class Appointment
{
    [Key]
    [Column("PatientID")]
    public int PatientId { get; set; }

    [Key]
    [Column("DoctorID")]
    public int DoctorId { get; set; }

    [Key]
    [Column(TypeName = "datetime")]
    public DateTime AppointmentDate { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    [StringLength(255)]
    public string? Notes { get; set; }

    [ForeignKey("DoctorId")]
    [InverseProperty("Appointments")]
    public virtual Doctor Doctor { get; set; } = null!;

    [ForeignKey("PatientId")]
    [InverseProperty("Appointments")]
    public virtual Patient Patient { get; set; } = null!;
}
