using System;
using AutoMapper;
using JetBrains.Annotations;
using MarginTrading.Backend.Contracts.Account;
using MarginTrading.Backend.Contracts.AccountHistory;
using MarginTrading.Backend.Contracts.TradeMonitoring;
using MarginTrading.NotificationGenerator.Core.Domain;
using MarginTrading.NotificationGenerator.Core.Services;

namespace MarginTrading.NotificationGenerator.Services
{
    [UsedImplicitly]
    public class ConvertService : IConvertService
    {
        private readonly IMapper _mapper = CreateMapper();

        private static IMapper CreateMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                // todo: add some global configurations here
                cfg.CreateMap<OrderHistoryContract, OrderHistory>().ReverseMap();
                cfg.CreateMap<OrderContract, OrderHistory>().ReverseMap();
                cfg.CreateMap<DataReaderAccountBackendContract, Account>().ReverseMap();
                cfg.CreateMap<AccountHistoryContract, AccountHistory>().ReverseMap();

            }).CreateMapper();
        }

        public TResult Convert<TSource, TResult>(TSource source,
            Action<IMappingOperationOptions<TSource, TResult>> opts)
        {
            return _mapper.Map(source, opts);
        }

        public TResult Convert<TSource, TResult>(TSource source)
        {
            return _mapper.Map<TSource, TResult>(source);
        }
    }
}