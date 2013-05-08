using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantKata.Infrastructure
{
    public class ThreadedConsumer<TMessage> : IStartable, IConsume<TMessage>, IHaveAQueue
        where TMessage : IMessage
    {
        public IConsume<TMessage> Consumer { get { return consumer; } }
        public string Name 
        {
            get { return name; }
        }

        public int CountOfItemsInQueue 
        {
            get
            {
                return messagesToProcess.Count;
            }
        }

        private readonly string name;
        private readonly IConsume<TMessage> consumer;
        private readonly int queueLimit;
        private readonly ConcurrentQueue<TMessage> messagesToProcess = new ConcurrentQueue<TMessage>();

        public ThreadedConsumer(string name, IConsume<TMessage> consumer, int queueLimit = 10)
        {
            this.name = name;
            this.consumer = consumer;
            this.queueLimit = queueLimit;
        }

        public void Start()
        {
            Task.Run((Action)StartProcessingMessages);
        }

        private void StartProcessingMessages()
        {
            while (true)
            {
                TMessage message;
                if (messagesToProcess.TryDequeue(out message))
                {
                    consumer.Consume(message);
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        public bool Consume(TMessage message)
        {
            if (messagesToProcess.Count >= queueLimit) return false;
            messagesToProcess.Enqueue(message);
            return true;
        }
    }
}