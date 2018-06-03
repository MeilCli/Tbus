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

        private ReactivePropertySlim<string> id = new ReactivePropertySlim<string>(string.Empty);
        public IReadOnlyReactiveProperty<string> Id => id;

        internal DayTableViewModel(IDayTableModel model)
        {
            this.model = model;
        }

        public override void SubscribeModel()
        {
            model.ObserveProperty(x => x.Id)
                .Subscribe(x => id.Value = x)
                .AddTo(Disposables);
        }

        public override void UnSubscribeModel()
        {
            Disposables.Clear();
        }
    }
}
