using System;
using System.ComponentModel.DataAnnotations;

public class PatientEditViewModel
{
    public int PatientID { get; set; }
    public string Username { get; set; }

    [Required(ErrorMessage = "Medical Record Number is required.")]
    [Display(Name = "Medical Record Number")]
    public string MedicalRecordNumber { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Gender is required.")]
    public string Gender { get; set; }

    [Required(ErrorMessage = "First Name is required.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is required.")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits.")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    public string Address { get; set; }
}
