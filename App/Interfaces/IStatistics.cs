using IndicadorChileAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IndicadorChileAPI.App.Interfaces
{
    public interface IStatistics
    {
        public Task<ActionResult<int>> GetCountAsync([FromQuery] SearchFilterModel SearchFilter);
        public Task<ActionResult<float>> GetMinimumAsync([FromQuery] SearchFilterModel SearchFilter);
        public Task<ActionResult<float>> GetMaximumAsync([FromQuery] SearchFilterModel SearchFilter);
        public Task<ActionResult<float>> GetSumAsync([FromQuery] SearchFilterModel SearchFilter);
        public Task<ActionResult<float>> GetSumOfSquares([FromQuery] SearchFilterModel SearchFilter);
        public Task<ActionResult<float>> GetAverageAsync([FromQuery] SearchFilterModel SearchFilter);
    }
}