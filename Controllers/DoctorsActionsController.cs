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
                .Where(patient => patient.MedicalRecordNumber.Contains(searchInput))
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
            // Retrieve the doctor's username from the session
            var doctorUsername = HttpContext.Session.GetString("doctorUsername");

            // Retrieve the doctor's information from the database
            var doctor = _context.Doctors.FirstOrDefault(d => d.Username == doctorUsername);

            if (doctor == null)
            {
                // Handle the case where the doctor is not found in the database
                // You can return an error view or perform any other desired action.
                return View("Error"); // Create an "Error" view for error handling
            }

            // Create a new instance of the MedicalRecord model
            var newMedicalRecord = new MedicalRecord
            {
                // Set the DoctorID to the ID of the logged-in doctor
                DoctorId = doctor.DoctorId,
                // Initialize other properties as needed
                DateOfVisit = DateTime.Now, // For example, set the visit date to the current date
            };

            // You can add more properties to the MedicalRecord as needed

            // Save the new medical record to the database
            _context.MedicalRecords.Add(newMedicalRecord);
            _context.SaveChanges();

            // Redirect to a success or confirmation page
            return View("CreateMedicalRecordConfirmation"); // Create a "CreateMedicalRecordConfirmation" view
        }


    }
}
