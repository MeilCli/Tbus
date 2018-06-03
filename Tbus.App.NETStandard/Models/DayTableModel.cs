using Tbus.Parser.NETStandard;

namespace Tbus.App.NETStandard.Models
{
    internal class DayTableModel : BaseModel, IDayTableModel
    {
        private readonly DayTable dayTable;

        public string Id { get; }

        public DayTableModel(string id, DayTable dayTable)
        {
            Id = id;
            this.dayTable = dayTable;
        }
    }
}
