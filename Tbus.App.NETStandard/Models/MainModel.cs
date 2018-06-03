using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Tbus.App.NETStandard.Events;
using Tbus.App.NETStandard.Repositories;

namespace Tbus.App.NETStandard.Models
{
    internal class MainModel : BaseModel, IMainModel
    {
        public event EventHandler<ErrorEventArgs> ErrorOccurred;

        private readonly ITimeTableRepository timeTableRepository;

        private readonly ObservableCollection<DayTableModel> dayTableModels = new ObservableCollection<DayTableModel>();
        public ReadOnlyObservableCollection<DayTableModel> DayTableModels { get; }

        private bool _isLoading;
        public bool IsLoading {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public MainModel(ITimeTableRepository timeTableRepository)
        {
            this.timeTableRepository = timeTableRepository;
            DayTableModels = new ReadOnlyObservableCollection<DayTableModel>(dayTableModels);
        }

        public async Task LoadAsync()
        {
            try
            {
                IsLoading = true;
                var todayTimeTables = await timeTableRepository.GetTodayTimeTablesAsync();
                foreach (var todayTimeTable in todayTimeTables)
                {
                    dayTableModels.Add(new DayTableModel(todayTimeTable.id, todayTimeTable.dayTable));
                }
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(this, new ErrorEventArgs(e.Message));
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
