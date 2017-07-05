using CompanyDirectoryService.Database;

using Messages.Events;

using NServiceBus;

using System;
using System.Threading.Tasks;

namespace CompanyDirectoryService
{
    class Program
    {
        static void Main()
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            Console.Title = "CompanyDirectory";

            //Create a new Endpoint configuration with the name "CompanyDirectory"
            var endpointConfiguration = new EndpointConfiguration("CompanyDirectory");

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

            routing.RegisterPublisher(typeof(AccountCreated), "Authentication");//Register to events of type AccountCreated from Authentication endpoint

            //Start the endpoint with the configuration defined above. It should be noted that any changes made to the endpointConfiguration after an endpoint is instantiated will not apply to any endpoints that have already been instantiated
            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine("Press Enter to exit.");
            string entry;

            do
            {
                entry = Console.ReadLine();

                switch (entry)
                {
                    case ("DELETEDB"):
                        CompanyDirectoryDB.getInstance().deleteDatabase();
                        Console.WriteLine("Delete database attempt complete");
                        break;
                    case ("CREATEDB"):
                        CompanyDirectoryDB.getInstance().createDB();
                        Console.WriteLine("Completed Database Creation Attempt.");
                        break;
                    default:
                        Console.WriteLine("Command not understood");
                        break;
                }
            } while (!entry.Equals(""));
            
            await endpointInstance.Stop().ConfigureAwait(false);
        }
    }
}
