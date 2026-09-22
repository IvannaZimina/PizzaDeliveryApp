using System;

// Main business logic class for managing pizza orders (Create, Read, Delete)
namespace Core
{
    // Static manager class responsible for business logic and order array management
    // Static class (no 'new' required); methods and data are accessed directly through the class name.
    public static class PizzaManager
    {
        // Fixed-size array to store up to 100 pizza orders
        private static PizzaOrder[] _orders = new PizzaOrder[100];

        // Current number of orders stored in the array
        public static int Count { get; private set; } = 0;

        // Adds a new pizza order to the array if there is available space
        public static void AddOrder(PizzaOrder order)
        {
            // Check if the array is full
            if (Count >= 100)
            {
                throw new DomainException("Order limit reached! Cannot add more than 100 orders.");
            }

            // Store the order and increment the count
            _orders[Count] = order;
            Count++;
        }

        // Deletes an order by its unique ID and shifts remaining elements to the left
        public static void DeleteOrder(int id)
        {
            int indexToDelete = -1; // -1 - not found yet

            // Find the index of the order with the specified ID
            for (int i = 0; i < Count; i++) // linear search
            {
                if (_orders[i].Id == id)
                {
                    indexToDelete = i;
                    break;
                }
            }

            // If the order was not found, throw a custom exception
            if (indexToDelete == -1)
            {
                throw new DomainException($"Order with ID {id} was not found.");
            }

            // Shift elements to the left to fill the gap after deletion
            for (int i = indexToDelete; i < Count - 1; i++)
            {
                _orders[i] = _orders[i + 1];
            }

            // Clear the last element slot and decrease the count
            _orders[Count - 1] = default; // default means reset to empty/zero state
            Count--;
        }

        // Calculates total price and preparation time based on size, dough, and sauces
        // The method returns a tuple of two values.
        public static (decimal price, int prepTime) CalculatePriceAndPrepTime(PizzaSize size, DoughType dough, int sauceCount)
        {
            // Validate business rule: small pizza cannot have more than 3 sauces
            if (size == PizzaSize.Small && sauceCount > 3)
            {
                throw new DomainException("A small pizza can have a maximum of 3 sauces.");
            }

            // Switch expression covering 6 combinations of size and dough types
            // what switch return (tuple) : properties to switch for compare with the combination of rules  
            (decimal basePrice, int baseTime) = (size, dough) switch
            {
                (PizzaSize.Small, DoughType.Thin) => (7.00m, 15),
                (PizzaSize.Small, DoughType.Traditional) => (8.00m, 18),
                (PizzaSize.Medium, DoughType.Thin) => (10.00m, 20),
                (PizzaSize.Medium, DoughType.Traditional) => (11.00m, 22),
                (PizzaSize.Large, DoughType.Traditional) => (14.00m, 25),
                (PizzaSize.Large, DoughType.Fluffy) => (16.00m, 30),

                _ => (9.00m, 20) // Default fallback values for any other combination
            };

            // Calculate final price and cooking time including extra sauces
            decimal totalPrice = basePrice + (sauceCount * 1.5m); // m - means decimal
            int totalPrepTime = baseTime + (sauceCount * 2);

            return (totalPrice, totalPrepTime);
        }

        // Gets an order by its index for viewing/binding purposes
        public static PizzaOrder GetOrderAt(int index)
        {
            // index can't be 0 or negative
            // index can't be greater than or equal to Count
            if (index < 0 || index >= Count)
            {
                throw new DomainException("Invalid index.");
            }
            return _orders[index];
        }
    }
}