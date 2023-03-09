using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;

namespace HCIN_SWP_Alexa_VVT.Models
{
    public class VVT_API
    {
        List<VVT_Station> Stations;

        //public List<VVT_Station> GetAllStations() {
        public void GetAllStations() {


            //Call Api
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://smartinfo.ivb.at/api/JSON");
            var response = client.GetAsync("/STOPS").Result;

            if (response.IsSuccessStatusCode) {
                var jsonString = response.Content.ReadAsStringAsync();
                jsonString.Wait();
            }

    }
}
