using Autofac;
using Autofac.Integration.Mvc;

using NServiceBus;

using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ClientApplicationMVC
{
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// The endpoint instance being used by the controllers to connect to the bus
        /// </summary>
        IEndpointInstance endpoint;

        protected void Application_End()
        {
            endpoint?.Stop().GetAwaiter().GetResult();
        }

        /// <summary>
        /// In charge of the configuration and initialization of the NServiceBus Endpoint, as well as the controllers that will use them.
        /// Also in charge of some other configurations placed by default 
        /// </summary>
        protected void Application_Start()
        {
            //This container is responsible for configuring the dependencies and settings required for the controllers to make use of NServiceBus
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);//Registers the MVC controllers

            //Set the dependency resolver to be Autofac
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            //The following configures the settings for the endpoint that will be used to communicate with the service bus
            #region configureAndInitializeEndpoint
            var endpointConfiguration = new EndpointConfiguration("UserClient");
            endpointConfiguration.SendFailedMessagesTo("error");//Sets the queue to send messages the endpoint cannot process to the given queue
            endpointConfiguration.MakeInstanceUniquelyAddressable("1");//Ensures that each client's endpoint will be uniquely identifiable from the next (I think??)
            endpointConfiguration.UseSerialization<JsonSerializer>();//Sets the type of serialization to be used by the endpoint
            endpointConfiguration.UseContainer<AutofacBuilder>(//Sets the container the endpoint will be using to the one created above
                customizations: customizations =>
                {
                    customizations.ExistingLifetimeScope(container);
                });
            endpointConfiguration.UsePersistence<InMemoryPersistence>();//Sets the method by which the endpoint will store information
            endpointConfiguration.UseTransport<MsmqTransport>();//Sets the type of queue to be used by the endpoint.
            endpointConfiguration.EnableCallbacks();
            
            endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();//Creates an instance of the endpoint using the configuration above
            #endregion
            
            var updater = new ContainerBuilder();
            updater.RegisterInstance(endpoint);//Registers the newly created endpoint with the container
            updater.RegisterControllers(typeof(MvcApplication).Assembly);
            var updated = updater.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(updated));//Set the resolver to be the newly updated container

            AreaRegistration.RegisterAllAreas();//Added by VS upon creation of project
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);//Added by VS upon creation of project
            RouteConfig.RegisterRoutes(RouteTable.Routes);//Added by VS upon creation of project
            BundleConfig.RegisterBundles(BundleTable.Bundles);//Added by VS upon creation of project
        }
    }
}
