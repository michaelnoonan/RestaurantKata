using System;
using System.Collections.Concurrent;

namespace RestaurantKata
{
    public class Cashier : IOrderConsumer
    {
        private readonly IOrderConsumer _nextStep;
        private readonly ConcurrentDictionary<int, Order> _unpaidOrders = new ConcurrentDictionary<int, Order>();
        private readonly ConcurrentDictionary<int, Order> _paidOrders = new ConcurrentDictionary<int, Order>();

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
            _unpaidOrders[order.TableNumber] = order;
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
            Order order;
            if (_unpaidOrders.TryRemove(tableNumber, out order))
            {
                if (amountPaid >= order.Total)
                {
                    order.Paid = true;
                    _paidOrders[tableNumber] = order;
                }
            }
            else
            {
                throw new Exception("Ahhhhh, this should never happen?");
            }
        }

        public bool IsBillReadyToPay(int tableNumber)
        {
            return _unpaidOrders.ContainsKey(tableNumber);
        }
    }
}