using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RestaurantKata
{
    public class Order
    {
        public string CustomerCategory { get; set; }
        public int TableNumber { get; set; }
        public string Server { get; set; }
        public decimal Vat { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public int Cooktime { get; set; }
        public string Id { get; set; }
        public Item[] Items { get; set; }
    }

    public class Item
    {
        public Item(string itemDescription, decimal quantity)
        {
            ItemDescription = itemDescription;
            Quantity = quantity;
        }

        public string ItemDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal? Price { get; set; }
        public string[] Ingredients { get; set; }
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
            Assert.That(deserializedWaitressOrder.TableNumber, Is.EqualTo("15"));

            var cook = new CookOrder(deserializedWaitressOrder);
            cook.TableNumber = "999";
            cook.OriginalDocument.AnotherProperty = 15M;
            var reserialized = JsonConvert.SerializeObject(cook.OriginalDocument);


        }
    }
}
