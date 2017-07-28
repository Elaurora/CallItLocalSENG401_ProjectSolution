using AuthenticationService.Database;

using NServiceBus;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {

            Console.Title = "Authentication";// Give the console a title so it is easier to tell them apart

            //Create a new Endpoint configuration with the name "Authentication"
            EndpointConfiguration endpointConfiguration = new EndpointConfiguration("Authentication");

            //Allows the endpoint to run installers upon startup. This includes things such as the creation of message queues.
            endpointConfiguration.EnableInstallers();
            //Instructs the queue to serialize messages with Json, should it need to serialize them
            endpointConfiguration.UseSerialization<JsonSerializer>();
            //Instructs the endpoint to use local RAM to store queues. TODO: Good during development, not during deployment (According to the NServiceBus tutorial)
            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            //Instructs the endpoint to send messages it cannot process to a queue named "error"
            endpointConfiguration.SendFailedMessagesTo("error");
            //Allows the endpoint to make requests to other endpoints and await a response.
            endpointConfiguration.EnableCallbacks();

            //Instructs the endpoint to use Microsoft Message Queuing TOD): Consider using RabbitMQ instead, only because Arcurve reccomended it. 
            var transport = endpointConfiguration.UseTransport<MsmqTransport>();
            //This variable is used to configure how messages are routed. Using this, you may set the default reciever of a particular command, and/or subscribe to any number of events
            var routing = transport.Routing();

            endpointConfiguration.MakeInstanceUniquelyAddressable("1");

            //Start the endpoint with the configuration defined above. It should be noted that any changes made to the endpointConfiguration after an endpoint is instantiated will not apply to any endpoints that have already been instantiated
            var eventRaisingEndpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Server serverConnection = new Server(eventRaisingEndpoint);

            Thread serverThread = new Thread(new ThreadStart(serverConnection.StartListening));

            serverThread.Start();//Start the server

            Messages.Debug.consoleMsg("Press Enter to exit.");
            string entry;

            do
            {
                entry = Console.ReadLine();

                switch (entry)
                {
                    case ("DELETEDB"):
                        AuthenticationDatabase.getInstance().deleteDatabase();
                        Messages.Debug.consoleMsg("Delete database attempt complete");
                        break;
                    case ("CREATEDB"):
                        AuthenticationDatabase.getInstance().createDB();
                        Messages.Debug.consoleMsg("Completed Database Creation Attempt.");
                        break;
                    default:
                        Messages.Debug.consoleMsg("Command not understood");
                        break;
                }
            } while (!entry.Equals(""));
        }


    }
}
