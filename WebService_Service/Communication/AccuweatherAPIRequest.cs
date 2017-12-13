using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceService.Communication
{
    public partial class AccuweatherAPIRequest
    {
        /// <summary>
        /// The purpose of this function is to get one of the location keys for the given city
        /// </summary>
        /// <returns>A Location key for calgary</returns>
        public string getLocationKey(string city)
        {
            //If the program is not already aware of the location key it will make a call to the web API to find it.
            if (calgaryLocationKey == null)
            {
                //Sets the path and parameters for the call
                string path = "locations/v1/cities/search?"
                    + "apikey=" + apiKey
                    + "&q=" + city;

                HttpResponseMessage response = null;

                try
                {
                    //Makes the call to the web API and awaits the response from the server
                    response = webCaller.GetAsync(path).GetAwaiter().GetResult();
                }
                catch (HttpRequestException e)
                {
                    return "Unable to make call to web API. Exception message: " + e.Message;
                }

                //These three lines parse the response from the server. For now, we only care about the location key
                string locationKey = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                int keyStart = locationKey.IndexOf("\"Key\"") + 7;
                locationKey = locationKey.Substring(keyStart, locationKey.IndexOf('"', keyStart) - keyStart);

                calgaryLocationKey = locationKey;
            }

            return calgaryLocationKey;
        }

        /// <summary>
        /// Returns the daily forcast for the given city in an unparsed JSON format received from the web API.
        /// </summary>
        /// <returns>Daily forcast for the given city in an unparsed JSON format received from the web API.</returns>
        public string getDailyWeatherForecast(string city)
        {
            string locationKey = getLocationKey(city);
            string path = $"forecasts/v1/daily/1day/{locationKey}?"
                + $"apikey={apiKey}&metric=true";

            HttpResponseMessage response = null;

            try
            {
                //Make the call to the web api
                response = webCaller.GetAsync(path).GetAwaiter().GetResult();
            }
            catch (HttpRequestException e)
            {
                return "Unable to make call to web API. Exception message: " + e.Message;
            }

            string result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            return result;
        }



    }

    public partial class AccuweatherAPIRequest
    {
        /// <summary>
        /// This is the address used to access the web API
        /// </summary>
        private const string baseAddress = "http://dataservice.accuweather.com/";
        
        /// <summary>
        /// This is our application specific key used to authenticate ourselves with the web API
        /// </summary>
        private const string apiKey = "PGgBLiijTQQWVrBrkXhJqdYA67v95DTy";

        /// <summary>
        /// This is the class responsible for making our request to the API
        /// </summary>
        private static readonly HttpClient webCaller = initializeWebCaller();

        /// <summary>
        /// The location key for calgary, saved to reduce the number of calls we make to the web API
        /// </summary>
        private string calgaryLocationKey = null;

        /// <summary>
        /// This function sets up the class that makes the calls to the API
        /// </summary>
        /// <returns></returns>
        private static HttpClient initializeWebCaller()
        {
            HttpClient webCaller = new HttpClient();
            webCaller.BaseAddress = new Uri(baseAddress);
            webCaller.DefaultRequestHeaders.Accept.Clear();
            webCaller.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return webCaller;
        }
    }
}