using Microsoft.Maui.Controls;
using System;

namespace Mökinvaraus.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void AvaaMokkiPage_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MokkiPage());
        }

        private async void AvaaAsiakasPage_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AsiakasPage());
        }
       
    }
}
