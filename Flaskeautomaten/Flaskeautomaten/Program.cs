using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flaskeautomaten
{
    class Program
    {

        static void Main(string[] args)
        {
            BottleMachine bm = new BottleMachine();

            Thread trProducer = new Thread(new ThreadStart(bm.ProducerThread));
            Thread trConsumeThread = new Thread(new ThreadStart(bm.ConsumerThread));
            Thread trGiveConsumerBeverages = new Thread(new ThreadStart(bm.GiveConsumerBeverages));
            trProducer.Start();
            trConsumeThread.Start();
            trGiveConsumerBeverages.Start();

            Console.Read();
        }
    }

    class BottleMachine
    {
        private static Queue<Beverage> ProducerBuffer = new Queue<Beverage>(20);
        private static Queue<Beverage> SodaBuffer = new Queue<Beverage>(10);
        private static Queue<Beverage> BeerBuffer = new Queue<Beverage>(10);
        private static int serialNo = 0;

        public void ProducerThread()
        {
            while (true)
            {
                lock (ProducerBuffer)
                {
                        //Fill up the whole buffer with sodas and beers.
                        for (int i = 0; i < 20 - ProducerBuffer.Count; i+=2)
                        {
                            ProducerBuffer.Enqueue(new Soda(serialNo++));
                            ProducerBuffer.Enqueue(new Beer(serialNo++));
                            Console.WriteLine("Adding beer and soda");

                        }
                        //Announce that new beverages has been made.
                        Monitor.PulseAll(ProducerBuffer);
                }
            }
        }

        public void ConsumerThread()
        {
            while (true)
            {
                lock (ProducerBuffer)
                //Moving beverage from producerbuffer to consumerbuffer
                {

                    if (ProducerBuffer.Count == 0)
                    {
                        Monitor.Wait(ProducerBuffer);
                    }

                    Console.WriteLine("brews in the house");
                    //ProducerBuffer.Dequeue();

                    var item = ProducerBuffer.Dequeue();
                    //Console.WriteLine("Moving {0}: with S/N {1} from producerbuffer to consumerbuffer", item.ToString(),item.SerialNo);

                    if (item.GetType() == typeof(Soda))
                    {
                        lock (SodaBuffer)
                        {
                            SodaBuffer.Enqueue(item);
                        }
                    }
                    else if (item.GetType() == typeof(Beer))
                    {
                    lock (BeerBuffer)
                    {
                        BeerBuffer.Enqueue(item);
                    }

                    }
                    Console.WriteLine("Amount of beers in beerbuffer: " + BeerBuffer.Count);
                    Console.WriteLine("Amount of sodas in soadbuffer: " + SodaBuffer.Count);
                    //Thread.Sleep(5000);
                }
            }
        }

        public void GiveConsumerBeverages()
        {
            while (true)
            {
                if (BeerBuffer.Count == 0 %% SodaBuffer.Count == 0)
                {
                    Monitor.Wait(ProducerBuffer);
                }

                {
                    lock (BeerBuffer)
                    {
                        for (int i = 0; i < BeerBuffer.Count; i++)
                        {
                            Console.WriteLine("Giving beer to the consumer");
                        }
                    }
                }
            }
        }
    }
}
