using System.Collections.Generic;
using JetBrains.Annotations;

namespace SmartOrderRouter.AdminApi.Models.OrderBooks
{
    /// <summary>
    /// Represents a price level in the aggregated order book.
    /// </summary>
    [PublicAPI]
    public class AggregatedOrderBookLevelModel : OrderBookLevelModel
    {
        /// <summary>
        /// A collection of volumes on exchange on price level.
        /// </summary>
        public IReadOnlyList<AggregatedOrderBookVolumeModel> ExchangeVolumes { get; set; }
    }
}
