using AutoMapper;
using SmartOrderRouter.AdminApi.Models.Balances;
using SmartOrderRouter.AdminApi.Models.ExternalLimitOrders;
using SmartOrderRouter.AdminApi.Models.MarketOrders;
using SmartOrderRouter.AdminApi.Models.OrderBooks;

namespace SmartOrderRouter.AdminApi
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Lykke.Service.SmartOrderRouter.Client.Models.Balances.BalanceModel, BalanceModel>(MemberList
                .Destination);

            CreateMap<Lykke.Service.SmartOrderRouter.Client.Models.ExternalLimitOrders.ExternalLimitOrderModel,
                ExternalLimitOrderModel>(MemberList.Destination);

            CreateMap<Lykke.Service.SmartOrderRouter.Client.Models.MarketOrders.MarketOrderModel,
                MarketOrderModel>(MemberList.Destination);

            CreateMap<Lykke.Service.SmartOrderRouter.Client.Models.OrderBooks.AggregatedOrderBookLevelModel,
                AggregatedOrderBookLevelModel>(MemberList.Destination);

            CreateMap<Lykke.Service.SmartOrderRouter.Client.Models.OrderBooks.AggregatedOrderBookModel,
                AggregatedOrderBookModel>(MemberList.Destination);

            CreateMap<Lykke.Service.SmartOrderRouter.Client.Models.OrderBooks.AggregatedOrderBookVolumeModel,
                AggregatedOrderBookVolumeModel>(MemberList.Destination);
        }
    }
}
