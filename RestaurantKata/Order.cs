﻿using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestaurantKata.Infrastructure;

namespace RestaurantKata
{
    public interface IMessage
    {
        string CorrelationId { get; }
    }
    public interface IEvent : IMessage {}
    public abstract class OrderMessageBase : IMessage
    {
        public Order Order { get; set; }
        public string CorrelationId { get { return Order.CorrelationId; } }
    }
    public abstract class OrderEventBase : OrderMessageBase, IEvent {}

    public class OrderPlaced : OrderEventBase {}
    public class OrderReadyToCook : OrderEventBase {}
    
    public class FoodCooked : OrderEventBase {}
    public class OrderReadyForPricing : OrderEventBase {}
    
    public class OrderPriced : OrderEventBase {}
    public class OrderReadyForPayment : OrderEventBase {}
    
    public class OrderPaid : OrderEventBase {}
    public class OrderCompleted : OrderEventBase {}

    public class OrderDropped : OrderEventBase
    {
    }

    public class CheckeFoodIsCooked : OrderEventBase
    {
    }


    [DataContract]
    public class Order : ExtensibleDynamicObject
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string CausationId { get; set; }
        [DataMember]
        public string CorrelationId { get { return Id; } }

        [DataMember]
        public string CustomerCategory { get; set; }
        [DataMember]
        public int TableNumber { get; set; }
        [DataMember]
        public string Server { get; set; }
        [DataMember]
        public decimal? Vat { get; set; }
        [DataMember]
        public decimal? Total { get; set; }
        [DataMember]
        public decimal? Subtotal { get; set; }
        [DataMember]
        public int? CookTime { get; set; }
        [DataMember]
        public Item[] Items { get; set; }
        [DataMember]
        public bool Paid { get; set; }
        [DataMember]
        public DateTime TimeToLive { get; set; }

        public bool IsCooked()
        {
            return CookTime.HasValue;
        }

        public bool IsPriced()
        {
            return Total.HasValue;
        }

        public bool IsPaid()
        {
            return Paid;
        }

        public bool IsCompleted()
        {
            return IsCooked() && IsPriced() && IsPaid();
        }

        public bool IsNew()
        {
            return !(IsCooked() && IsPriced() && IsPaid());
        }
    }

    [DataContract]
    public class Item
    {
        [DataMember]
        public string ItemDescription { get; set; }
        [DataMember]
        public decimal Quantity { get; set; }
        [DataMember]
        public decimal? Price { get; set; }
        [DataMember]
        public string[] Ingredients { get; set; }

        public Item(string itemDescription, decimal quantity)
        {
            ItemDescription = itemDescription;
            Quantity = quantity;
        }
    }

   

    public class WaitressOrder
    {
        public int TableNumber { get; set; }
        public string Server { get; set; }
    }

    public class CookOrder
    {
        private readonly dynamic _originalDocument;
        public dynamic OriginalDocument { get { return _originalDocument; } }

        public CookOrder(dynamic originalDocument)
        {
            _originalDocument = originalDocument;
        }

        public string TableNumber
        {
            get { return _originalDocument.TableNumber; }
            set { _originalDocument.TableNumber = value; }
        }
    }

    [TestFixture]
    public class Tests
    {
        [Test]
        public void DocumentIsPreserved()
        {
            var order = new WaitressOrder
                            {
                                TableNumber = 15,
                                Server = "Mike",
                            };

            var serializedWaitressOrder = JsonConvert.SerializeObject(order);

            dynamic deserializedWaitressOrder = JObject.Parse(serializedWaitressOrder);
            Assert.That(deserializedWaitressOrder.TableNumber.ToString(), Is.EqualTo("15"));

            var cook = new CookOrder(deserializedWaitressOrder);
            cook.TableNumber = "999";
            cook.OriginalDocument.AnotherProperty = 15M;
            var reserialized = JsonConvert.SerializeObject(cook.OriginalDocument);


        }
    }
}
