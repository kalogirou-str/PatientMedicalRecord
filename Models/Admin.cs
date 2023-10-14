using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PatientMedicalRecord.Models;

public partial class Admin
{
    [Key]
    [Column("AdminID")]
    public int AdminId { get; set; }

    [StringLength(50)]
    public string Username { get; set; } = null!;

    public virtual User UsernameNavigation { get; set; } = null!;
}
