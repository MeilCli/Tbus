using System;

namespace Tbus.App.NETStandard.ViewModels
{
    public interface IViewModel : IDisposable
    {
        void SubscribeModel();

        void UnSubscribeModel();
    }
}
