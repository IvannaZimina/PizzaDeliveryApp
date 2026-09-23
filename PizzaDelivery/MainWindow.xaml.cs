using Core; // Namespace containing our business logic and data entities
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

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
            // The Core layer stores orders in an encapsulated fixed-size array to strictly protect business rules. 
            // Because the UI requires a dynamic collection for filtering and data binding, we use a for loop as a bridge 
            // to sequentially transfer the elements into a temporary List<PizzaOrder>.
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

            // Fill the DataGrid with the filtered list of orders:
            // [OrdersDataGrid] - The UI table control named in XAML
            // [ItemsSource] - The built-in property (the "connection port") that accepts a collection of items to display
            // [displayedOrders] - The actual filtered list of pizza orders (the collection of items)
            OrdersDataGrid.ItemsSource = displayedOrders;

            // Update bottom statistics labels based on the currently displayed (filtered) orders
            TxtTotalOrders.Text = displayedOrders.Count.ToString();

            // Sum() from LINQ is used to calculate the total revenue by summing the TotalPrice property of displayed orders.
            decimal totalRevenue = displayedOrders.Sum(o => o.TotalPrice);

            // :F2 means that the number will be rounded to exactly two decimal places.
            TxtTotalRevenue.Text = $"{totalRevenue:F2} $";
        }

        // Event handler for real-time search filtering as the user types
        // [void] - Return type: indicates that the method performs an action but does not return any data back when finished
        // [object sender] - a reference to the specific search text box control where the user is typing
        // [TextChangedEventArgs e] - "event details" (technical background data about how the text was changed)
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // [TxtSearch.Text] - grabs whatever characters the user has typed right now and passes that string into the filter method
            RefreshDataGrid(TxtSearch.Text);
        }

        // Event handler to open the order creation dialog window
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Creates an instance of the Order Creation dialog box.
            var dialog = new OrderDialog();

            // 1. Specify that the current window is the owner of this dialog box
            dialog.Owner = this;

            // 2. Instruct the system to open the window strictly in the center of the owner window
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (dialog.ShowDialog() == true)
            {
                // Passing the current search text there so that the table is updated, but the filter is not reset.
                RefreshDataGrid(TxtSearch.Text);
            }
        }

        // Event handler to delete the selected order from the list and storage
        // [object sender, RoutedEventArgs e] - Standard parameters: who clicked and event details
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Check if an order is currently selected in the DataGrid
            // isPizzaOrder — a security check if the user hasn't selected anything and the table is empty.
            // selectedOrder — if the check passes, a temporary variable with this name is created, where the order is stored, making it clear that it should be deleted.
            if (OrdersDataGrid.SelectedItem is PizzaOrder selectedOrder)
            {
                // MessageBox.Show is a ready-made system dialog built into Windows and WPF, so there's no need to design it in XAML
                // The `result` variable for the user's response - whether they clicked Yes or No
                var result = MessageBox.Show(
                    PizzaDelivery.Resources.Resources.DeleteConfirmMessage, // The text of the question itself from Resources
                    PizzaDelivery.Resources.Resources.DeleteConfirmTitle,   // The text in the header (title) of this window - Delete confirmation
                    MessageBoxButton.YesNo,                                 // Telling the system: "Show the user two buttons - 'Yes' and 'No'"
                    MessageBoxImage.Question);                              // Drawing a question mark icon to attract attention


                // check if the client choose yes
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // call the Core layer to delete the order by its unique Id
                        PizzaManager.DeleteOrder(selectedOrder.Id);
                        // Refresh table and stats after deletion
                        RefreshDataGrid(TxtSearch.Text);
                    }
                    catch (DomainException ex)
                    {
                        // Handle business logic exceptions, which was set in the Core
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