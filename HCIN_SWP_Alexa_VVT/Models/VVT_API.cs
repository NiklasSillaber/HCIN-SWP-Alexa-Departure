using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Xml.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json.Linq;

namespace HCIN_SWP_Alexa_VVT.Models
{
    public class VVT_API
    {
        public List<VVT_Station> Stations { get; set; }

        public VVT_API() { 
            this.Stations = GetAllStations();
        }
        public string GetDepartures(string station)
        {
            int uid = GetUid(station);
            List<Departure> departures = GetInfos(uid);
            string info = "";
            foreach(Departure de in departures) {
                info += "Route " + de.Route + " Richtung " + de.Direction + " Zeit " + de.Time + " ";
            }
            return info;
        }

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

                    if (obj["stop"] != null) {

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
            }
            return stations;
        }

        public int GetUid(string StationName)
        {
            foreach (VVT_Station Station in Stations) {
                if (StationName == Station.Name) {
                    return Station.StationId;
                }
            }
            return -1;
        }

        public List<Departure> GetInfos(int Uid)
        {

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://smartinfo.ivb.at");
            var response = client.GetAsync("api/JSON/PASSAGE?stopID=" + Uid).Result;

            List<Departure> departures = new List<Departure>();

            if (response.IsSuccessStatusCode) {
                var jsonString = response.Content.ReadAsStringAsync();
                jsonString.Wait();

                JArray array = JArray.Parse(jsonString.Result);
                foreach (JObject obj in array.Children<JObject>()) {

                    departures.Add(
                        new Departure() {
                            Route = (string)obj["smartinfo"]["route"],
                            Direction = (string)obj["smartinfo"]["direction"],
                            Time = (string)obj["smartinfo"]["time"]
                        }
                    );
                }

            }

            return departures;
        }
    }
}
