using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientMedicalRecord.Models;
using System.Linq;

namespace PatientMedicalRecord.Controllers
{
    public class AdminActionsController : Controller
    {
        private readonly MedicalRecordsDBContext _context;

        public AdminActionsController(MedicalRecordsDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Retrieve the username from the session using the correct key (for admins)
            var username = HttpContext.Session.GetString("adminUsername");

            // Query the database to retrieve user and admin details
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            // If you have an "Admin" entity in your database, you can query it as well
            var admin = _context.Admins.FirstOrDefault(a => a.Username == username);

            // Pass the user and admin details to the view
            ViewBag.User = user;
            ViewBag.Admin = admin;

            return View();
        }

        public IActionResult CreatePatient()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreatePatient(AdminCreatePatientViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check for duplicate Username, MedicalRecordNumber, Email, and Phone
                if (!_context.Users.Any(u => u.Username == model.Username) &&
                    !_context.Patients.Any(p => p.MedicalRecordNumber == model.MedicalRecordNumber) &&
                    !_context.Users.Any(u => u.Email == model.Email) &&
                    !_context.Users.Any(u => u.Phone == model.Phone))
                {
                    // Create a new User record with additional fields
                    var user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Username = model.Username,
                        Password = model.Password,
                        UserType = "Patient", // Set UserType to "Patient" for users associated with patients
                        Email = model.Email,   // Include email
                        Phone = model.Phone,   // Include phone
                        Address = model.Address // Include address
                    };

                    // Create a new Patient record
                    var patient = new Patient
                    {
                        MedicalRecordNumber = model.MedicalRecordNumber,
                        DateOfBirth = model.DateOfBirth,
                        Gender = model.Gender,
                        Username = model.Username // Associate the patient with the created user
                    };

                    // Add both user and patient to the context and save changes
                    _context.Users.Add(user);
                    _context.Patients.Add(patient);
                    _context.SaveChanges();

                    // Redirect to the admin's patient list or another page
                    return RedirectToAction("PatientList");
                }
                else
                {
                    // Handle uniqueness constraint violations
                    ModelState.AddModelError(string.Empty, "One or more fields are not unique.");
                }
            }

            // If the model is not valid or uniqueness constraints are violated, return to the form with validation errors
            return View(model);
        }

        public IActionResult PatientList()
        {
            // Query the database to retrieve data from both the "Patient" and "Users" tables
            var patients = _context.Patients
                .Join(_context.Users,
                      patient => patient.Username,
                      user => user.Username,
                      (patient, user) => new PatientListViewModel
                      {
                          PatientID = patient.PatientId,
                          FirstName = user.FirstName,
                          LastName = user.LastName,
                          MedicalRecordNumber = patient.MedicalRecordNumber,
                          DateOfBirth = patient.DateOfBirth ?? DateTime.MinValue, // Handle null value
                          Gender = patient.Gender,
                          Username = user.Username,
                          Email = user.Email,
                          Phone = user.Phone,
                          Address = user.Address
                      })
                .ToList();

            return View(patients);
        }

        public IActionResult Delete(string username)
        {
            // Retrieve the user and patient records by username
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            var patient = _context.Patients.FirstOrDefault(p => p.Username == username);

            if (user == null || patient == null)
            {
                // Handle not found user or patient
                return NotFound();
            }

            // Delete the user and patient records
            // Save changes to the database

            return RedirectToAction("PatientList");
        }

        public IActionResult EditPatient(string username)
        {
            // Retrieve the user and patient records by username
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            var patient = _context.Patients.FirstOrDefault(p => p.Username == username);

            if (user == null || patient == null)
            {
                // Handle not found user or patient
                return NotFound();
            }

            // Create an edit view model and populate it with user and patient data
            var editModel = new EditPatientViewModel
            {
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Password = user.Password,
                MedicalRecordNumber = patient.MedicalRecordNumber,
                DateOfBirth = patient.DateOfBirth ?? DateTime.MinValue, // Handle null value
                Gender = patient.Gender,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address
            };

            return View(editModel);
        }



        [HttpPost]
        public IActionResult Update(EditPatientViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the user and patient records by username
                var user = _context.Users.FirstOrDefault(u => u.Username == model.Username);
                var patient = _context.Patients.FirstOrDefault(p => p.Username == model.Username);

                if (user == null || patient == null)
                {
                    // Handle not found user or patient
                    return NotFound();
                }

                // Update the user and patient data with values from the model
                // Save changes to the database

                return RedirectToAction("PatientList");
            }

            // If the model is not valid, return to the edit form with validation errors
            return View("EditPatient", model);
        }

    }

}
