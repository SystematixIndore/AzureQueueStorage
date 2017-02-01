using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystematixAzureQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            AzureQueue q = new AzureQueue();
            Task task = Task.Run(async () =>
            {
                await q.AddBookingQueue("booking json Hello world", 6);
                await q.AddBookingQueue("booking json Hello world", 7);
                await q.AddBookingQueue("booking json Hello world", 8);
                await q.AddBookingQueue("booking json Hello world", 8);

                await q.PrintPendingBooking(6);

            });
            task.Wait();
        }
    }
}
