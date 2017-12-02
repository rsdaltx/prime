﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Prime.Plugins.Services.NovaExchange;
using RestEase;

namespace Prime.Plugins.Services.Liqui
{
    internal interface ILiquiApi
    {
        [Get("/ticker/{currencyPair}")]
        Task<LiquiSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/ticker/{currencyPair}")]
        Task<Dictionary<string,LiquiSchema.TickerResponse>> GetTickersAsync([Path] string currencyPair);

        [Get("/info")]
        Task<LiquiSchema.AssetPairsResponse> GetAssetPairsAsync();
    }
}
