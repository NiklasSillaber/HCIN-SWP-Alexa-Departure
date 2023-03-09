using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace HCIN_SWP_Alexa_VVT.Models
{
    public class VVT_API
    {
        List<VVT_Station> Stations;

        //public List<VVT_Station> GetAllStations() {
        public List<VVT_Station> GetAllStations()
        {
            //Call Api
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://smartinfo.ivb.at");
            var response = client.GetAsync("/api/JSON/STOPS").Result;

            List<VVT_Station> stations= new List<VVT_Station>();

            if (response.IsSuccessStatusCode) {
                var jsonString = response.Content.ReadAsStringAsync();
                jsonString.Wait();

                JArray array = JArray.Parse(jsonString.Result);
                foreach (JObject obj in array.Children<JObject>()) {

                    stations.Add(
                        new VVT_Station() {
                            StationId = Int32.Parse((string)obj["stop"]["uid"]),
                            Name = (string)obj["stop"]["name"],
                            Latitude = (string)obj["stop"]["lat"],
                            Longitude = (string)obj["stop"]["lon"]
                        }
                    );  

                }
            }

            return stations;

        }
    }
}
