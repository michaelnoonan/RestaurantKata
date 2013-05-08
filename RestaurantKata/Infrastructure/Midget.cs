﻿using System;
using RestaurantKata.Infrastructure.Alarm;

namespace RestaurantKata.Infrastructure
{
    public class Midget :
        IConsume<FoodCooked>,
        IConsume<OrderPriced>,
        IConsume<OrderPaid>,
        IConsume<OrderDropped>
    {
        private bool isCooked;
        private bool isDropped;

        public string CorrelationId { get; private set; }

        public Midget(string correlationId)
        {
            CorrelationId = correlationId;
        }

        public void StartOrder(Order order)
        {
            TopicPubSub.Instance.Publish(Topics.CooksQueue, new OrderReadyToCook { Order = order});
            TopicPubSub.Instance.Publish(Topics.WakeUpCalls, new WakeUpCall(DateTime.Now.AddSeconds(10), () =>
            {
                if (!isCooked && !isDropped)
                {
                    Logger.Warn("Order is being retried. Id {0}", order.Id);
                    StartOrder(order);
                }
            }));                        
        }

        public bool Consume(FoodCooked message)
        {
            isCooked = true;
            TopicPubSub.Instance.Publish(Topics.Pricing, new OrderReadyForPricing { Order = message.Order });
            return true;
        }

        public bool Consume(OrderPriced message)
        {
            TopicPubSub.Instance.Publish(Topics.Payment, new OrderReadyForPayment { Order = message.Order });
            return true;
        }

        public bool Consume(OrderPaid message)
        {
            TopicPubSub.Instance.Publish(Topics.CompletedOrders, new OrderCompleted { Order = message.Order });
            return true;
        }

        public bool Consume(OrderDropped message)
        {
            isDropped = true;
            TopicPubSub.Instance.Publish(Topics.CompletedOrders, new OrderCompleted { Order = message.Order });
            return true;
        }
    }
}