using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManagement.Models;
using InventoryManagement.Services;

namespace InventoryManagement.ViewModels
{
    public class EmployeeViewModel : ViewModelBase
    {
        private readonly EmployeeService _employeeService;

        private ObservableCollection<Employee> employees;
        public ObservableCollection<Employee> Employees
        {
            get => employees;
            set
            {
                employees = value;
                OnPropertyChanged();
            }
        }
        public EmployeeViewModel(EmployeeService employeeService)
        {
            Employees = [];

            _employeeService = employeeService;
        }

        public async Task LoadEmployees()
        {
            Employees = await _employeeService.LoadEmployeesAsync();
        }

        public async Task<ResponseModel> SaveEmployee(Employee employee)
        {
            return await _employeeService.AddEmployee(employee);
        }

        public async Task<ResponseModel> UpdateEmployee(Employee employee)
        {
            return await _employeeService.UpdateEmployee(employee);
        }
    }
}
