﻿#region

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Prime.Core;
using Jojatekok.PoloniexAPI;
using Jojatekok.PoloniexAPI.MarketTools;
using LiteDB;
using Nito.AsyncEx;
using RestEase;
using Prime.Utility;

#endregion

namespace plugins
{
    public class CoinbaseProvider : IExchangeProvider, IWalletService, IOhlcProvider
    {
        private static readonly ObjectId IdHash = "prime:coinbase".GetObjectIdHashCode();

        private static readonly string _pairs = "btcusd,btceur,eurusd,ethusd,etheur,ethbtc,ltcusd,ltceur,ltcbtc";

        public AssetPairs Pairs => new AssetPairs(3, _pairs, this);
        public Network Network { get; } = new Network("Coinbase");

        public bool Disabled => false;

        public int Priority => 100;

        public string AggregatorName => null;

        public string Title => Network.Name;

        public ObjectId Id => IdHash;
        
        public T GetApi<T>(NetworkProviderContext context) where T : class
        {
            return RestClient.For<ICoinbaseApi>("https://api.coinbase.com/v2/") as T;
        }

        public T GetApi<T>(NetworkProviderPrivateContext context) where T : class
        {
            var key = context.GetKey(this);
            return RestClient.For<ICoinbaseApi>("https://api.coinbase.com/v2/", new CoinbaseAuthenticator(key).GetRequestModifier) as T;
        }

        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public Task<Money> GetLastPrice(PublicPriceContext context)
        {
            var t = new Task<Money>(() => Money.Zero);
            t.RunSynchronously();
            return t;
        }

        public BuyResult Buy(BuyContext ctx)
        {
            return null;
        }

        public SellResult Sell(SellContext ctx)
        {
            return null;
        }

        public Task<AssetPairs> GetAssetPairs(NetworkProviderContext context)
        {
            var t = new Task<AssetPairs>(() => Pairs);
            t.RunSynchronously();
            return t;
        }

        public OhclData GetOhlc(OhlcContext context)
        {
            var api = GetApi<PoloniexClient>(null);
            var cpair = new CurrencyPair(context.Pair.Asset1.ToRemoteCode(this), context.Pair.Asset2.ToRemoteCode(this));
            var mp = MarketPeriod.Hours2;
            var ds = DateTime.UtcNow.AddDays(-10);
            var de = DateTime.UtcNow;
            var apir = AsyncContext.Run(() => api.Markets.GetChartDataAsync(cpair, mp, ds, de));
            var r = new OhclData(context.Market);
            var seriesid = OhlcResolutionDataAdapter.GetHash(context.Pair, context.Market, Network);
            foreach (var i in apir)
            {
                r.Add(new OhclEntry(seriesid, i.Time, this)
                {
                    Open = i.Open,
                    Close = i.Close,
                    Low = i.Low,
                    High = i.High,
                    VolumeTo = (long) i.VolumeQuote,
                    VolumeFrom = (long) i.VolumeBase,
                    WeightedAverage = i.WeightedAverage
                });
            }
            return r;
        }

        public Task<string> TestApi(ApiTestContext context)
        {
            var t = new Task<string>(delegate
            {
                try
                {
                    var api = GetApi<ICoinbaseApi>(context);
                    var r = AsyncContext.Run(api.GetAccountsAsync);
                    return r != null ? null : "BAD";
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            });
            return t;
        }

        public bool CanMultiDepositAddress { get; } = true;

        public bool CanGenerateDepositAddress { get; } = true;


        public WalletAddresses FetchAllDepositAddresses(WalletAddressContext context)
        {
            throw new NotImplementedException();
        }

        public BalanceResults GetBalance(NetworkProviderPrivateContext context)
        {
            return context.L.Trace("Getting balance from " + Title, () =>
            {
                var api = GetApi<ICoinbaseApi>(context);
                var r = AsyncContext.Run(api.GetAccountsAsync);

                var results = new BalanceResults(this);

                foreach (var a in r.data)
                {
                    if (a.balance == null)
                        continue;

                    var c = a.balance.currency.ToAsset(this);
                    results.AddBalance(c, a.balance.amount);
                    results.AddAvailable(c, a.balance.amount);
                    results.AddReserved(c, a.balance.amount);
                }

                return results;
            });
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public WalletAddresses FetchDepositAddresses(WalletAddressAssetContext context)
        {
            var api = GetApi<ICoinbaseApi>(context);

            var accid = "";

            var r = AsyncContext.Run(async delegate
            {
                var accs = await api.GetAccounts();
                var ast = context.Asset.ToRemoteCode(this);

                var acc = accs.data.FirstOrDefault(x=> string.Equals(x.currency, ast, StringComparison.OrdinalIgnoreCase));
                if (acc == null)
                    return null;

                accid = acc.id;

                return await api.GetAddressesAsync(acc.id);
            });

            if (accid == null)
                return null;

            if (r.data.Count == 0 && context.CanGenerateAddress)
            {
                var cr = AsyncContext.Run(() => api.CreateAddressAsync(accid));
                if (cr != null)
                    r.data.AddRange(cr.data);
            }

            var addresses = new WalletAddresses();

            foreach (var a in r.data)
            {
                if (string.IsNullOrWhiteSpace(a.address))
                    continue;
                
                var forasset = FromNetwork(a.network);
                if (!context.Asset.Equals(forasset))
                    continue;

                addresses.Add(new WalletAddress(this, context.Asset) {Address = a.address});
            }

            return addresses;
        }

        private Asset FromNetwork(string network)
        {
            switch (network)
            {
                case "bitcoin":
                    return "BTC".ToAssetRaw();
                case "litecoin":
                    return "LTC".ToAssetRaw();
                case "ethereum":
                    return "ETH".ToAssetRaw();
                default:
                    return Asset.None;
            }
        }
    }
}