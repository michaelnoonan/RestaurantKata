using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace RestaurantKata
{
    public class Cashier : IOrderConsumer
    {
        private readonly IOrderConsumer _nextStep;
        private readonly List<Order> _unpaidOrders = new List<Order>();

        public Cashier(IOrderConsumer nextStep)
        {
            _nextStep = nextStep;
        }

        public void Consume(Order order)
        {
            TotalOrder(order);
            SaveOrder(order);
            _nextStep.Consume(order);
        }

        private void SaveOrder(Order order)
        {
            _unpaidOrders.Add(order);
        }

        private void TotalOrder(Order order)
        {
            foreach (var item in order.Items)
            {
                order.Subtotal += item.Price.Value * item.Quantity;
            }
            order.Vat = order.Subtotal*0.1M;
            order.Total = order.Subtotal + order.Vat;
        }

        public void PayBill(int tableNumber, decimal amountPaid)
        {
            var order = _unpaidOrders.Single(x => x.TableNumber == tableNumber);
            if (order.Total <= amountPaid) order.Paid = true;
        }
    }
}