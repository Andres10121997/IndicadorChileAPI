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
        


        public Task<ActionResult<int>> GetCountAsync([FromQuery] SearchFilterModel SearchFilter);
        public Task<ActionResult<float>> GetMinimumAsync([FromQuery] SearchFilterModel SearchFilter);
        public Task<ActionResult<float>> GetMaximumAsync([FromQuery] SearchFilterModel SearchFilter);
        public Task<ActionResult<float>> GetSumAsync([FromQuery] SearchFilterModel SearchFilter);
        public Task<ActionResult<float>> GetSumOfSquaresAsync([FromQuery] SearchFilterModel SearchFilter);
        public Task<ActionResult<float>> GetAverageAsync([FromQuery] SearchFilterModel SearchFilter);
    }
}