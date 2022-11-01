using Microsoft;
using Newtonsoft.Json.Linq;
using OppFiAssessment.ApiMethods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace OppFiAssessment.Tests
{
    [TestClass]
    public class OppFiDemoApiTests
    {
        [TestMethod]
        public void OfferAccepted()
        {
            OppFiApi api = new OppFiApi();
            var acceptedOfferRequestFile = File.ReadAllText("RequestSchemas/acceptedOfferRequest.json");
            try
            {
                //Make request to Offer api!
                JObject response = api.requestOffer(acceptedOfferRequestFile);

                //Variables used to validate correct response fields (formats etc.)
                string accepted = response.GetValue("accepted").ToString();
                string code = response.GetValue("code").ToString();
                string status = response.GetValue("status").ToString();
                int amount = Convert.ToInt32(response.SelectToken("offers[0].amount").ToString());
                double representativeAPR = Convert.ToDouble(response.SelectToken("offers[0].representativeAPR").ToString());
                int interestRate = Convert.ToInt32(response.SelectToken("offers[0].interestRate").ToString());
                int term = Convert.ToInt32(response.SelectToken("offers[0].term").ToString());
                int monthlyPayment =Convert.ToInt32(response.SelectToken("offers[0].monthlyPayment").ToString());

                //Expected values (static values) are based on what I received from the API when testing this.
                Assert.AreEqual("True", accepted, "accepted values did not match.");
                Assert.AreEqual("201", code, "code values did not match.");
                Assert.AreEqual("APPROVED", status, "APPROVED status was not reached.");
                Assert.IsNotNull(response.GetValue("offers").ToString(), "No offers exist for this request.");
                Assert.AreEqual(1400, amount, "Approved loan amount is not correct.");
                Assert.AreEqual(159.99, representativeAPR, "Approved representative APR is not correct.");
                Assert.AreEqual(160, interestRate, "Approved interest rate is not correct");
                Assert.AreEqual(10,term, "Approved term is not correct.");
                Assert.AreEqual(261, monthlyPayment , "Approved monthly payment is not correct.");
            }
            catch (Exception e)
            {
                Debug.WriteLine("error: " + e);
                Assert.Fail();
            }
        }
        [TestMethod]
        public void OfferDeclined()
        {
            OppFiApi api = new OppFiApi();
            var declinedOfferRequestFile = File.ReadAllText("RequestSchemas/declinedOfferRequest.json");
            try
            {
                
                //Expected values (static values) are based on what I received from the API when testing this.
                JObject response = api.requestOffer(declinedOfferRequestFile);

                Assert.AreEqual("False", response.GetValue("accepted").ToString(), "Accepted values do not match.");
                Assert.AreEqual("315", response.GetValue("code").ToString(), "Response parameter code doesn't match expect code value.");
                Assert.AreEqual("DECLINED", response.GetValue("status").ToString(), "Status does not equal Declined.");
                Assert.IsNull(response.GetValue("offers"), "Offers exist for this request.");
            }
            catch (Exception e)
            {
                Debug.WriteLine("error: " + e);
                Assert.Fail("Test failed one of the asserts or another error outside of the asserts.");
            }
        }
        [TestMethod]
        public void OfferNoDecision()
        {
            OppFiApi api = new OppFiApi();
            var noDecisionRequest = File.ReadAllText("RequestSchemas/noDecisionRequest.json");
            try
            {
                //Expected values (static values) are based on what I received from the API when testing this.
                JObject response = api.requestOffer(noDecisionRequest);
                Assert.AreEqual("False", response.GetValue("accepted").ToString(), "Accepted values do not match.");
                Assert.AreEqual("402", response.GetValue("code").ToString(), "Response parameter code doesn't match expected code value.");
                Assert.AreEqual("DECLINED", response.GetValue("status").ToString(), "Status does not equal DECLINED");
            }
            catch (Exception e)
            {
                Debug.WriteLine("error: " + e);
            }
        }
    }
}