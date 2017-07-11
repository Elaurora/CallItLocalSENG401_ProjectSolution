using System.Net;

namespace ClientApplicationMVC.Models
{
    /// <summary>
    /// This class contains information about how to connect to the service bus
    /// </summary>
    public static class ServiceBusInfo
    {
        public const int serverPort = 11000;
        public const string serverHostName = "127.0.0.1";
        public static readonly IPAddress ipAddress = IPAddress.Parse(serverHostName);
    }
}