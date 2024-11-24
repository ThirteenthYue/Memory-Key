/*using Microsoft.Maui.Controls;
using System;
using Microsoft.Maui;
*/
namespace Memory_Key
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }


        // to study page
        private async void OnCounterClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//study");
        }

        // Make sure the page is opaque
        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.Opacity = 1;
            System.Diagnostics.Debug.WriteLine("MainPage is appearing");
        }

    }


}

