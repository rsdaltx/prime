﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Abucoins
{
    internal interface IAbucoinsApi
    {
        [Get("/products/{currencyPair}/ticker")]
        Task<AbucoinsSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/products")]
        Task<AbucoinsSchema.ProductResponse[]> GetAssetsAsync();

        [Get("/products/stats")]
        Task<AbucoinsSchema.VolumeResponse[]> GetVolumesAsync();

        [Get("/products/{currencyPair}/book?level={depth}")]
        Task<AbucoinsSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair, [Path] int depth);
    }
}
