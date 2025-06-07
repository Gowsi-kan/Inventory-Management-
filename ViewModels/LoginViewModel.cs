using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManagement.Services;
using static InventoryManagement.Components.LoginComponent;

namespace InventoryManagement.ViewModels
{
    public class LoginViewModel
    {
        private readonly UserService _userService;

        public LoginViewModel(UserService userService)
        {
            _userService = userService;
        }

        public async Task<bool> LoginUser(LoginModel model)
        {
            return await _userService.LoginUser(model);
        }
    }
}
