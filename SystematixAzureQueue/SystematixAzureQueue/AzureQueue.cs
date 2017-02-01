using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Queue; // Namespace for Queue storage types

namespace SystematixAzureQueue
{
    public class AzureQueue
    {
        // Parse the connection string and return a reference to the storage account.
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

        public async Task AddBookingQueue(string bookingJson, int shopid)
        {
            try
            {
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a container.
                CloudQueue queue = queueClient.GetQueueReference("bookingqueue" + shopid);

                // Create the queue if it doesn't already exist
                if (await queue.CreateIfNotExistsAsync())
                {
                    Console.WriteLine("Queue '{0}' Created", queue.Name);
                }
                else
                {
                    Console.WriteLine("Queue '{0}' Exists", queue.Name);
                }
                // Create a message and add it to the queue.
                CloudQueueMessage message = new CloudQueueMessage(bookingJson);
                // Async enqueue the message
                await queue.AddMessageAsync(message);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Get Pending Booking Print Queue from Azure Queue and print in booking printer, then delete that queue from auzre, 
        /// Make sure internet connection before start this process.
        /// </summary>
        public async Task<bool> PrintPendingBooking(int shopid)
        {
            try
            {
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a container.
                CloudQueue queue = queueClient.GetQueueReference("bookingqueue" + shopid);

                // Create the queue if it doesn't already exist
                if (await queue.CreateIfNotExistsAsync())
                {
                    Console.WriteLine("Queue '{0}' Created", queue.Name);
                }
                else
                {
                    Console.WriteLine("Queue '{0}' Exists", queue.Name);
                }

                foreach (CloudQueueMessage message in queue.GetMessages(20, TimeSpan.FromMinutes(5)))
                {
                    // Print Pending Queue
                    bool bl = PrintBookingDetails(message.AsString);

                    if (bl)
                    {
                        // Process all messages in less than 5 minutes, deleting each message after processing.
                        await queue.DeleteMessageAsync(message);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private bool PrintBookingDetails(string modelJson)
        {
            // Print Booking Details.
            //int times = 0;
            //BookingPrinter printer = new BookingPrinter(model.Fk_ShopId.Value, out times);
            //bool bl = false;
            //for (int i = 1; i <= times; i++)
            //{
            //    bl = printer.Print(times, model);
            //}
            return true;
        }

        #region Implementation Practices
        public void DequeMessage()
        {
            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the queue client
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            CloudQueue queue = queueClient.GetQueueReference("myqueue");

            // Get the next message
            CloudQueueMessage retrievedMessage = queue.GetMessage();

            //Process the message in less than 30 seconds, and then delete the message
            queue.DeleteMessage(retrievedMessage);
        }
        public async Task DequeMessageAsync(int shopid)
        {
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a container.
            CloudQueue queue = queueClient.GetQueueReference("bookingqueue" + shopid);

            // Create the queue if it doesn't already exist
            if (await queue.CreateIfNotExistsAsync())
            {
                Console.WriteLine("Queue '{0}' Created", queue.Name);
            }
            else
            {
                Console.WriteLine("Queue '{0}' Exists", queue.Name);
            }

            // Create a message to put in the queue
            CloudQueueMessage cloudQueueMessage = new CloudQueueMessage("My message");

            // Async enqueue the message
            await queue.AddMessageAsync(cloudQueueMessage);
            Console.WriteLine("Message added");

            // Async dequeue the message
            CloudQueueMessage retrievedMessage = await queue.GetMessageAsync();
            Console.WriteLine("Retrieved message with content '{0}'", retrievedMessage.AsString);


            foreach (CloudQueueMessage message in queue.GetMessages(20, TimeSpan.FromMinutes(5)))
            {
                // Process all messages in less than 5 minutes, deleting each message after processing.
                queue.DeleteMessage(message);
            }

            // Async delete the message
            await queue.DeleteMessageAsync(retrievedMessage);
            Console.WriteLine("Deleted message");
        }
        public void GetQueueLength()
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the queue client.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue.
            CloudQueue queue = queueClient.GetQueueReference("myqueue");

            // Fetch the queue attributes.
            queue.FetchAttributes();

            // Retrieve the cached approximate message count.
            int? cachedMessageCount = queue.ApproximateMessageCount;

            // Display number of messages.
            Console.WriteLine("Number of messages in queue: " + cachedMessageCount);
        }

        public void DeleteQueue()
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the queue client.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue.
            CloudQueue queue = queueClient.GetQueueReference("myqueue");

            // Delete the queue.
            queue.Delete();
        }
        #endregion
    }
}
