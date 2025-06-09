using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyAddressStreet { get; set; }
        public string? CompanyAddressCity { get; set; }
        public string? CompanyAddressDistrict { get; set; }
        public string? CompanyAddressProvince { get; set; }
        public string? CompanyAddressCountry { get; set; }
        public string? CompanyPhoneNumber { get; set; }
        public string? OrderAdvancePaymentType { get; set; }
        public float OrderAdvancePayment { get; set; }
        public string? OrderFinalPaymentType { get; set; }
        public float OrderFinalPayment { get; set; }
        public string? OrderReceiveStatus { get; set; }
        public bool OrderPaymentRollbacked { get; set; }
        public float OrderRollbackPayment { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime OrderReceivedDate { get; set; }
        public bool PaymentBillUploaded { get; set; }
        public string? PaymentBillDir { get; set; }
        public string ProductData { get; set; }
    }

    public enum OrderReceiveStatus
    {
        [Display(Name = "Not Received")]
        NotReceived,

        [Display(Name = "Partially Paid")]
        PartiallyPaid,

        [Display(Name = "Fully Paid")]
        FullyPaid,

        [Display(Name = "Cancelled")]
        Cancelled,

        [Display(Name = "Returned")]
        Returned
    }
}
