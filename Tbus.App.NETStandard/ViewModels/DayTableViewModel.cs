using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Linq;
using Tbus.App.NETStandard.Models;

namespace Tbus.App.NETStandard.ViewModels
{
    public class DayTableViewModel : BaseViewModel
    {
        private readonly IDayTableModel model;

        private readonly ReactivePropertySlim<string> id = new ReactivePropertySlim<string>(string.Empty);
        public IReadOnlyReactiveProperty<string> Id => id;

        private readonly ReactivePropertySlim<string> time = new ReactivePropertySlim<string>(string.Empty);
        public IReadOnlyReactiveProperty<string> Time => time;

        private readonly ReactivePropertySlim<string> destination = new ReactivePropertySlim<string>(string.Empty);
        public IReadOnlyReactiveProperty<string> Destination => destination;

        private readonly ReactivePropertySlim<string> remainingTime = new ReactivePropertySlim<string>(string.Empty);
        public IReadOnlyReactiveProperty<string> RemainingTime => remainingTime;

        public ReactiveCommand StartCounterCommand { get; } = new ReactiveCommand();
        public ReactiveCommand StopCounterCommand { get; } = new ReactiveCommand();

        /// <summary>
        /// デザイナー用コンストラクター
        /// </summary>
        [Obsolete("Designer only", true)]
        public DayTableViewModel() { }

        internal DayTableViewModel(IDayTableModel model)
        {
            this.model = model;
        }

        internal DayTableViewModel Clone()
        {
            return new DayTableViewModel(model);
        }

        public override void SubscribeModel()
        {
            model.ObserveProperty(x => x.Id)
                .Subscribe(x => id.Value = x)
                .AddTo(Disposables);
            model.ObserveProperty(x => x.Time)
                .Subscribe(x => time.Value = x)
                .AddTo(Disposables);
            model.ObserveProperty(x => x.Destination)
                .Subscribe(x => destination.Value = x)
                .AddTo(Disposables);
            model.ObserveProperty(x => x.RemainingTime)
                .Subscribe(x => remainingTime.Value = x)
                .AddTo(Disposables);
            StartCounterCommand.Subscribe(_ => model.StartCounter())
                .AddTo(Disposables);
            StopCounterCommand.Subscribe(_ => model.StopCounter())
                .AddTo(Disposables);
        }

        public override void UnSubscribeModel()
        {
            Disposables.Clear();
        }
    }
}
