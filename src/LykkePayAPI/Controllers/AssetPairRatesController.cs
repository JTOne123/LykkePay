
#define TEST_ASSERT_ENABLE


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Contracts.Pay;
using LykkePay.API.Code;
using LykkePay.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;



namespace LykkePay.API.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/assetPairRates")]
    public class AssetPairRatesController : BaseController
    {

        
        public AssetPairRatesController(PayApiSettings payApiSettings, HttpClient client, ILog log)
            : base(payApiSettings, client, log)
        {
            
        }




        [HttpGet("{assertId}")]
        public async Task<IActionResult> Get(string assertId)
        {
            if (string.IsNullOrEmpty(assertId))
            {
                return BadRequest();
            }
#if TEST_ASSERT_ENABLE
            if (assertId.Equals("BTCTEST", StringComparison.InvariantCultureIgnoreCase))
            {
                return Json(new AssertPairRate
                {
                    Accuracy = 3,
                    Ask = 6500.555,
                    Bid = 6400.444,
                    AssetPair = "BTCTEST"
                });
            }

#endif
            List<AssertPairRate> rates;
            try
            {
                var rateServiceUrl = $"{PayApiSettings.Services.PayServiceService.TrimDoubleSplash()}?sessionId={MerchantSessionId}&cacheTimeout={Merchant?.TimeCacheRates}";

                var response = JsonConvert.DeserializeObject<AssertListWithSession>(
                    await (await HttpClient.GetAsync(rateServiceUrl)).Content
                        .ReadAsStringAsync());

                // var newSessionId = response.SessionId;
                rates = response.Asserts;


                if (!rates.Any(r => r.AssetPair.Equals(assertId, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return NotFound();
                }
            }
            catch(Exception e)
            {
                await Log.WriteErrorAsync(nameof(AssetPairRatesController), "Get real assert  pair rate", e);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            var rate = rates.First(r => r.AssetPair.Equals(assertId, StringComparison.CurrentCultureIgnoreCase));

            return Json(rate);
        }

        // POST api/assetPairRates/assertId
        [HttpPost("{assertId}")]
        public async Task<IActionResult> Post([FromBody]AprSafeRequest safeRequest, string assertId)
        {
            AprRequest request;
            if (safeRequest == null || !safeRequest.AprRequest(out request) || request.Percent < 0 || request.Pips < 0)
            {
                return BadRequest();
            }

            var isValid = await ValidateRequest();
            if ((isValid as OkResult)?.StatusCode != Ok().StatusCode)
            {
                return isValid;
            }


            var result = await GetRate(assertId);


#if TEST_ASSERT_ENABLE
            if (assertId.Equals("BTCTEST", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new AssertPairRateWithSession(new AssertPairRate
                {
                    Accuracy = 3,
                    Ask = 6500.555,
                    Bid = 6400.444,
                    AssetPair = "BTCTEST"
                }, "testSession");
            }
            else
            {
                var post = result as StatusCodeResult;
                if (post != null)
                {
                    return post;
                }
            }

#else
             var post = result as StatusCodeResult;
             if (post != null)
            {
                return post;
            }
#endif


            var rate = (AssertPairRateWithSession)result;

            rate.DBid = CalculateValue(rate.Bid, rate.Accuracy, request, false);
            rate.DAsk = CalculateValue(rate.Ask, rate.Accuracy, request, true);

            return new JsonResult(rate);
        }





    }
}