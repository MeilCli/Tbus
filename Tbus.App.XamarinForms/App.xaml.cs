using Autofac;
using System;
using Tbus.App.NETStandard.ViewModels;
using Tbus.App.XamarinForms.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Tbus.App.XamarinForms
{
    public partial class App : Application
    {
        public static IContainer Container { get; private set; }

        public App()
        {
            InitializeComponent();
            initComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        private void initComponent()
        {
            var builder = new ContainerBuilder();
            builder.Register(c => new MainViewModel());
            Container = builder.Build();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
