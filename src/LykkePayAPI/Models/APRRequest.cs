﻿using System.Globalization;
using Lykke.Contracts.Pay;

namespace LykkePay.API.Models
{
    public class AprRequest
    {
        public AprRequest()
        {
            Markup = new AssertPairRateRequest();
        }

        public double Percent => Markup.Percent;

        public int Pips => Markup.Pips;

        public AssertPairRateRequest Markup { get; set; }
    }


    public class AssertPairRateSafeRequest
    {
        public string Percent { get; set; }

        public string Pips { get; set; }
    }

    public class AprSafeRequest
    {
        public string Percent => Markup.Percent;

        public string Pips => Markup.Pips;

        public AssertPairRateSafeRequest Markup { get; set; }

        public bool AprRequest(out AprRequest request)
        {
            float pr =  0;
            int p = 0;
            request = null;
            var result = (Percent == null || float.TryParse(Percent, NumberStyles.Float | NumberStyles.AllowThousands,
                              CultureInfo.InvariantCulture, out pr)) &&
                   (Pips == null || int.TryParse(Pips, NumberStyles.Float | NumberStyles.AllowThousands,
                        CultureInfo.InvariantCulture, out p));
            if (result)
            {
                request = new AprRequest
                {
                    Markup =
                    {
                        Pips = p,
                        Percent = pr
                    }
                };
            };

            return result;
        }
    }

}
