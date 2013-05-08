using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using RestaurantKata.Infrastructure;

namespace RestaurantKata
{
    public class Cashier : IConsume<OrderReadyForPayment>
    {
        private readonly ConcurrentDictionary<string, Order> unpaidOrders = new ConcurrentDictionary<string, Order>();

        public bool Consume(OrderReadyForPayment message)
        {
            TotalOrder(message.Order);
            SaveOrder(message.Order);
            return true;
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
                TopicPubSub.Instance.Publish(order.CorrelationId, new OrderPaid { Order = order });
            }
            else
            {
                throw new Exception("Ahhhhh, this should never happen?");
            }
        }
    }
}