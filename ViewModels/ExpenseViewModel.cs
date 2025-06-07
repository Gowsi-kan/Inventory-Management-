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
    public partial class ExpenseViewModel : ViewModelBase
    {
        private readonly ExpenseService _expenseService;

        private ObservableCollection<Expense> expenses;
        public ObservableCollection<Expense> Expenses
        {
            get => expenses;
            set
            {
                expenses = value;
                OnPropertyChanged();
                CalculateExpenseSummaries();
            }
        }

        private float totalDailyExpense;
        public float TotalDailyExpense
        {
            get => totalDailyExpense;
            set
            {
                totalDailyExpense = value;
                OnPropertyChanged();
            }
        }

        private float totalWeeklyExpense;
        public float TotalWeeklyExpense
        {
            get => totalWeeklyExpense;
            set
            {
                totalWeeklyExpense = value;
                OnPropertyChanged();
            }
        }

        private float totalMonthlyExpense;
        public float TotalMonthlyExpense
        {
            get => totalMonthlyExpense;
            set
            {
                totalMonthlyExpense = value;
                OnPropertyChanged();
            }
        }

        private float totalYearlyExpense;
        public float TotalYearlyExpense
        {
            get => totalYearlyExpense;
            set
            {
                totalYearlyExpense = value;
                OnPropertyChanged();
            }
        }

        public double[] MonthlyExpenses { get; private set; } = new double[12];

        public ExpenseViewModel(ExpenseService expenseService)
        {
            Expenses = [];
        
            _expenseService = expenseService;
        }

        public async Task LoadExpenses()
        {
            Expenses = await _expenseService.LoadExpensesAsync();
            CalculateExpenseSummaries();
        }

        public async Task<ResponseModel> AddExpense(Expense expense)
        {
            return await _expenseService.AddExpense(expense);
        }

        private void CalculateExpenseSummaries()
        {
            var now = DateTime.Now;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek);

            TotalDailyExpense = Expenses.Where(e => e.UpdateDate.Date == now.Date).Sum(e => e.ExpenseAmount);
            TotalWeeklyExpense = Expenses.Where(e => e.UpdateDate.Date >= startOfWeek.Date).Sum(e => e.ExpenseAmount);
            TotalMonthlyExpense = Expenses.Where(e => e.UpdateDate.Year == now.Year && e.UpdateDate.Month == now.Month).Sum(e => e.ExpenseAmount);
            TotalYearlyExpense = Expenses.Where(e => e.UpdateDate.Year == now.Year).Sum(e => e.ExpenseAmount);

            // Calculate expenses for each month
            MonthlyExpenses = new double[12];
            for (int i = 0; i < 12; i++)
            {
                MonthlyExpenses[i] = Expenses
                    .Where(e => e.UpdateDate.Year == now.Year && e.UpdateDate.Month == (i + 1))
                    .Sum(e => e.ExpenseAmount);
            }

            OnPropertyChanged(nameof(TotalDailyExpense));
            OnPropertyChanged(nameof(TotalWeeklyExpense));
            OnPropertyChanged(nameof(TotalMonthlyExpense));
            OnPropertyChanged(nameof(TotalYearlyExpense));
            OnPropertyChanged(nameof(MonthlyExpenses));
        }
    }
}
