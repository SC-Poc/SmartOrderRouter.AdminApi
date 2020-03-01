using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.SmartOrderRouter.Client;
using Microsoft.AspNetCore.Mvc;
using SmartOrderRouter.AdminApi.Models.ExternalLimitOrders;

namespace SmartOrderRouter.AdminApi.Controllers
{
    [Route("/api/[controller]")]
    public class ExternalLimitOrdersController : ControllerBase
    {
        private readonly ISmartOrderRouterClient _smartOrderRouterClient;
        private readonly IMapper _mapper;

        public ExternalLimitOrdersController(ISmartOrderRouterClient smartOrderRouterClient, IMapper mapper)
        {
            _smartOrderRouterClient = smartOrderRouterClient;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Returns a collection of external limit orders.
        /// </summary>
        /// <param name="marketOrderId">The identifier of the market order.</param>
        /// <response code="200">A collection of external limit orders.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ExternalLimitOrderModel>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync(string marketOrderId)
        {
            var externalLimitOrders =
                await _smartOrderRouterClient.ExternalLimitOrders.GetAsync(marketOrderId, null, null, null, null);

            var model = _mapper.Map<IReadOnlyList<ExternalLimitOrderModel>>(externalLimitOrders);

            return Ok(model);
        }
    }
}
