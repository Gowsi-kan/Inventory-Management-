using System.ComponentModel.DataAnnotations;
using Color = MudBlazor.Color;

namespace InventoryManagement.Models
{
    public enum AccountType
    {
        Admin,
        Owner,
        User
    }

    public static class EnumHelper
    {
        public static string GetDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field.GetCustomAttributes(typeof(DisplayAttribute), false)
                            .FirstOrDefault() as DisplayAttribute;
            return attr?.Name ?? value.ToString();
        }
    }

    public static class Utility 
    {
        public static AccountType GetAccountType(int accountPreviledge)
        {
            return accountPreviledge switch
            {
                0 => AccountType.Owner,
                1 => AccountType.Admin,
                2 => AccountType.User,
                _ => AccountType.User,
            };
        }

        public static short GetAccountType(AccountType accountPreviledge)
        {
            return accountPreviledge switch
            {
                AccountType.Owner => 0,
                AccountType.Admin => 1,
                AccountType.User => 2,
                _ => 2, // Default to User if unknown
            };
        }

        public static (string Label, Color ChipColor) GetAccountTypeChip(int userPrivilege)
        {
            return userPrivilege switch
            {
                1 => ("Admin", Color.Error),    // e.g., red for Admin
                0 => ("Owner", Color.Success),     // e.g., blue for Owner
                2 => ("User", Color.Primary),   // e.g., green for User
                _ => ("User", Color.Primary),
            };
        }
    }
}
