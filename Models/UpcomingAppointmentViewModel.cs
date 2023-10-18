namespace PatientMedicalRecord.Models
{
    public class UpcomingAppointmentViewModel
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public string MedicalRecordNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
