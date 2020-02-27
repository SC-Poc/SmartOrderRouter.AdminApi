using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.SmartOrderRouter.Client;
using Microsoft.AspNetCore.Mvc;
using SmartOrderRouter.AdminApi.Models.Balances;

namespace SmartOrderRouter.AdminApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class BalancesController : ControllerBase
    {
        private readonly ISmartOrderRouterClient _smartOrderRouterClient;
        private readonly IMapper _mapper;

        public BalancesController(ISmartOrderRouterClient smartOrderRouterClient, IMapper mapper)
        {
            _smartOrderRouterClient = smartOrderRouterClient;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var balances = await _smartOrderRouterClient.Balances.GetAsync();

            var model = _mapper.Map<List<BalanceModel>>(balances);

            return Ok(model);
        }
    }
}