using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Linq;
using Tbus.App.NETStandard.Events;
using Tbus.App.NETStandard.Models;
using Tbus.App.NETStandard.Repositories;

namespace Tbus.App.NETStandard.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IMainModel model;

        private readonly ReactivePropertySlim<ReadOnlyReactiveCollection<DayTableViewModel>> dayTableViewModels
            = new ReactivePropertySlim<ReadOnlyReactiveCollection<DayTableViewModel>>();
        public IReadOnlyReactiveProperty<ReadOnlyReactiveCollection<DayTableViewModel>> DayTableViewModels => dayTableViewModels;

        private readonly ReactivePropertySlim<bool> isLoading = new ReactivePropertySlim<bool>();
        public IReadOnlyReactiveProperty<bool> IsLoading => isLoading;

        public ReactiveCommand<DayTableViewModel> PushDayTableViewCommand { get; } = new ReactiveCommand<DayTableViewModel>();
        public ReactiveCommand<string> ShowAlertCommand { get; } = new ReactiveCommand<string>();
        public AsyncReactiveCommand LoadCommand { get; } = new AsyncReactiveCommand();

        public ReactiveCommand<DayTableViewModel> ItemSelectedCommand { get; } = new ReactiveCommand<DayTableViewModel>();
        public ReactiveCommand<DayTableViewModel> ItemAppearingCommand { get; } = new ReactiveCommand<DayTableViewModel>();
        public ReactiveCommand<DayTableViewModel> ItemDisappearingCommand { get; } = new ReactiveCommand<DayTableViewModel>();

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
            dayTableViewModels.Value = model.DayTableModels.ToReadOnlyReactiveCollection(x => new DayTableViewModel(x))
                .AddTo(Disposables);
            model.ObserveProperty(x => x.IsLoading)
                .Subscribe(x => isLoading.Value = x)
                .AddTo(Disposables);
            Observable.FromEventPattern<ErrorEventArgs>(h => model.ErrorOccurred += h, h => model.ErrorOccurred -= h)
                .Select(x => x.EventArgs)
                .Subscribe(x => ShowAlertCommand.Execute(x.Message))
                .AddTo(Disposables);
            LoadCommand.Subscribe(async x => await model.LoadAsync())
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
}
