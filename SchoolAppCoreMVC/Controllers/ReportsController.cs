
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Reporting.NETCore;
using SchoolAppCoreMVC.Models;
using SchoolAppCoreMVC.ReportModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolAppCoreMVC.Controllers
{
    public class ReportsController : Controller
    {
        private readonly SchoolContext _context;
        private readonly IWebHostEnvironment _environment;

        public ReportsController(
            SchoolContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> StudentsPdf()
        {
            // 1. Load student data from SchoolDB
            var students = await _context.Students
                .AsNoTracking()
                .OrderBy(s => s.StudentID)
                .Select(s => new StudentReportItem
                {
                    StudentID = s.StudentID,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    EnrollmentDate = s.EnrollmentDate
                })
                .ToListAsync();

            // 2. Locate the RDLC file
            string reportPath = Path.Combine(
                _environment.ContentRootPath,
                "Reports",
                "StudentsReports.rdlc"
            );

            if (!System.IO.File.Exists(reportPath))
            {
                return NotFound(
                    $"RDLC report not found: {reportPath}");
            }

            // 3. Load the local RDLC report
            using var report = new LocalReport();

            using (var stream =
                System.IO.File.OpenRead(reportPath))
            {
                report.LoadReportDefinition(stream);
            }

            // 4. Supply data using the exact RDLC dataset name
            report.DataSources.Clear();

            report.DataSources.Add(
                new ReportDataSource(
                    "StudentsDataSet",
                    students
                )
            );

            // 5. Render the PDF
            byte[] pdf = report.Render("PDF");

            return File(
                pdf,
                "application/pdf",
                "Students.pdf"
            );
        }
    }
}
