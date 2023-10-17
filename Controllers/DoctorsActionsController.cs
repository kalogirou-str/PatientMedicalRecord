using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientMedicalRecord.Models;

namespace PatientMedicalRecord.Controllers
{
    public class DoctorsActionsController : Controller
    {
        private readonly MedicalRecordsDBContext _context;

        public DoctorsActionsController(MedicalRecordsDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Retrieve the username from the session using the correct key
            var username = HttpContext.Session.GetString("doctorUsername");

            // Query the database to retrieve user and patient details
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            var doctor = _context.Doctors.FirstOrDefault(p => p.Username == username);

            // Pass the user and patient details to the view
            ViewBag.User = user;
            ViewBag.Patient = doctor;

            return View();
        }


        public IActionResult SearchPatients()
        {
            return View(); // Display the search form
        }


        [HttpPost]
        public IActionResult SearchResults(string searchInput)
        {
            // Search for patients based on Medical Record Number and join with Users table
            var searchResults = _context.Patients
                .Where(patient => patient.MedicalRecordNumber.Equals(searchInput.ToString()))
                .Join(
                    _context.Users,
                    patient => patient.Username,
                    user => user.Username,
                    (patient, user) => new SearchResultViewModel
                    {
                        PatientID = patient.PatientId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        DateOfBirth = patient.DateOfBirth,
                        Gender = patient.Gender
                        // Add more properties here
                    }
                )
                .ToList();
            return View(searchResults);
        }



        public IActionResult ViewMedicalRecord(int patientId)
        {
            // Retrieve the medical records for the specified patient from the database
            var patientMedicalRecords = _context.MedicalRecords
                .Where(record => record.PatientId == patientId)
                .ToList();
            return View(patientMedicalRecords);
        }

        public IActionResult UpcomingAppointments()
        {
            var doctorUsername = HttpContext.Session.GetString("doctorUsername");
            var today = DateTime.Today;
            var upcomingAppointments = _context.Appointments
                .Where(a => a.Doctor.Username == doctorUsername && a.AppointmentDate >= today)
                .Select(a => new UpcomingAppointmentViewModel
                {
                    AppointmentDate = a.AppointmentDate,
                    Status = a.Status,
                    Notes = a.Notes,
                    MedicalRecordNumber = a.Patient.MedicalRecordNumber,
                    FirstName = _context.Users.Single(u => u.Username == a.Patient.Username).FirstName,
                    LastName = _context.Users.Single(u => u.Username == a.Patient.Username).LastName
                })
                .ToList();

            return View(upcomingAppointments);
        }

        public IActionResult CreateMedicalRecord()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateMedicalRecord(MedicalRecordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the logged-in doctor's username from the session
                var doctorUsername = HttpContext.Session.GetString("doctorUsername");

                // Get the doctor's information
                var doctor = _context.Doctors.FirstOrDefault(d => d.Username == doctorUsername);

                if (doctor != null)
                {
                    // Find the patient using their MedicalRecordNumber
                    var patient = _context.Patients.FirstOrDefault(p => p.MedicalRecordNumber == model.MedicalRecordNumber);

                    if (patient != null)
                    {
                        // Create a new MedicalRecord instance
                        var medicalRecord = new MedicalRecord
                        {
                            DoctorId = doctor.DoctorId,
                            PatientId = patient.PatientId,
                            DateOfVisit = model.DateOfVisit,
                            Diagnosis = model.Diagnosis,
                            Treatment = model.Treatment,
                            Prescription = model.Prescription
                            // Set other properties as needed
                        };

                        // Add the medical record to the context and save changes
                        _context.MedicalRecords.Add(medicalRecord);
                        _context.SaveChanges();

                        // Redirect to a success page or take other appropriate actions
                        return RedirectToAction("MedicalRecordCreated");
                    }
                    else
                    {
                        // Handle the case where the patient with the provided MedicalRecordNumber is not found
                        ModelState.AddModelError(string.Empty, "Patient not found. Please check the Medical Record Number.");
                    }
                }
                else
                {
                    // Handle the case where the doctor is not found
                    ModelState.AddModelError(string.Empty, "Doctor not found. Please log in again.");
                }
            }

            return View(model);
        }

        public IActionResult MedicalRecordCreated()
        {
            // Set a success message in TempData to be displayed in the view
            TempData["SuccessMessage"] = "Medical record created successfully";
            return View();
        }


    }
}
