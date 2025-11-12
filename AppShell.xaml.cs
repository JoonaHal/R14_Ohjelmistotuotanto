using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Mökinvaraus
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        private void SfSegmentedControl_SelectionChanged(object sender, EventArgs e)
        {
            
        }

        public static async Task DisplayToastAsync(string message, ToastDuration duration = ToastDuration.Short)
        {
            var toast = Toast.Make(message, duration);
            await toast.Show();
        }
    }
}
