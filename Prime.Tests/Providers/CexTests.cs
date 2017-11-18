﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Cex;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class CexTests : ProviderDirectTestsBase
    {
        public CexTests()
        {
            Provider = Networks.I.Providers.OfType<CexProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var context = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
                "BTC_RUB".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(context);
        }

        [TestMethod]
        public override void TestGetPrice()
        {
            var context = new PublicPriceContext("BTC_USD".ToAssetPairRaw());
            base.TestGetPrice(context, false);
        }

        [TestMethod]
        public override void TestGetPrices()
        {
            var context = new PublicPricesContext(new List<AssetPair>()
            {
                "BTC_USD".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
                "BTC_RUB".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw()
            });
            base.TestGetPrices(context);
        }

        [TestMethod]
        public override void TestGetAssetPrices()
        {
            var context = new PublicAssetPricesContext(new List<Asset>()
            {
                "GHS".ToAssetRaw(),
                "ZEC".ToAssetRaw(),
                "DASH".ToAssetRaw(),
                "BCH".ToAssetRaw()
            }, Asset.Btc);

            base.TestGetAssetPrices(context);
        }

        [TestMethod]
        public override void TestGetVolume()
        {
            var context = new VolumeContext("BTC_USD".ToAssetPairRaw());

            base.TestGetVolume(context);
        }
    }
}
