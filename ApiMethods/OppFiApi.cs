using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace OppFiAssessment.ApiMethods
{
    public class OppFiApi
    {
        //HttpClient client;

        public string targetUrl = "";
        public string apiKey = "";
        public OppFiApi()
        {
            StreamReader file = File.OpenText(@"config.json");
            JsonTextReader reader = new JsonTextReader(file);

            JObject headerValues = (JObject)JToken.ReadFrom(reader);
            targetUrl = headerValues.GetValue("TARGET_URL").ToString();
            apiKey = headerValues.GetValue("API_KEY").ToString();
        }

        public JObject requestOffer(string jsonRequest)
        {
            RestClient client = new RestClient(targetUrl);
            JObject responseJSON = new JObject();
            try
            {
                var request = new RestRequest()
                .AddHeader("x-api-key", apiKey);
                request.AddBody(jsonRequest);
                
                var response = client.PostAsync(request).Result;
                
                System.Console.WriteLine("Test");
                if (response.IsSuccessStatusCode)
                {
                    responseJSON = (JObject)JToken.Parse(response.Content);
                    System.Console.WriteLine("test");
                }

            }
            catch(Exception ex)
            {
                Debug.WriteLine("ERROR: " + ex.Message);
                Assert.Fail();
            }
            return responseJSON;
        }

        public bool IsEmailValid(string email)
        {
            var valid = true;

            try
            {
                var emailAddress = new MailAddress(email);
            }
            catch
            {
                valid = false;
            }

            return valid;
        }
    }
}
