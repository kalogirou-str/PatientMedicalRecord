namespace PatientMedicalRecord.Models
{
    public class SearchResultViewModel
    {
        public int PatientID { get; set; }
        public string MedicalRecordNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        // Add more properties as needed
    }

}
