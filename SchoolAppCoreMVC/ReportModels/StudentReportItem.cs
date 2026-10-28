
namespace SchoolAppCoreMVC.ReportModels
{
    public class StudentReportItem
    {
        public int StudentID { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public DateTime EnrollmentDate { get; set; }
    }
}
