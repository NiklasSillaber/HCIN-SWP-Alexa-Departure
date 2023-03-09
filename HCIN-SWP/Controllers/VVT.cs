using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace HCIN_SWP.Controllers
{
    public class VVT
    {
        public List<> GetFahrplan()
        {

            //Diese Daten an unsere WEbAPI (Microservice, Restful Service) senden
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7500");

            var response = client.GetAsync("/api/zimmer/zimmer").Result;

            if (response.IsSuccessStatusCode) {
                var jsonString = response.Content.ReadAsStringAsync();
                jsonString.Wait();
                var zimmer = JsonConvert.DeserializeObject<ObservableCollection<Zimmer>>(jsonString.Result);
                return zimmer;

            } else {
                return null;
            }

            //Meldung (Erfolg/Misserfolg) ausgeben
        }
    }
}
