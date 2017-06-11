using System.Net;

namespace ClientApplicationMVC.Models
{
    public static class ServiceBusInfo
    {
        public const int serverPort = 11000;
        public const string serverHostName = "127.0.0.1";
        public static IPAddress ipAddress = IPAddress.Parse(serverHostName);
        public static IPEndPoint serverEndPoint = new IPEndPoint
            (ipAddress, serverPort);
    }
}