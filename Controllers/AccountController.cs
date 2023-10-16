using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientMedicalRecord.Models;
using Newtonsoft.Json;


namespace PatientMedicalRecord.Controllers
{
    public class AccountController : Controller
    {
        private readonly MedicalRecordsDBContext _context;

        public AccountController(MedicalRecordsDBContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            // check if the provided username and password match with the database
            // use the Entity framework to check the database
            using (MedicalRecordsDBContext db = new MedicalRecordsDBContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
                if (user != null)
                {
                    ViewBag.Role = user.UserType;
                    // Serialize the user object
                    var json = JsonConvert.SerializeObject(user);
                    byte[] userData = Encoding.UTF8.GetBytes(json);
                    HttpContext.Session.Set("user", userData);
                    // redirect the user to the appropriate page based on their role
                    switch (user.UserType)
                    {
                        case "Patient":
                            var patient = db.Patients.FirstOrDefault(s => s.Username == user.Username);
                            string patientUsername = patient.Username;
                            HttpContext.Session.SetString("patientUsername", patientUsername);
                            return RedirectToAction("Index", "PatientsActions");
                        case "Doctor":
                            var doctor = db.Doctors.FirstOrDefault(p => p.Username == user.Username);
                            string doctorUsername = doctor.Username;
                            HttpContext.Session.SetString("doctorUsername", doctorUsername);
                            return RedirectToAction("Index", "DoctorsActions");
                        case "Admin":
                            var admin = db.Admins.FirstOrDefault(s => s.Username == user.Username);
                            string adminUsername = admin.Username;
                            HttpContext.Session.SetString("adminUsername", adminUsername);
                            return RedirectToAction("Index", "AdminActions");
                        default:
                            return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    // if no match is found, return an error message
                    TempData["Message"] = "Incorrect username or password";
                    return RedirectToAction("Login");
                }
            }
        }
    }
}
