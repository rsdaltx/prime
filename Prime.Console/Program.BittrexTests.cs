﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Prime.Common;
using Prime.Plugins.Services.Bittrex;
using Prime.Plugins.Services.Poloniex;

namespace Prime.TestConsole
{
	public partial class Program
	{
		public class BittrexTests
		{
			public void ApiTest()
			{
				var provider = Networks.I.Providers.OfType<BittrexProvider>().FirstProvider();
				var apiTestCtx = new ApiPrivateTestContext(UserContext.Current.GetApiKey(provider));

				var ok = AsyncContext.Run(() => provider.TestPrivateApiAsync(apiTestCtx));


				try
				{

					System.Console.WriteLine($"Api test OK: {ok}");
				}
				catch (Exception e)
				{
					System.Console.WriteLine(e.Message);
					throw;
				}
			}

			public void GetDepositAddresses()
			{
				var provider = Networks.I.Providers.OfType<BittrexProvider>().FirstProvider();

				var ctx = new WalletAddressAssetContext("BTC".ToAsset(provider), UserContext.Current);
				var ctxAll = new WalletAddressContext(UserContext.Current);

				var addresses = AsyncContext.Run(() => provider.GetAddressesForAssetAsync(ctx));
				var addressesAll = AsyncContext.Run(() => provider.GetAddressesAsync(ctxAll));

				try
				{
					System.Console.WriteLine("Addresses for 1 asset");
					foreach (var address in addresses)
					{
						System.Console.WriteLine($"{address.Asset} : {address.Address}");
					}

					System.Console.WriteLine("Addresses for all assets");
					foreach (var address in addressesAll)
					{
						System.Console.WriteLine($"{address.Asset} : {address.Address}");
					}
				}
				catch (Exception e)
				{
					System.Console.WriteLine(e);
					throw;
				}
			}

			public void GetAssetPairs()
			{
				var provider = Networks.I.Providers.OfType<BittrexProvider>().FirstProvider();
				var ctx = new NetworkProviderContext();

				try
				{
					var pairs = AsyncContext.Run(() => provider.GetAssetPairsAsync(ctx));

					foreach (var pair in pairs)
					{
						System.Console.WriteLine($"{pair}");
					}
				}
				catch (Exception e)
				{
					System.Console.WriteLine(e.Message);
					throw;
				}
			}

			public void GetBalances()
			{
				var provider = Networks.I.Providers.OfType<BittrexProvider>().FirstProvider();
				var ctx = new NetworkProviderPrivateContext(UserContext.Current);

				var balances = AsyncContext.Run(() => provider.GetBalancesAsync(ctx));

				try
				{


					foreach (var balance in balances)
					{
						System.Console.WriteLine($"{balance.Asset} : {balance.AvailableAndReserved}, {balance.Available}, {balance.Reserved}");
					}
				}
				catch (Exception e)
				{
					System.Console.WriteLine(e.Message);
					throw;
				}
			}

			public void LatestPrices()
			{
				var provider = Networks.I.Providers.OfType<BittrexProvider>().FirstProvider();
				var pair = new AssetPair("BTC", "LTC");

				var ctx = new PublicPriceContext(pair);

				try
				{
					var price = AsyncContext.Run(() => provider.GetPriceAsync(ctx));

					System.Console.WriteLine($"Latest price for {pair} is {price.FirstPrice}");
				}
				catch (Exception e)
				{
					System.Console.WriteLine(e);
					throw;
				}
			}
		}
	}
}
