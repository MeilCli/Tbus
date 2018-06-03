using Autofac;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Tbus.App.NETStandard.ViewModels;
using Xamarin.Forms;

namespace Tbus.App.XamarinForms.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel viewModel;
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public MainPage()
        {
            InitializeComponent();
            viewModel = App.Container.Resolve<MainViewModel>();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.SubscribeModel();
            viewModel.LoadCommand.Execute();
            viewModel.ShowAlertCommand.Subscribe(x => DisplayAlert("Erroer", x, "OK"))
                .AddTo(disposables);
            viewModel.PushDayTableViewCommand.Subscribe(x => Navigation.PushAsync(new DayTablePage(x)))
                .AddTo(disposables);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            viewModel.UnSubscribeModel();
            disposables.Clear();
        }

        ~MainPage()
        {
            // おまじない
            viewModel.Dispose();
            disposables.Dispose();
        }
    }
}
