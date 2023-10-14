using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PatientMedicalRecord.Models;

public partial class MedicalRecord
{
    [Key]
    [Column("RecordID")]
    public int RecordId { get; set; }

    [Column("PatientID")]
    public int? PatientId { get; set; }

    [Column("DoctorID")]
    public int? DoctorId { get; set; }

    [Column(TypeName = "date")]
    public DateTime? DateOfVisit { get; set; }

    [StringLength(255)]
    public string? Diagnosis { get; set; }

    [StringLength(255)]
    public string? Treatment { get; set; }

    [StringLength(255)]
    public string? Prescription { get; set; }

    [ForeignKey("DoctorId")]
    [InverseProperty("MedicalRecords")]
    public virtual Doctor? Doctor { get; set; }

    [ForeignKey("PatientId")]
    [InverseProperty("MedicalRecords")]
    public virtual Patient? Patient { get; set; }
}
