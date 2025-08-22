using IndicadorChileAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IndicadorChileAPI.App.Interfaces
{
    public interface IStatistics
    {
        public enum StatisticsEnum
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