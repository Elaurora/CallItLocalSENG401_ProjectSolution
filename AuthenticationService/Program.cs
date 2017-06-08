using NServiceBus;

using System;
using System.Threading.Tasks;

namespace AuthenticationService
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            Console.Title = "Authentication";

            var endpointConfiguration = new EndpointConfiguration("Authentication");//Create a new Endpoint configuration with the name "Authentication"
            endpointConfiguration.MakeInstanceUniquelyAddressable("1");
            //endpointConfiguration.EnableInstallers();//Allows the endpoint to run installers upon startup. This includes things such as 
            endpointConfiguration.UseSerialization<JsonSerializer>();//Instructs the queue to serialize messages with Json, should it need to serialize them
            endpointConfiguration.UsePersistence<InMemoryPersistence>();//Instructs the endpoint to use local RAM to store queues. TODO: Good during development, not during deployment (According to the NServiceBus tutorial)
            endpointConfiguration.UseTransport<MsmqTransport>();//Instructs the endpoint to use Microsoft Message Queuing TOD): Consider using RabbitMQ instead, only because Arcurve reccomended it. 

            endpointConfiguration.SendFailedMessagesTo("error");//Instructs the endpoint to send messages it cannot process to a queue named "error"

            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);//Start the endpoint with the configuration defined above.

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            await endpointInstance.Stop().ConfigureAwait(false);

        }
    }
}
