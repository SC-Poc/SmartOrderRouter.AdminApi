using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.SmartOrderRouter.Client;
using Microsoft.AspNetCore.Mvc;
using SmartOrderRouter.AdminApi.Models.OrderBooks;

namespace SmartOrderRouter.AdminApi.Controllers
{
    [Route("/api/[controller]")]
    public class OrderBooksController : ControllerBase
    {
        private readonly ISmartOrderRouterClient _smartOrderRouterClient;
        private readonly IMapper _mapper;

        public OrderBooksController(ISmartOrderRouterClient smartOrderRouterClient, IMapper mapper)
        {
            _smartOrderRouterClient = smartOrderRouterClient;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns an aggregated order book by asset pair. 
        /// </summary>
        /// <param name="assetPair">The name of the asset pair.</param>
        /// <returns>The aggregated order book.</returns>
        /// <response code="200">The aggregated order book.</response>
        [HttpGet("aggregated/{assetPair}")]
        [ProducesResponseType(typeof(AggregatedOrderBookModel), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetAggregatedAsync(string assetPair)
        {
            var orderBook = await _smartOrderRouterClient.OrderBooks.GetAggregatedAsync(assetPair);

            var model = _mapper.Map<AggregatedOrderBookModel>(orderBook);

            return Ok(model);
        }
    }
}
