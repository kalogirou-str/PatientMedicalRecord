using System.ComponentModel.DataAnnotations;

namespace PatientMedicalRecord.Models
{
    public class MedicalRecordViewModel
    {
        [Required(ErrorMessage = "Please enter a valid date.")]
        [DataType(DataType.Date)]
        public DateTime DateOfVisit { get; set; }

        [Required(ErrorMessage = "Diagnosis is required.")]
        [StringLength(255, ErrorMessage = "Diagnosis must not exceed 255 characters.")]
        public string Diagnosis { get; set; }

        [Required(ErrorMessage = "Treatment is required.")]
        [StringLength(255, ErrorMessage = "Treatment must not exceed 255 characters.")]
        public string Treatment { get; set; }

        [StringLength(255, ErrorMessage = "Prescription must not exceed 255 characters.")]
        public string Prescription { get; set; }

        [Required(ErrorMessage = "Medical Record Number is required.")]
        [StringLength(20, ErrorMessage = "Medical Record Number must not exceed 20 characters.")]
        public string MedicalRecordNumber { get; set; }
    }

}
