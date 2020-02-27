using AutoMapper;
using SmartOrderRouter.AdminApi.Models.Balances;

namespace SmartOrderRouter.AdminApi
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Lykke.Service.SmartOrderRouter.Client.Models.Balances.BalanceModel, BalanceModel>(MemberList
                .Destination);
        }
    }
}