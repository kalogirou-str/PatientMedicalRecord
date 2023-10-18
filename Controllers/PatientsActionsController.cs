using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientMedicalRecord.Models;
using Microsoft.AspNetCore.Http;

namespace PatientMedicalRecord.Controllers
{
    public class PatientsActionsController : Controller
    {
        private readonly MedicalRecordsDBContext _context;

        public PatientsActionsController(MedicalRecordsDBContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            // Retrieve the username from the session using the correct key
            var username = HttpContext.Session.GetString("patientUsername");

            // Query the database to retrieve user and patient details
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            var patient = _context.Patients.FirstOrDefault(p => p.Username == username);

            // Pass the user and patient details to the view
            ViewBag.User = user;
            ViewBag.Patient = patient;

            return View();
        }

        public IActionResult Appointments()
        {
            // Retrieve the username from the session using the correct key
            var username = HttpContext.Session.GetString("patientUsername");

            // Query the database to retrieve the patient's appointments along with doctor information
            var patient = _context.Patients.FirstOrDefault(p => p.Username == username);

            // Ensure the patient exists
            if (patient != null)
            {
                // Query appointments for the patient and include related doctor information
                var appointments = _context.Appointments
                    .Where(a => a.PatientId == patient.PatientId)
                    .Include(a => a.Doctor)
                    .ToList();

                return View(appointments);
            }

            // Handle the case where the patient is not found
            // You can redirect to an error page or show a message
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CancelAppointment(int patientId, int doctorId, DateTime appointmentDate)
        {
            // Retrieve the appointment based on the provided values
            var appointment = _context.Appointments
                .FirstOrDefault(a => a.PatientId == patientId && a.DoctorId == doctorId && a.AppointmentDate == appointmentDate);

            if (appointment != null)
            {
                // Remove the appointment from the context and mark it for deletion
                _context.Appointments.Remove(appointment);
                _context.SaveChanges(); // Save changes to delete the appointment

                // Redirect to the updated appointments list or a success page
                return RedirectToAction("Appointments");
            }

            // Handle the case where the appointment is not found
            // You can redirect to an error page or show a message
            return RedirectToAction("Appointments");
        }


        public IActionResult MedicalRecords()
        {
            // Retrieve the username from the session using the correct key
            var username = HttpContext.Session.GetString("patientUsername");

            // Query the database to retrieve the patient's medical records
            var patient = _context.Patients.FirstOrDefault(p => p.Username == username);

            // Ensure the patient exists
            if (patient != null)
            {
                // Query medical records for the patient
                var medicalRecords = _context.MedicalRecords
                    .Where(mr => mr.PatientId == patient.PatientId)
                    .ToList();

                return View(medicalRecords);
            }

            // Handle the case where the patient is not found
            // You can redirect to an error page or show a message
            return RedirectToAction("Index");
        }

        public IActionResult ScheduleAppointment()
        {
            var doctors = _context.Doctors
                .Join(_context.Users, doctor => doctor.Username, user => user.Username,
                    (doctor, user) => new DoctorListViewModel
                    {
                        DoctorID = doctor.DoctorId,
                        FullName = $"{user.FirstName} {user.LastName}",
                        Specialization = doctor.Specialization
                    })
                .ToList();

            return View(doctors);
        }



        [HttpPost]
        public IActionResult CreateAppointment(int DoctorID, DateTime AppointmentDate, string? Notes)
        {
            // Retrieve the username from the session using the correct key
            var username = HttpContext.Session.GetString("patientUsername");

            // Find the patient based on the username
            var patient = _context.Patients.FirstOrDefault(p => p.Username == username);

            // Ensure the patient and doctor exist
            if (patient != null)
            {
                var doctor = _context.Doctors.FirstOrDefault(d => d.DoctorId == DoctorID);

                if (doctor != null && ModelState.IsValid)
                {
                    var appointment = new Appointment
                    {
                        PatientId = patient.PatientId,
                        DoctorId = doctor.DoctorId,
                        AppointmentDate = AppointmentDate,
                        Notes = Notes
                    };

                    // Set the appointment status based on the appointment date
                    if (AppointmentDate <= DateTime.Now)
                    {
                        appointment.Status = "Completed";
                    }
                    else
                    {
                        appointment.Status = "Pending";
                    }

                    // The database will automatically generate the AppointmentID
                    _context.Appointments.Add(appointment);
                    _context.SaveChanges();

                    // Redirect to the appointments list or a success page
                    return RedirectToAction("Appointments");
                }
            }

            // Handle errors or redirection in case of failure
            return View("ScheduleAppointment");
        }


    }
}
