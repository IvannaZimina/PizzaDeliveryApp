using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Core; // Namespace containing our business logic and data entities

namespace PizzaDelivery
{
    // Main window logic for managing pizza orders, UI events, and data binding
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RefreshDataGrid(); // Load initial data when window opens
        }

        // Method to update DataGrid and bottom statistics panel (with optional search filter)
        private void RefreshDataGrid(string filter = "")
        {
            // Collect all valid orders from the Core manager array based on current Count
            // In the Core project, orders are stored not in a standard List, but in a regular fixed array,
            // and the number of filled elements is tracked in the PizzaManager.Count property.
            // Since it is inconvenient to directly return the array or iterate through it using foreach, a temporary List<PizzaOrder> is created.
            var allOrders = new List<PizzaOrder>();
            for (int i = 0; i < PizzaManager.Count; i++)
            {
                allOrders.Add(PizzaManager.GetOrderAt(i));
            }

            // Filter orders by client name if search text is provided
            // First, check whether the search string is empty or whether it consists only of spaces.
            // .Where() from LINQ is used to filter the list of orders based on the search string.
            var displayedOrders = string.IsNullOrWhiteSpace(filter)
                ? allOrders
                : allOrders.Where(o => o.ClientName.Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();

            // Fill the DataGrid with the filtered list of orders
            OrdersDataGrid.ItemsSource = displayedOrders;

            // Update bottom statistics labels
            TxtTotalOrders.Text = allOrders.Count.ToString();

            // Fixed property name: TotalPrice instead of Price
            // Sum() from LINQ is used to calculate the total revenue by summing the TotalPrice property of all orders.
            decimal totalRevenue = allOrders.Sum(o => o.TotalPrice);

            // :F2 means that the number will be rounded to exactly two decimal places.
            TxtTotalRevenue.Text = $"{totalRevenue:F2} $";
        }

        // Event handler for real-time search filtering as the user types
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshDataGrid(TxtSearch.Text);
        }

        // Event handler to open the order creation dialog window
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            // OrderDialog will be created in the next step.
            var dialog = new OrderDialog();
            if (dialog.ShowDialog() == true)
            {
                RefreshDataGrid(TxtSearch.Text); // Refresh list if a new order was added successfully
            }
        }

        // Event handler to delete the selected order from the list and storage
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Check if an order is currently selected in the DataGrid
            if (OrdersDataGrid.SelectedItem is PizzaOrder selectedOrder)
            {
                // Ask for user confirmation using explicit resource reference
                var result = MessageBox.Show(
                    PizzaDelivery.Resources.Resources.DeleteConfirmMessage,
                    PizzaDelivery.Resources.Resources.DeleteConfirmTitle,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        PizzaManager.DeleteOrder(selectedOrder.Id);
                        RefreshDataGrid(TxtSearch.Text); // Refresh table and stats after deletion
                    }
                    catch (DomainException ex)
                    {
                        // Handle business logic exceptions
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                // Warn user if no row is selected
                MessageBox.Show("Please select an order to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}