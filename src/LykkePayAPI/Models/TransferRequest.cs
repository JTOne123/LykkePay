﻿

using System;
using System.Globalization;
using Lykke.Core;

namespace LykkePay.API.Models
{
    public class TransferRequest : IStoreRequest
    {
        public string SourceAddress { get; set; }
        public string DestinationAddress { get; set; }
        public string Amount { get; set; }
        public string AssetId { get; set; }
        public string SuccessUrl { get; set; }
        public string ErrorUrl { get; set; }
        public string ProgressUrl { get; set; }
        public string OrderId { get; set; }

        public Lykke.Pay.Service.StoreRequest.Client.Models.PayRequest GetRequest()
        {
            double amount = 0;
            if (double.TryParse(Amount, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out amount))
            {
                amount = Math.Round(amount, 8);

            }
            if (string.IsNullOrEmpty(DestinationAddress) || amount <= 0)
            {
                return null;
            }

            return new Lykke.Pay.Service.StoreRequest.Client.Models.PayRequest
            {
                
                MerchantPayRequestStatus = MerchantPayRequestStatus.New.ToString(),
                MerchantPayRequestType = MerchantPayRequestType.Transfer.ToString(),
                MerchantPayRequestNotification = MerchantPayRequestNotification.Nothing.ToString(),
                DestinationAddress = DestinationAddress,
                SourceAddress = SourceAddress,
                Amount = amount,
                AssetId = AssetId,
                SuccessUrl = SuccessUrl,
                ErrorUrl = ErrorUrl,
                ProgressUrl = ProgressUrl,
                OrderId = OrderId,
                RequestId = Guid.NewGuid().ToString()
            };
        }
    }
}
