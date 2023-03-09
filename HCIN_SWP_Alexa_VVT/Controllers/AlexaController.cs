using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HCIN_SWP_Alexa_VVT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlexaController : Controller
    {
        [HttpPost, Route("/vvt")]
        public async Task<SkillResponse> Alexa(SkillRequest input)
        {
            var request = HttpContext.Request;

            bool isValid = ValidateRequest(request, input).Result;
            if (!isValid)
            {
                return null;
            }
            else
            {
                SkillResponse output = new SkillResponse();
                output.Version = "1.0";
                output.Response = new ResponseBody();

                switch (input.Request.Type)
                {

                    case "LaunchRequest":
                        output.Response.OutputSpeech = new PlainTextOutputSpeech("Hallo, du kannst mich nach den aktuellen Fahrplänen einer Haltestelle fragen.");
                        output.Response.ShouldEndSession = false;
                        break;

                    case "IntentRequest":
                        IntentRequest intentRequest = (IntentRequest)input.Request;

                        switch (intentRequest.Intent.Name)
                        {
                            case "vvt":
                                string StationName = intentRequest.Intent.Slots["stationName"].Value;
                                //CALL METHOD
                                output.Response.OutputSpeech = new PlainTextOutputSpeech("Ich habe keine Ahnung");
                                output.Response.ShouldEndSession = true;
                                break;
                            case "AMAZON.FallbackIntent":
                                output.Response.OutputSpeech = new PlainTextOutputSpeech("Sorry, das habe ich nicht verstanden");
                                output.Response.ShouldEndSession = false;
                                break;
                        }
                        break;
                }
                return output;
            }
        }

        private static async Task<bool> ValidateRequest(HttpRequest request, SkillRequest skillRequest)
        {
            request.Headers.TryGetValue("SignatureCertChainUrl", out var signatureChainUrl);
            if (string.IsNullOrWhiteSpace(signatureChainUrl))
            {
                return false;
            }

            Uri certUrl;
            try
            {
                certUrl = new Uri(signatureChainUrl);
            }
            catch
            {
                return false;
            }

            request.Headers.TryGetValue("Signature", out var signature);
            if (string.IsNullOrWhiteSpace(signature))
            {
                return false;
            }

            request.Body.Seek(0, SeekOrigin.Begin);
            string body;
            using (StreamReader stream = new StreamReader(request.Body))
            {
                body = stream.ReadToEnd();
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                return false;
            }

            bool isTimestampValid = RequestVerification.RequestTimestampWithinTolerance(skillRequest);
            bool valid = await RequestVerification.Verify(signature, certUrl, body);

            if (!valid || !isTimestampValid)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
    }
}
