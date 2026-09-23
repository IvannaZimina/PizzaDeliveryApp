using Core; // Namespace for business logic, enums, and exceptions
using System;
using System.Windows;
using System.Windows.Controls;

namespace PizzaDelivery
{
    // Dialog window logic for creating a new pizza order
    // partial class split XAML and code-behind
    // Inherits from Window class
    public partial class OrderDialog : Window
    {
        // Static auto-incrementing ID generator for orders
        private static int _idCounter = 1;

        public OrderDialog()
        {
            InitializeComponent();  // xaml initialization
            LoadComboBoxes();       // Populate ComboBoxes with enum values
        }

        // Fill ComboBox options
        private void LoadComboBoxes()
        {
            CmbSize.ItemsSource = Enum.GetValues(typeof(PizzaSize));    // Adding variants of the enum PizzaSize from Core
            CmbSize.SelectedIndex = 0;                                  // Default selection

            CmbDough.ItemsSource = Enum.GetValues(typeof(DoughType));   // Adding variants of the enum DoughType from Core
            CmbDough.SelectedIndex = 0;                                 // Default selection
        }

        // Event handler for Cancel button
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;   // Close dialog without saving
            Close();                // For reliability, a forced closing option has also been added.
        }

        // ========= Validation methods Start ========= //

        // Проверка и получение имени клиента
        private string GetValidatedClientName()
        {
            // 1. Read and validate input fields, remove whitespaces
            string clientName = TxtClientName.Text.Trim();
            if (string.IsNullOrEmpty(clientName))
            {
                throw new DomainException("Client name cannot be empty.");
            }
            return clientName;
        }

        private PizzaSize GetValidatedSize()
        {
            // [out DoughType dough] - If the text actually matches some pizza size from the Enum,
            // the out method creates a size variable and writes this size there.
            if (!Enum.TryParse(CmbSize.SelectedItem?.ToString(), out PizzaSize size))
            {
                throw new DomainException("Please select a valid pizza size.");
            }
            return size;
        }

        private DoughType GetValidatedDough()
        {
            if (!Enum.TryParse(CmbDough.SelectedItem?.ToString(), out DoughType dough))
            {
                throw new DomainException("Please select a valid dough type.");
            }
            return dough;
        }

        private int GetValidatedSauceCount()
        {
            if (!int.TryParse(TxtSauceCount.Text, out int sauceCount) || sauceCount < 0)
            {
                throw new DomainException("Sauce count must be a valid non-negative number.");
            }
            return sauceCount;
        }

        // ========= Validation methods End ========= //

        private void SaveNewOrder()
        {
            // gather the data and validate it
            string clientName = GetValidatedClientName();
            PizzaSize size = GetValidatedSize();
            DoughType dough = GetValidatedDough();
            int sauceCount = GetValidatedSauceCount();

            // Calculate price and prep time using Core business logic
            var (totalPrice, prepTimeMinutes) = PizzaManager.CalculatePriceAndPrepTime(size, dough, sauceCount);

            // Create the new order instance
            var newOrder = new PizzaOrder(
                id: _idCounter++,
                clientName: clientName,
                size: size,
                dough: dough,
                sauceCount: sauceCount,
                totalPrice: totalPrice,
                prepTimeMinutes: prepTimeMinutes
            );

            // Add order to the manager storage
            PizzaManager.AddOrder(newOrder);

            // Close dialog successfully
            DialogResult = true;
            Close();
        }

        // Event handler for Save button with error handling
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveNewOrder();
            }
            catch (DomainException ex)
            {
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}