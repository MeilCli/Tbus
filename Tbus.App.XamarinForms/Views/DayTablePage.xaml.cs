using Tbus.App.NETStandard.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tbus.App.XamarinForms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DayTablePage : ContentPage
    {
        private readonly DayTableViewModel viewModel;

        public DayTablePage(DayTableViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.SubscribeModel();
            viewModel.StartCounterCommand.Execute();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            viewModel.StopCounterCommand.Execute();
            viewModel.UnSubscribeModel();
        }

        ~DayTablePage()
        {
            // おまじない
            viewModel.Dispose();
        }
    }
}