﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManagement.Models;
using InventoryManagement.Services;

namespace InventoryManagement.ViewModels
{
    public class OrdersViewModel : ViewModelBase
    {
        private readonly OrderService _orderService;

        private ObservableCollection<Order> orders;
        public ObservableCollection<Order> Orders
        {
            get => orders;
            set
            {
                orders = value;
                OnPropertyChanged();
            }
        }

        public OrdersViewModel(OrderService orderService)
        {
            Orders = [];

            _orderService = orderService;
        }

        public async Task<ResponseModel> AddOrder(Order order)
        {
            if (order.OrderReceiveStatus.Equals("NOTRECEIVED", StringComparison.OrdinalIgnoreCase))
            {
                order.OrderReceiveStatus = "NOT RECEIVED";
            }

            return await _orderService.AddOrder(order);
        }

        public async Task<ResponseModel> UpdateOrder(Order order)
        {
            if (order.OrderReceiveStatus.Equals("NOTRECEIVED", StringComparison.OrdinalIgnoreCase))
            {
                order.OrderReceiveStatus = "NOT RECEIVED";
            }

            return await _orderService.UpdateOrder(order);
        }

        public async Task LoadOrders()
        {
            Orders = await _orderService.LoadOrdersAsync();
        }

        public async Task<ResponseModel> DeleteOrder(int OrderId)
        {
            return await _orderService.DeleteOrder(OrderId);
        }
    }
}
