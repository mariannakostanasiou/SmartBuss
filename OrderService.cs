using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartBuss
{
    public class OrderService
    {
        private readonly List<Order> orders;
        private int nextOrderNumber;

        public OrderService()
        {
            orders = new List<Order>();
            nextOrderNumber = 1001;
        }

        public IReadOnlyList<Order> GetAll()
        {
            return orders.AsReadOnly();
        }

        public Order CreateOrder(
            List<CartItem> cartItems,
            string deliveryStop)
        {
            if (cartItems == null || cartItems.Count == 0)
            {
                throw new InvalidOperationException(
                    "Το καλάθι είναι άδειο.");
            }

            if (string.IsNullOrWhiteSpace(deliveryStop))
            {
                throw new InvalidOperationException(
                    "Δεν έχει επιλεγεί στάση παράδοσης.");
            }

            Order order = new Order
            {
                Number = nextOrderNumber++,
                Store = cartItems[0].Store,
                DeliveryStop = deliveryStop,
                Status = "Σε επεξεργασία",
                CreatedAt = DateTime.Now,
                Total = cartItems.Sum(item => item.Subtotal),
                Items = cartItems
                    .Select(item => item.Name + " x" + item.Quantity)
                    .ToList()
            };

            orders.Add(order);
            return order;
        }

        public List<OrderStatusChange> AdvanceOrders()
        {
            List<OrderStatusChange> changes =
                new List<OrderStatusChange>();

            foreach (Order order in orders)
            {
                string oldStatus = order.Status;
                string newStatus = oldStatus;

                if (oldStatus == "Σε επεξεργασία")
                {
                    newStatus = "Προς παράδοση";
                }
                else if (oldStatus == "Προς παράδοση")
                {
                    newStatus = "Παραδόθηκε";
                }

                if (newStatus != oldStatus)
                {
                    order.Status = newStatus;

                    changes.Add(new OrderStatusChange
                    {
                        Order = order,
                        PreviousStatus = oldStatus,
                        NewStatus = newStatus
                    });
                }
            }

            return changes;
        }

        public Order FindByNumber(int number)
        {
            return orders.FirstOrDefault(
                order => order.Number == number);
        }
    }

    public class OrderStatusChange
    {
        public Order Order { get; set; }
        public string PreviousStatus { get; set; }
        public string NewStatus { get; set; }
    }
}