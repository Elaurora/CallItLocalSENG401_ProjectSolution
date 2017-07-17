using System;

namespace Messages
{
    public static class Debug
    {
        public static bool debug { get; set; } = true;

        public static void consoleMsg(string msg)
        {
            if (debug == true)
            {
                Console.WriteLine(msg);
            }
        }
    }
}
