using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public class AdminCreatePatientViewModel
{
    [Required(ErrorMessage = "First Name is required.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is required.")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    [Remote("IsUsernameAvailable", "Validation", ErrorMessage = "This username is already in use.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Medical Record Number is required.")]
    [Display(Name = "Medical Record Number")]
    public string MedicalRecordNumber { get; set; }

    [Required(ErrorMessage = "Date of Birth is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Gender is required.")]
    public string Gender { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [Remote("IsEmailAvailable", "Validation", ErrorMessage = "This email is already in use.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits.")]
    [Remote("IsPhoneAvailable", "Validation", ErrorMessage = "This phone number is already in use.")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    public string Address { get; set; }

}
