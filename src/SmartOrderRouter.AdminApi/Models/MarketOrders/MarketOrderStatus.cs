using JetBrains.Annotations;

namespace SmartOrderRouter.AdminApi.Models.MarketOrders
{
    /// <summary>
    /// Specifies a market order type.
    /// </summary>
    [PublicAPI]
    public enum MarketOrderStatus
    {
        /// <summary>
        /// Unspecified market order type.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that the market order created and not processed yet.
        /// </summary>
        New,

        /// <summary>
        /// Indicates that the market order is in progress.
        /// </summary>
        Active,
        
        /// <summary>
        /// Indicates that the market order executed.
        /// </summary>
        Filled,

        /// <summary>
        /// Indicates that the market order can not be executed for some reasons.
        /// </summary>
        Cancelled
    }
}
