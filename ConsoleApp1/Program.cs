using System;
using System.Net.Http;
using System.Threading;

namespace Ex1
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient client = new HttpClient();
            var urlList = new string[]
            {
                "https://portal.reivernet.com/portal/",
                "http://localhost",
                "http://localhost",
                "http://localhost"
            };
            var cancellationTokenSource = new CancellationTokenSource();
            try
            {
                Ex1 worker = new Ex1(client,cancellationTokenSource.Token);               
                var length =  worker.GetWebResourceLength(urlList).Result;
                Console.WriteLine("Aggregated length: {0}", length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                client.Dispose();
            }

            Console.ReadKey();
        }
    }
}
