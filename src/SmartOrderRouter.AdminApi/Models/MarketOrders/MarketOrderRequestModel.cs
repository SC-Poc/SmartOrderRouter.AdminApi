using JetBrains.Annotations;

namespace SmartOrderRouter.AdminApi.Models.MarketOrders
{
    /// <summary>
    /// Represents a market order creation request.
    /// </summary>
    [PublicAPI]
    public class MarketOrderRequestModel
    {
        /// <summary>
        /// The name of asset pair.  
        /// </summary>
        public string AssetPair { get; set; }

        /// <summary>
        /// Indicates the order type.
        /// </summary>
        public OrderType Type { get; set; }

        /// <summary>
        /// The order volume.
        /// </summary>
        public decimal Volume { get; set; }
    }
}
