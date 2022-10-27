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
                JObject request = (JObject)JToken.Parse(acceptedOfferRequestFile);

                //Variables used to validate correct request fields (formats etc.).
                string socialSecurityNumber = request.SelectToken("socialSecurityNumber").ToString();
                string leadOfferId = request.SelectToken("leadOfferId").ToString();
                string email = request.SelectToken("email").ToString();
                string stateCode = request.SelectToken("stateCode").ToString();
                string grossMonthlyIncome = request.SelectToken("grossMonthlyIncome").ToString();
                string requestedLoanAmount = request.SelectToken("requestedLoanAmount").ToString();       

                //Validate correct request fields (formats etc.).
                Assert.IsTrue(int.TryParse(socialSecurityNumber, out int ssn), "Social Security Number is not a number.");
                Assert.IsTrue(socialSecurityNumber.Length == 9, "Social Security Number is not 9 digits long");
                Assert.IsNotNull(leadOfferId, "Lead Offer Id is null");
                Assert.IsTrue(api.IsEmailValid(email), "Email address given is not in email format.");
                int.TryParse(stateCode, out int stateCodeIsNumber);

                //If out variable is 0, then value is not a number.
                Assert.AreEqual(0, stateCodeIsNumber, "State Code given is a number.");
                
                //Then ok to check if it's two character length.  If they mess up the state code to be "11" then no reason to check for two characters.
                Assert.IsTrue(stateCode.Length == 2, "State code is not two characters long. ");
                Assert.IsTrue(int.TryParse(grossMonthlyIncome,out int grossMonthly), "Gross Monthly Income is not a number.");
                Assert.IsTrue(int.TryParse(requestedLoanAmount, out int requestedLoanAmt), "Gross Monthly Income is not a number.");

                //Make request to Offer api!
                JObject response = api.requestOffer(acceptedOfferRequestFile);

                //Variables used to validate correct response fields (formats etc.)
                string accepted = response.GetValue("accepted").ToString();
                string code = response.GetValue("code").ToString();
                string status = response.GetValue("status").ToString();
                int amount = Convert.ToInt32(response.SelectToken("offers[0].amount"));
                double representativeAPR = Convert.ToDouble(response.SelectToken("offers[0].representativeAPR"));
                int interestRate = Convert.ToInt32(response.SelectToken("offers[0].interestRate"));
                int term = Convert.ToInt32(response.SelectToken("offers[0].term"));
                int monthlyPayment =Convert.ToInt32(response.SelectToken("offers[0].monthlyPayment"));

                //Expected values (static values) are based on what I received from the API when testing this.
                Assert.AreEqual("True", accepted, "accepted values did not match.");
                Assert.AreEqual("201", code, "code values did not match.");
                Assert.AreEqual("APPROVED", "APPROVED status was not reached.");
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
                JObject request = (JObject)JToken.Parse(declinedOfferRequestFile);

                //Validate correct request fields (formats etc.).
                string socialSecurityNumber = request.GetValue("socialSecurityNumber").ToString();
                string leadOfferId = request.GetValue("leadOfferId").ToString();
                string email = request.GetValue("email").ToString();
                string stateCode = request.GetValue("stateCode").ToString();
                string grossMonthlyIncome = request.GetValue("grossMonthlyIncome").ToString();
                string requestedLoanAmount = request.GetValue("requestedLoanAmount").ToString();


                Assert.IsTrue(int.TryParse(socialSecurityNumber, out int ssn), "Social Security Number is not a number.");
                Assert.IsTrue(socialSecurityNumber.Length == 9, "Social Security Number is not 9 digits long");
                Assert.IsNotNull(leadOfferId, "Lead Offer Id is null");
                Assert.IsTrue(api.IsEmailValid(email), "Email address given is not in email format.");
                int.TryParse(stateCode, out int stateCodeIsNumber);

                //If out variable is 0, then value is not a number.
                Assert.AreEqual(0, stateCodeIsNumber, "State Code given is a number.");

                //Then ok to check if it's two character length.  If they mess up the state code to be "11" then no reason to check for two characters.
                Assert.IsTrue(stateCode.Length == 2, "State code is not two characters long. ");
                Assert.IsTrue(int.TryParse(grossMonthlyIncome, out int grossMonthly), "Gross Monthly Income is not a number.");
                Assert.IsTrue(int.TryParse(requestedLoanAmount, out int requestedLoanAmt), "Gross Monthly Income is not a number.");

                //Expected values (static values) are based on what I received from the API when testing this.
                JObject response = api.requestOffer(declinedOfferRequestFile);
                Assert.AreEqual("False", response.GetValue("accepted").ToString());
                Assert.AreEqual("315", response.GetValue("code").ToString());
                Assert.AreEqual("DECLINED", response.GetValue("status").ToString());
                Assert.IsNull(response.GetValue("offers").ToString(), "Offers exist for this request.");
            }
            catch (Exception e)
            {
                Debug.WriteLine("error: " + e);
                Assert.Fail();
            }
        }
        [TestMethod]
        public void OfferNoDecision()
        {
            OppFiApi api = new OppFiApi();
            var noDecisionRequest = File.ReadAllText("RequestSchemas/noDecisionRequest.json");
            try
            {
                //Validate correct request fields (formats etc.).
                JObject request = (JObject)JToken.Parse(noDecisionRequest);
                Assert.IsTrue(request.GetValue("socialSecurityNumber").ToString().Length == 9);
                Assert.IsNotNull(request.GetValue("leadOfferId"));
                Assert.IsTrue(api.IsEmailValid(request.GetValue("email").ToString()));
                Assert.IsTrue(request.GetValue("stateCode").ToString().Length == 2);
                Assert.IsTrue(int.TryParse(request.GetValue("grossMonthlyIncome").ToString(), out int grossMonthlyIncome));
                Assert.IsTrue(int.TryParse(request.GetValue("requestedLoanAmount").ToString(), out int requestedLoanAmount));

                //Expected values (static values) are based on what I received from the API when testing this.
                JObject response = api.requestOffer(noDecisionRequest);
                Assert.AreEqual("false", response.GetValue("accepted").ToString());
                Assert.AreEqual(402, response.GetValue("code"));
                Assert.AreEqual("DECLINED", response.GetValue("status").ToString());
            }
            catch (Exception e)
            {
                Debug.WriteLine("error: " + e);
            }
        }
    }
}