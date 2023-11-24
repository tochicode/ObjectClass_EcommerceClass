using System;
using System.Collections.Generic;

namespace ObjectClass_EcommerceClass
{
    using System;
    using System.Collections.Generic;

    interface IShippingService
    {
        void ShipOrder(Order order);
    }

    class ECommerceStore : IShippingService
    {
        public string Name { get; private set; }
        public string Owner { get; private set; }
        public string Currency { get; private set; }

        private List<Product> Products { get; set; }
        private Dictionary<int, Customer> Customers { get; set; }
        private List<Order> Orders { get; set; }

        // Constructor
        public ECommerceStore(string name, string owner, string currency = "USD")
        {
            Name = name;
            Owner = owner;
            Currency = currency;
            Products = new List<Product>();
            Customers = new Dictionary<int, Customer>();
            Orders = new List<Order>();
        }

        // Destructor
        ~ECommerceStore()
        {
            Console.WriteLine($"ECommerceStore instance for {Name} is being finalized.");
        }

        // Methods
        public void AddProduct(Product product)
        {
            Products.Add(product);
        }

        public void RemoveProduct(Product product)
        {
            Products.Remove(product);
        }

        public void AddCustomer(Customer customer)
        {
            Customers.Add(customer.Id, customer);
        }

        public void RemoveCustomer(int customerId)
        {
            Customers.Remove(customerId);
        }

        public Order CreateOrder(Customer customer, List<Product> products)
        {
            Order order = new Order(customer, products, Currency);
            Orders.Add(order);
            return order;
        }

        // Interface method implementation
        public void ShipOrder(Order order)
        {
            Console.WriteLine($"Shipping order {order.OrderId} to {order.Customer.Name}");
        }
    }

    class Product
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public string Description { get; private set; }

        // Constructor
        public Product(string name, decimal price, string description = "")
        {
            Name = name;
            Price = price;
            Description = description;
        }
    }

    class Customer
    {
        private static int idCounter = 1;

        public int Id { get; }
        public string Name { get; private set; }
        public string Email { get; private set; }

        // Constructor
        public Customer(string name, string email)
        {
            Id = idCounter++;
            Name = name;
            Email = email;
        }
    }

    enum OrderStatus
    {
        Pending,
        Shipped,
        Delivered
    }

    class Order
    {
        private static int orderCounter = 1;

        public int OrderId { get; }
        public Customer Customer { get; private set; }
        public List<Product> Products { get; private set; }
        public string Currency { get; private set; }
        public decimal TotalAmount { get; private set; }

        // Enum property
        public OrderStatus Status { get; set; }

        // Record (C# 9 feature)
        public record Address(string Street, string City, string Country);

        // Property with expression-bodied member
        public Address ShippingAddress { get; set; }

        // Constructor
        public Order(Customer customer, List<Product> products, string currency)
        {
            OrderId = orderCounter++;
            Customer = customer;
            Products = products;
            Currency = currency;
            TotalAmount = CalculateTotalAmount();
            Status = OrderStatus.Pending;
        }

        private decimal CalculateTotalAmount()
        {
            decimal total = 0;
            foreach (var product in Products)
            {
                total += product.Price;
            }
            return total;
        }

        // Delegate
        public delegate void OrderShippedEventHandler(Order order);

        // Event
        public event OrderShippedEventHandler OrderShipped;

        // Method
        public void Ship()
        {
            if (Status == OrderStatus.Pending)
            {
                Status = OrderStatus.Shipped;
                OrderShipped?.Invoke(this); // Invoke the event
            }
            else
            {
                Console.WriteLine("Cannot ship an order that is not pending.");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            // Example usage:

            // Create an e-commerce store
            ECommerceStore myStore = new ECommerceStore("MyStore", "OwnerName");

            // Add products to the store
            Product laptop = new Product("Laptop", 1200, "Powerful laptop with high-end specifications.");
            Product headphones = new Product("Headphones", 100, "Wireless over-ear headphones.");
            myStore.AddProduct(laptop);
            myStore.AddProduct(headphones);

            // Add customers to the store
            Customer joeTochi = new Customer("Joe Tochi", "joeTochi@example.com");
            Customer juliaBard = new Customer("Julia Bard", "janeBard@example.com");
            myStore.AddCustomer(joeTochi);
            myStore.AddCustomer(juliaBard);

            // Create an order
            Order order1 = myStore.CreateOrder(joeTochi, new List<Product> { laptop, headphones });

            // Set shipping address
            order1.ShippingAddress = new Order.Address("123 Main St", "Cityville", "Countryland");

            // Subscribe to the OrderShipped event
            order1.OrderShipped += HandleOrderShipped;

            // Ship the order
            order1.Ship();
        }

        static void HandleOrderShipped(Order order)
        {
            Console.WriteLine($"Order {order.OrderId} has been shipped to {order.ShippingAddress.City}, {order.ShippingAddress.Country}");
        }
    }

}