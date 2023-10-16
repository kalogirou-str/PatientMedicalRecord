namespace PatientMedicalRecord.Models
{
    public class UpcomingAppointmentViewModel
    {
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public string MedicalRecordNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
