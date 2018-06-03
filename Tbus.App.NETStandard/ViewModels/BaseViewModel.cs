using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;

namespace Tbus.App.NETStandard.ViewModels
{
    public abstract class BaseViewModel : IViewModel
    {
        private protected CompositeDisposable Disposables = new CompositeDisposable();

        public abstract void SubscribeModel();
        public abstract void UnSubscribeModel();

        public void Dispose()
        {
            Disposables.Dispose();
        }
    }
}
