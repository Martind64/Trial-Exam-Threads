using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DownloadWebPagesStartingPoint
{
    class Program
    {
        private static readonly string[] Addresses = 
        { "http://laerer.rhs.dk/andersb/", "http://easj.dk/", 
            "http://msdn.com/", "http://google.com/" };

        private static int GetNumberOfBytes(String url)
        {
            byte[] data = new WebClient().DownloadData(url);

            return data.Length;
        }

        static void Main(string[] args)
        {
            try
            {
                Sequential();
                usingThread();
                usingTask();
                usingParallel();
                usingPLINQ();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        private static void Sequential()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (string address in Addresses)
            {
                int bytes = GetNumberOfBytes(address);
                //Console.WriteLine("{0} {1} bytes", address, bytes);
            }
            stopwatch.Stop();
            Console.WriteLine("Sequential: {0}", stopwatch.Elapsed);
        }

        private static void usingThread()
        {
            List<Thread> threadList = new List<Thread>();
            
            Thread runningThread;

            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (string address in Addresses)
            {
                int bytes = 0;
                runningThread = new Thread(() => bytes = GetNumberOfBytes(address));
                //runningThread = new Thread(() => Console.WriteLine("Address: {0} Bytes: {1}", address, GetNumberOfBytes(address)));
                threadList.Add(runningThread);
                runningThread.Start();
            }

            foreach (var thread in threadList)
            {
                thread.Join();
            }

            stopwatch.Stop();
            Console.WriteLine("Using Thread: {0}", stopwatch.Elapsed);
        }

        private static void usingTask()
        {
            List<Task> taskList = new List<Task>();

            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (string address in Addresses)
            {
                int bytes = 0;
                Task task = new Task(() => bytes = GetNumberOfBytes(address));
                //Task task = new Task(() => Console.WriteLine("Address: {0} Bytes: {1}", address, GetNumberOfBytes(address)));
                taskList.Add(task);
                task.Start();
            }
            Task.WaitAll(taskList.ToArray());
            stopwatch.Stop();
            Console.WriteLine("Using Task: {0}", stopwatch.Elapsed);
        }

        private static void usingParallel()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            int bytes = 0;

            Parallel.ForEach(Addresses, address => bytes = GetNumberOfBytes(address));
            //Parallel.ForEach(Addresses, address => Console.WriteLine("Address: {0} Bytes: {1}", address, GetNumberOfBytes(address)));

            stopwatch.Stop();
            Console.WriteLine("Using Parallel: {0}", stopwatch.Elapsed);
        }

        private static void usingPLINQ()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            var bytes = from address in Addresses.AsParallel()
                        select GetNumberOfBytes(address);

            //var bytes = from address in Addresses.AsParallel()
            //            select address;

            //bytes.ForAll(address => Console.WriteLine(GetNumberOfBytes(address)));


            foreach (var b in bytes)
            {
                //Console.WriteLine(b);
            }

            stopwatch.Stop();
            Console.WriteLine("Using PLINQ: {0}", stopwatch.Elapsed);
        }
    }
}
