using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Tbus.App.NETStandard.Events;
using Tbus.App.NETStandard.Models;
using Tbus.App.NETStandard.Repositories;

namespace Tbus.App.NETStandard.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IMainModel model;

        public ReactiveCollection<MainGroupListViewModel> DayTableViewModels { get; }
            = new ReactiveCollection<MainGroupListViewModel>();

        private readonly ReactivePropertySlim<bool> isLoading = new ReactivePropertySlim<bool>();
        public IReadOnlyReactiveProperty<bool> IsLoading => isLoading;

        public AsyncReactiveCommand<DayTableViewModel> PushDayTableViewCommand { get; } = new AsyncReactiveCommand<DayTableViewModel>();
        public ReactiveCommand<string> ShowAlertCommand { get; } = new ReactiveCommand<string>();
        public AsyncReactiveCommand LoadCommand { get; } = new AsyncReactiveCommand();
        public AsyncReactiveCommand LoadIfEmptyCommand { get; } = new AsyncReactiveCommand();

        public ReactiveCommand<DayTableViewModel> ItemSelectedCommand { get; } = new ReactiveCommand<DayTableViewModel>();
        public ReactiveCommand<IViewModel> ItemAppearingCommand { get; } = new ReactiveCommand<IViewModel>();
        public ReactiveCommand<IViewModel> ItemDisappearingCommand { get; } = new ReactiveCommand<IViewModel>();

        public MainViewModel()
        {
            model = new MainModel(new TimeTableRepository());
        }

        internal MainViewModel(IMainModel model)
        {
            this.model = model;
        }

        public override void SubscribeModel()
        {
            model.DayTableModels.ObserveAddChanged()
                .Subscribe(x =>
                {
                    MainGroupListViewModel sameTitleViewModel = DayTableViewModels.FirstOrDefault(y => y.Title == x.Station);
                    if (sameTitleViewModel != null)
                    {
                        sameTitleViewModel.Add(new DayTableViewModel(x));
                    }
                    else
                    {
                        DayTableViewModels.Add(new MainGroupListViewModel(x.Station) { new DayTableViewModel(x) });
                    }
                })
                .AddTo(Disposables);
            model.DayTableModels.ObserveResetChanged()
                .Subscribe(x => DayTableViewModels.Clear())
                .AddTo(Disposables);
            model.ObserveProperty(x => x.IsLoading)
                .Subscribe(x => isLoading.Value = x)
                .AddTo(Disposables);
            Observable.FromEventPattern<ErrorEventArgs>(h => model.ErrorOccurred += h, h => model.ErrorOccurred -= h)
                .Select(x => x.EventArgs)
                .Subscribe(x => ShowAlertCommand.Execute(x.Message))
                .AddTo(Disposables);
            LoadCommand.Subscribe(async _ => await model.LoadAsync())
                .AddTo(Disposables);
            LoadIfEmptyCommand.Subscribe(async _ => await model.LoadIfEmptyAsync())
                .AddTo(Disposables);
            ItemSelectedCommand.Subscribe(x => PushDayTableViewCommand.Execute(x.Clone()))
                .AddTo(Disposables);
            ItemAppearingCommand.Subscribe(x => x.SubscribeModel())
                .AddTo(Disposables);
            ItemDisappearingCommand.Subscribe(x => x.UnSubscribeModel())
                .AddTo(Disposables);
        }

        public override void UnSubscribeModel()
        {
            Disposables.Clear();
        }
    }

    public class MainGroupListViewModel : ObservableCollection<DayTableViewModel>
    {
        public string Title { get; }

        public MainGroupListViewModel(string title)
        {
            Title = title;
        }
    }
}
