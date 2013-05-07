using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RestaurantKata
{
    public class Cashier : IOrderConsumer
    {
        private readonly IOrderConsumer nextStep;
        private readonly ConcurrentDictionary<string, Order> unpaidOrders = new ConcurrentDictionary<string, Order>();

        public Cashier(IOrderConsumer nextStep)
        {
            this.nextStep = nextStep;
        }

        public void Consume(Order order)
        {
            TotalOrder(order);
            SaveOrder(order);
            nextStep.Consume(order);
        }

        private void SaveOrder(Order order)
        {
            unpaidOrders[order.Id] = order;
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

        public IEnumerable<Order> GetOrdersReadyToPay()
        {
            return unpaidOrders.Values;
        }

        public void PayBill(Order order)
        {
            if (unpaidOrders.TryRemove(order.Id, out order))
            {
                order.Paid = true;
            }
            else
            {
                throw new Exception("Ahhhhh, this should never happen?");
            }
        }
    }
}