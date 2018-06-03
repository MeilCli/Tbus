using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Tbus.App.NETStandard.Events;

namespace Tbus.App.NETStandard.Models
{
    internal interface IMainModel : IModel
    {
        event EventHandler<ErrorEventArgs> ErrorOccurred;

        bool IsLoading { get; }

        ReadOnlyObservableCollection<DayTableModel> DayTableModels { get; }

        Task LoadAsync();
    }
}
