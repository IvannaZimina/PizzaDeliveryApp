namespace Core
{
    public struct PizzaOrder
    {
        // { get; set; } added as properties for DataGrid in WPF
        public int Id { get; set; }             // Unique number to identify the order
        public string ClientName { get; set; }  // Full name of the customer who ordered the pizza
        public PizzaSize Size { get; set; }     // Size of the pizza (Small, Medium, or Large)
        public DoughType Dough { get; set; }    // Type of the dough (Thin, Traditional, or Fluffy)
        public int SauceCount { get; set; }     // Quantity of extra sauces chosen for the pizza
        public decimal TotalPrice { get; set; } // Final calculated price including size, dough, and sauces
        public int PrepTimeMinutes { get; set; } // Total preparation and cooking time in minutes

        // Constructor to create a new order with all required values
        public PizzaOrder(int id, string clientName, PizzaSize size, DoughType dough, int sauceCount, decimal totalPrice, int prepTimeMinutes)
        {
            Id = id;
            ClientName = clientName;
            Size = size;
            Dough = dough;
            SauceCount = sauceCount;
            TotalPrice = totalPrice;
            PrepTimeMinutes = prepTimeMinutes;
        }
    }
}