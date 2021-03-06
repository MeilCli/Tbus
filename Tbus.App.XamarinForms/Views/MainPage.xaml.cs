﻿using Autofac;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Tbus.App.NETStandard.ViewModels;
using Tbus.App.XamarinForms.Extensions;
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
            if (Device.RuntimePlatform == Device.UWP || Device.RuntimePlatform == Device.WPF)
            {
                ToolbarItems.Add(new ToolbarItem
                {
                    Text = "Load",
                    Command = viewModel.LoadCommand,
                    CommandParameter = string.Empty
                });
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.SubscribeModel();
            viewModel.ShowAlertCommand.Subscribe(x => DisplayAlert("Erroer", x, "OK"))
                .AddTo(disposables);
            viewModel.PushDayTableViewCommand.Subscribe(async x => await Navigation.PushPageAsync(new DayTablePage(x)))
                .AddTo(disposables);
            viewModel.LoadIfEmptyCommand.Execute();
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

        private void ListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (e.Item is DayTableViewModel dayTableViewModel)
            {
                viewModel.ItemAppearingCommand.Execute(dayTableViewModel);
            }
        }

        private void ListView_ItemDisappearing(object sender, ItemVisibilityEventArgs e)
        {
            if (e.Item is DayTableViewModel dayTableViewModel)
            {
                viewModel.ItemDisappearingCommand.Execute(dayTableViewModel);
            }
        }
    }
}
