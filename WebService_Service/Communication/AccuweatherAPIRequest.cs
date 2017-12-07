using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebService_Service.Communication
{
    public partial class AccuweatherAPIRequest
    {
        public string getCalgaryLocationKey()
        {
            string path = "locations/v1/cities/search?"
                + "apikey=" + apiKey
                + "&q=calgary";
            
            HttpResponseMessage response = null;

            try
            {
                response = webCaller.GetAsync(path).GetAwaiter().GetResult();
            }
            catch(HttpRequestException e)
            {
                return "Unable to make call to web API. Exception message: " + e.Message;
            }

            string locationKey = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            int keyStart = locationKey.IndexOf("\"key\"") + 8;
            locationKey = locationKey.Substring(keyStart, locationKey.IndexOf('"', keyStart) - keyStart);

            return locationKey;
        }

        public string getCalgaryDailyWeatherForecast()
        {
            string locationKey = getCalgaryLocationKey();
            string path = $"forecasts/v1/daily/1day/{locationKey}?"
                + $"apikey={apiKey}";

            HttpResponseMessage response = null;

            try
            {
                response = webCaller.GetAsync(path).GetAwaiter().GetResult();
            }
            catch (HttpRequestException e)
            {
                return "Unable to make call to web API. Exception message: " + e.Message;
            }

            return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }



    }

    public partial class AccuweatherAPIRequest
    {
        private const string baseAddress = "http://dataservice.accuweather.com/";
        
        private const string apiKey = null;

        private static readonly HttpClient webCaller = initializeWebCaller();

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