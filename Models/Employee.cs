using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public DateTime EmployeeDOB { get; set; }
        public string EmployeeNIC { get; set; }
        public string? EmployeePassportNO { get; set; } = "";
        public string EmployeePhoneNumber { get; set; }
        public int EmployeeYearOfExpYears { get; set; }
        public bool EmployeeOLQualified { get; set; }
        public bool EmployeeALQualified { get; set; }
        public bool EmployeeExtraQualification { get; set; }
        public bool EmployeeCertificateOfExpUploaded { get; set; }
        public bool EmployeeCertificateOfOLUploaded { get; set; }
        public bool EmployeeCertificateOfALUploaded { get; set; }
        public bool EmployeeCertificateOfExtraQualificationUploaded { get; set; }
        public bool EmployeeNICCopyUploaded { get; set; }
        public bool EmployeePassportCopyUploaded { get; set; }
        public bool EmployeeContractFormUploaded { get; set; }
        public string? EmployeeFileDir { get; set; } = "";
    }
}
