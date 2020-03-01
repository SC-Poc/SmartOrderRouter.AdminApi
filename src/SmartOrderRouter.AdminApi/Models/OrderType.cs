using JetBrains.Annotations;

namespace SmartOrderRouter.AdminApi.Models
{
    /// <summary>
    /// Specifies an order type.
    /// </summary>
    [PublicAPI]
    public enum OrderType
    {
        /// <summary>
        /// Unspecified order type.
        /// </summary>
        None,

        /// <summary>
        /// Indicates buy order.
        /// </summary>
        Buy,

        /// <summary>
        /// Indicates sell order.
        /// </summary>
        Sell
    }
}
