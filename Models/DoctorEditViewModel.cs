using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PatientMedicalRecord.Models
{
    public class DoctorEditViewModel
    {
        public int DoctorID { get; set; }
        public string Username { get; set; }

        [Required(ErrorMessage = "Specialization is required.")]
        public string Specialization { get; set; }

        [Required(ErrorMessage = "License Number is required.")]
        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

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

}
