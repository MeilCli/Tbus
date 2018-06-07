using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tbus.Parser.NETStandard;

namespace Tbus.App.NETStandard.Models
{
    internal class DayTableModel : BaseModel, IDayTableModel
    {
        private readonly DayTable dayTable;

        private bool isRequestRunning = false;

        public string Id { get; }

        public string Station { get; }

        public string Direction { get; }

        private string _time;
        public string Time {
            get => _time;
            set => SetProperty(ref _time, value);
        }

        private string _destination;
        public string Destination {
            get => _destination;
            set => SetProperty(ref _destination, value);
        }

        private string _remainingTime;
        public string RemainingTime {
            get => _remainingTime;
            set => SetProperty(ref _remainingTime, value);
        }

        public DayTableModel(string id, DayTable dayTable)
        {
            Id = id;
            string[] ar = id.Split(' ');
            Station = 1 <= ar.Length ? ar[0] ?? string.Empty : string.Empty;
            Direction = 2 <= ar.Length ? ar[1] ?? string.Empty : string.Empty;
            this.dayTable = dayTable;
        }

        public async void StartCounter()
        {
            int day = DateTime.Now.Day;
            isRequestRunning = true;
            while (isRequestRunning)
            {
                int time = DateTime.Now.Hour * 100 + DateTime.Now.Minute;
                Bus bus = dayTable.Buses.Where(_ => day == DateTime.Now.Day)
                    .Where(x => time < x.Hour * 100 + x.Minute)
                    .FirstOrDefault();
                if (bus != null)
                {
                    Time = $"{bus.Hour}時{bus.Minute}分発";
                    Destination = bus.Destination;
                    var busTime = new DateTime(
                        DateTime.Now.Year,
                        DateTime.Now.Month,
                        DateTime.Now.Day,
                        bus.Hour,
                        bus.Minute,
                        0,
                        DateTimeKind.Local);
                    TimeSpan timeOffset = busTime - DateTime.Now;

                    var sb = new StringBuilder();
                    sb.Append("あと約");
                    if (0 < timeOffset.Hours)
                    {
                        sb.Append($"{timeOffset.Hours}時間");
                        sb.Append($"{timeOffset.Minutes}分");
                        sb.Append($"{timeOffset.Seconds}秒");
                    }
                    else if (0 < timeOffset.Minutes)
                    {
                        sb.Append($"{timeOffset.Minutes}分");
                        sb.Append($"{timeOffset.Seconds}秒");
                    }
                    else
                    {
                        sb.Append($"{timeOffset.Seconds}秒");
                    }
                    RemainingTime = sb.ToString();
                }
                else
                {
                    Time = "もうバスはありません";
                    Destination = string.Empty;
                    RemainingTime = string.Empty;
                    isRequestRunning = false;
                }
                await Task.Delay(500);
            }
        }

        public void StopCounter()
        {
            isRequestRunning = false;
        }
    }
}
