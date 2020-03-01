using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.SmartOrderRouter.Client;
using Microsoft.AspNetCore.Mvc;
using SmartOrderRouter.AdminApi.Models;
using SmartOrderRouter.AdminApi.Models.MarketOrders;

namespace SmartOrderRouter.AdminApi.Controllers
{
    [Route("/api/[controller]")]
    public class MarketOrdersController : ControllerBase
    {
        private const string ClientId = "admin-api";

        private readonly ISmartOrderRouterClient _smartOrderRouterClient;
        private readonly IMapper _mapper;

        public MarketOrdersController(ISmartOrderRouterClient smartOrderRouterClient, IMapper mapper)
        {
            _smartOrderRouterClient = smartOrderRouterClient;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns a collection of last 100 market orders.
        /// </summary>
        /// <returns>A collection of market orders.</returns>
        /// <response code="200">A collection of market orders.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<MarketOrderModel>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync()
        {
            var marketOrders = await _smartOrderRouterClient.MarketOrders
                .GetAsync(DateTime.UtcNow.AddYears(-1), DateTime.UtcNow, 100);

            var model = _mapper.Map<IReadOnlyList<MarketOrderModel>>(marketOrders);

            return Ok(model);
        }

        /// <summary>
        /// Returns the market order by identifier.
        /// </summary>
        /// <param name="marketOrderId">The identifier of the market order.</param>
        /// <returns>The market order.</returns>
        /// <response code="200">A market order.</response>
        [HttpGet("{marketOrderId}")]
        [ProducesResponseType(typeof(MarketOrderModel), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetByIdAsync(string marketOrderId)
        {
            var marketOrder = await _smartOrderRouterClient.MarketOrders.GetByIdAsync(marketOrderId);

            var model = _mapper.Map<MarketOrderModel>(marketOrder);

            return Ok(model);
        }

        /// <summary>
        /// Creates new market order.
        /// </summary>
        /// <param name="model">The model that describes market order creation information.</param>
        /// <response code="200">The market order successfully created.</response>
        /// <response code="400">An error occurred while creating market order.</response>
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task CreateAsync([FromBody] MarketOrderRequestModel model)
        {
            var request = new Lykke.Service.SmartOrderRouter.Client.Models.MarketOrders.MarketOrderRequestModel
            {
                AssetPair = model.AssetPair,
                ClientId = ClientId,
                Volume = model.Volume,
                Type = model.Type == OrderType.Sell
                    ? Lykke.Service.SmartOrderRouter.Client.Models.OrderType.Sell
                    : Lykke.Service.SmartOrderRouter.Client.Models.OrderType.Buy
            };

            await _smartOrderRouterClient.MarketOrders.CreateAsync(request);
        }
    }
}
