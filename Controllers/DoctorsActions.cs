using Microsoft.AspNetCore.Mvc;

namespace PatientMedicalRecord.Controllers
{
    public class DoctorsActions : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
