namespace SmartOrderRouter.AdminApi.Models.Balances
{
    /// <summary>
    /// Represents an asset balance on exchange.
    /// </summary>
    public class BalanceModel
    {
        /// <summary>
        /// The name of the exchange.
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// The name of the asset.
        /// </summary>
        public string Asset { get; set; }

        /// <summary>
        /// The current amount of balance.
        /// </summary>
        public decimal Amount { get; set; }
    }
}