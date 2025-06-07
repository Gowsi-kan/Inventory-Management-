using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Models
{
    public class Expense
    {
        public int ExpenseId { get; set; } // Primary key column

        public string? ExpenseName { get; set; } // Name of the expense

        public float ExpenseAmount { get; set; } // Amount of the expense

        public string? ExpensePaymentType { get; set; } // Type of payment for the expense (e.g., cash, card, cheque, bank)

        public bool ExpenseBillUploaded { get; set; } // Indicates if the bill for the expense is uploaded

        public string? ExpensesBillFileDir { get; set; } // Directory path of the expense bill file, nullable

        public DateTime UpdateDate { get; set; } // Date when the expense was updated
    }
}
