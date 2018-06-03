using System.Collections.Generic;
using System.Threading.Tasks;
using Tbus.Parser.NETStandard;

namespace Tbus.App.NETStandard.Repositories
{
    internal interface ITimeTableRepository
    {
        Task<List<(string id, DayTable dayTable, HourTable hourTable)>> GetTodayTimeTablesAsync();
    }
}
