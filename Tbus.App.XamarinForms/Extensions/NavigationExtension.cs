using System.Threading.Tasks;
using Xamarin.Forms;

namespace Tbus.App.XamarinForms.Extensions
{
    public static class NavigationExtension
    {
        public static async Task PushPageAsync(this INavigation navigation, Page page)
        {
            if (Device.RuntimePlatform == Device.WPF)
            {
                // WPFがPushAsyncに対応していない
                await navigation.PushModalAsync(page);
            }
            else
            {
                await navigation.PushAsync(page);
            }
        }
    }
}
