using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManagement.Models;
using InventoryManagement.Services;
using static InventoryManagement.Components.LoginComponent;

namespace InventoryManagement.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly UserService _userService;

        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
        {
            get => users;
            set
            {
                users = value;
                OnPropertyChanged();
            }
        }

        public LoginViewModel(UserService userService)
        {
            _userService = userService;
        }

        public async Task<bool> LoginUser(LoginModel model)
        {
            return await _userService.LoginUser(model);
        }

        public async Task LoadUsers()
        {
            Users = await _userService.LoadUsersAsync();
        }

        public async Task<ResponseModel> AddUser(User user)
        {
            return await _userService.AddUser(user);
        }
    }
}
