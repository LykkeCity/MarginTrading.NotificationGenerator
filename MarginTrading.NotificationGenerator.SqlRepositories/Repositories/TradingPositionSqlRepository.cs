using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Dapper;
using MarginTrading.NotificationGenerator.Core.Domain;
using MarginTrading.NotificationGenerator.Core.Services;
using MarginTrading.NotificationGenerator.SqlRepositories.Entities;

namespace MarginTrading.NotificationGenerator.SqlRepositories.Repositories
{
    public class TradingPositionSqlRepository : ITradingPositionSqlRepository
    {
        private readonly ILog _log;
        private readonly IConvertService _convertService;
        private readonly string _connectionString;
        
        private const string ClosedTableName = "TradingPositionClosed";
        private static readonly string GetColumns =
            string.Join(",", typeof(TradingPositionEntity).GetProperties().Select(x => x.Name));

        public TradingPositionSqlRepository(string connectionString, ILog log, IConvertService convertService)
        {
            _connectionString = connectionString;
            _log = log;
            _convertService = convertService;
        }
        
        public async Task<IEnumerable<TradingPosition>> GetClosedFromPeriod(DateTime @from, DateTime to)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    await conn.OpenAsync();

                var query = $"SELECT {GetColumns} FROM {ClosedTableName} WHERE [CloseDate] < @to AND [CloseDate] > @from";
                var data = await conn.QueryAsync<TradingPositionEntity>(query, new { from, to });
                
                return data.Select(x => _convertService.Convert<TradingPositionEntity, TradingPosition>(x));
            }
        }
    }
}