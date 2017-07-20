using ChatService.Database;

using Messages;
using Messages.Events;

using NServiceBus;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService
{
    public class Program
    {
        public static void Main()
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            Console.Title = "Chat";

            //Create a new Endpoint configuration with the name "Authentication"
            EndpointConfiguration endpointConfiguration = new EndpointConfiguration("Chat");

            //Allows the endpoint to run installers upon startup. This includes things such as the creation of message queues.
            endpointConfiguration.EnableInstallers();
            //Instructs the queue to serialize messages with Json, should it need to serialize them
            endpointConfiguration.UseSerialization<JsonSerializer>();
            //Instructs the endpoint to use local RAM to store queues. TODO: Good during development, not during deployment (According to the NServiceBus tutorial)
            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            //Instructs the endpoint to send messages it cannot process to a queue named "error"
            endpointConfiguration.SendFailedMessagesTo("error");

            //Instructs the endpoint to use Microsoft Message Queuing TOD): Consider using RabbitMQ instead, only because Arcurve reccomended it. 
            var transport = endpointConfiguration.UseTransport<MsmqTransport>();
            //This variable is used to configure how messages are routed. Using this, you may set the default reciever of a particular command, and/or subscribe to any number of events
            var routing = transport.Routing();

            //Register to the MessageSent events published by the authentication endpoint
            routing.RegisterPublisher(typeof(MessageSent), "Authentication");

            //Start the endpoint with the configuration defined above.It should be noted that any changes made to the endpointConfiguration after an endpoint is instantiated will not apply to any endpoints that have already been instantiated
            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Debug.consoleMsg("Press Enter to exit.");
            string entry;

            do
            {
                entry = Console.ReadLine();

                switch (entry)
                {
                    case ("DELETEDB"):
                        ChatServiceDatabase.getInstance().deleteDatabase();
                        Debug.consoleMsg("Delete database attempt complete");
                        break;
                    case ("CREATEDB"):
                        ChatServiceDatabase.getInstance().createDB();
                        Debug.consoleMsg("Completed Database Creation Attempt.");
                        break;
                    default:
                        Debug.consoleMsg("Command not understood");
                        break;
                }
            } while (!entry.Equals(""));
        }
    }
}
