﻿using System;
using Prime.Utility;

namespace Prime.Common
{
    public class PlaceOrderLimitContext : NetworkProviderPrivateContext
    {
        public PlaceOrderLimitContext(UserContext userContext, AssetPair pair, bool isBuy, decimal quantity, Money rate, ILogger logger = null) : base(userContext, logger)
        {
            Pair = pair;
            IsBuy = isBuy;
            Quantity = quantity;
            Rate = rate;

            if (!pair.Has(rate.Asset))
                throw new ArgumentException($"The {nameof(rate)}'s asset does not belong to this market '{pair}'");

            if(IsSell && !pair.Asset2.Equals(rate.Asset))
                throw new ArgumentException($"Wrong currency rate asset is set for '{pair}' market when SELLING - must be {pair.Asset2}");

            if(IsBuy && !pair.Asset1.Equals(rate.Asset))
                throw new ArgumentException($"Wrong currency rate asset is set for '{pair}' market when BUYING - must be {pair.Asset1}");
        }

        public AssetPair Pair { get; }
        public bool IsBuy { get; }
        public decimal Quantity { get; }
        public Money Rate { get; }

        public bool IsSell => !IsBuy;
    }
}