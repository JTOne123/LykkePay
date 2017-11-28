using System;
using System.Runtime.Serialization;
using Lykke.Common.Entities.Pay;

namespace LykkePay.API.Models
{
    [DataContract]
    public class AssertPairRateWithSession : AssertPairRate
    {
        [DataMember(Name = "Lykke-Merchant-Session-Id")]
        public string SessionId { get; set; }

        [DataMember(Name = "assetPair")]
        public string DAssetPair => AssetPair;

        [DataMember(Name = "bid")]
        public float DBid => Bid;

        [DataMember(Name = "ask")]
        public float DAsk => Ask;

        [DataMember(Name = "accuracy")]
        public int DAccuracy => Accuracy;


        public AssertPairRateWithSession(AssertPairRate rate, string newSessionId) : base(rate)
        {
            SessionId = newSessionId;
        }
    }
}