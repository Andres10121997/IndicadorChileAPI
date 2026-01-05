using API.Models.Get;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.App.Interfaces
{
    public interface IStatistics
    {
        public enum StatisticsEnum : byte
        {
            Count,
            Minimum,
            Maximum,
            Sum,
            SumOfSquares,
            Average
        }


        public Task<ActionResult<float>> GetStatisticsAsync(StatisticsEnum Statistics, [FromQuery] SearchFilterModel SearchFilter);
    }
}