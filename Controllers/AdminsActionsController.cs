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

        public IActionResult DeletePatient(int id)
        {
            // Find the patient by ID
            var patient = _context.Patients.FirstOrDefault(p => p.PatientId == id);

            if (patient == null)
            {
                return NotFound();
            }

            // Get the Username associated with the patient
            string username = patient.Username;

            // Delete the associated appointments and medical records
            var appointments = _context.Appointments.Where(a => a.PatientId == id).ToList();
            var medicalRecords = _context.MedicalRecords.Where(mr => mr.PatientId == id).ToList();

            _context.Appointments.RemoveRange(appointments);
            _context.MedicalRecords.RemoveRange(medicalRecords);

            // Remove the patient from the Patients table
            _context.Patients.Remove(patient);

            // Remove the corresponding User from the Users table
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            // Save the changes
            _context.SaveChanges();

            return RedirectToAction("PatientList");
        }


        [HttpGet]
        public IActionResult EditPatient(int id)
        {
            // Find the patient by ID
            var patient = _context.Patients.FirstOrDefault(p => p.PatientId == id);

            if (patient == null)
            {
                return NotFound();
            }

            // Find the associated user by username
            var user = _context.Users.FirstOrDefault(u => u.Username == patient.Username);

            if (user == null)
            {
                return NotFound();
            }

            var editModel = new PatientEditViewModel
            {
                PatientID = patient.PatientId,
                Username = patient.Username,
                MedicalRecordNumber = patient.MedicalRecordNumber,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address
            };

            return View(editModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPatient(PatientEditViewModel editModel)
        {
            if (ModelState.IsValid)
            {
                // Find the patient by ID
                var patient = _context.Patients.FirstOrDefault(p => p.PatientId == editModel.PatientID);

                if (patient == null)
                {
                    return NotFound();
                }

                // Find the associated user by username
                var user = _context.Users.FirstOrDefault(u => u.Username == editModel.Username);

                if (user == null)
                {
                    return NotFound();
                }

                // Update patient details
                patient.MedicalRecordNumber = editModel.MedicalRecordNumber;
                patient.DateOfBirth = editModel.DateOfBirth;
                patient.Gender = editModel.Gender;

                // Update user details
                user.FirstName = editModel.FirstName;
                user.LastName = editModel.LastName;
                user.Email = editModel.Email;
                user.Phone = editModel.Phone;
                user.Address = editModel.Address;

                // Update both the patient and user records in the context
                _context.Patients.Update(patient);
                _context.Users.Update(user);

                // Save the changes
                _context.SaveChanges();

                return RedirectToAction("PatientList");
            }

            return View(editModel);
        }


        [HttpGet]
        public IActionResult CreateDoctor()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateDoctor(AdminCreateDoctorViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check for duplicate Username, Email, and Phone
                if (!_context.Users.Any(u => u.Username == model.Username) &&
                    !_context.Doctors.Any(d => d.LicenseNumber == model.LicenseNumber) &&
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
                        UserType = "Doctor", // Set UserType to "Doctor" for users associated with doctors
                        Email = model.Email,
                        Phone = model.Phone,
                        Address = model.Address,
                    };

                    // Create a new Doctor record
                    var doctor = new Doctor
                    {
                        Specialization = model.Specialization,
                        LicenseNumber = model.LicenseNumber,
                        Username = model.Username, // Associate the doctor with the created user
                    };

                    // Add both user and doctor to the context and save changes
                    _context.Users.Add(user);
                    _context.Doctors.Add(doctor);
                    _context.SaveChanges();

                    // Redirect to the admin's doctor list or another page
                    return RedirectToAction("DoctorList");
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

        // Action to list doctors
        public IActionResult DoctorList()
        {
            var doctors = _context.Doctors
                .Join(_context.Users,
                      doctor => doctor.Username,
                      user => user.Username,
                      (doctor, user) => new DoctorListFromPatientViewModel
                      {
                          DoctorID = doctor.DoctorId,
                          FirstName = user.FirstName,
                          LastName = user.LastName,
                          Specialization = doctor.Specialization,
                          LicenseNumber = doctor.LicenseNumber,
                          Username = user.Username,
                          Email = user.Email,
                          Phone = user.Phone,
                          Address = user.Address
                      })
                .ToList();

            return View(doctors);
        }

        // Action to delete a doctor
        public IActionResult DeleteDoctor(int id)
        {
            var doctor = _context.Doctors.FirstOrDefault(d => d.DoctorId == id);

            if (doctor == null)
            {
                return NotFound();
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == doctor.Username);

            if (user == null)
            {
                return NotFound();
            }

            var appointments = _context.Appointments.Where(a => a.DoctorId == id).ToList();
            var medicalRecords = _context.MedicalRecords.Where(mr => mr.DoctorId == id).ToList();

            _context.Appointments.RemoveRange(appointments);
            _context.MedicalRecords.RemoveRange(medicalRecords);
            _context.Doctors.Remove(doctor);
            _context.Users.Remove(user);

            _context.SaveChanges();

            return RedirectToAction("DoctorList");
        }

        [HttpGet]
        public IActionResult EditDoctor(int id)
        {
            var doctor = _context.Doctors.FirstOrDefault(d => d.DoctorId == id);

            if (doctor == null)
            {
                return NotFound();
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == doctor.Username);

            if (user == null)
            {
                return NotFound();
            }

            var editModel = new DoctorEditViewModel
            {
                DoctorID = doctor.DoctorId,
                Username = doctor.Username,
                Specialization = doctor.Specialization,
                LicenseNumber = doctor.LicenseNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address
            };

            return View(editModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditDoctor(DoctorEditViewModel editModel)
        {
            if (ModelState.IsValid)
            {
                var doctor = _context.Doctors.FirstOrDefault(d => d.DoctorId == editModel.DoctorID);

                if (doctor == null)
                {
                    return NotFound();
                }

                var user = _context.Users.FirstOrDefault(u => u.Username == editModel.Username);

                if (user == null)
                {
                    return NotFound();
                }

                doctor.Specialization = editModel.Specialization;
                doctor.LicenseNumber = editModel.LicenseNumber;
                user.FirstName = editModel.FirstName;
                user.LastName = editModel.LastName;
                user.Email = editModel.Email;
                user.Phone = editModel.Phone;
                user.Address = editModel.Address;

                _context.Doctors.Update(doctor);
                _context.Users.Update(user);

                _context.SaveChanges();

                return RedirectToAction("DoctorList");
            }

            return View(editModel);
        }
    }

}
