using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IndicadorChileAPI.App.Interfaces
{
    public interface IStatistics
    {
        public Task<ActionResult<int>> GetCountAsync(ushort Year, byte? Month);
        public Task<ActionResult<float>> GetMinimumAsync(ushort Year, byte? Month);
        public Task<ActionResult<float>> GetAverageAsync(ushort Year, byte? Month);
    }
}