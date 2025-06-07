using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Models
{
    public class ResponseModel
    {
        public bool IsSuccess { get; set; } = false;
        public string Message { get; set; }

        public ResponseModel(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}
