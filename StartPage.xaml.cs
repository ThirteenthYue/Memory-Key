namespace Memory_Key
{
    public partial class StartPage : ContentPage
    {
        public StartPage()
        {
            InitializeComponent();
        }

        // choose num
        private async void On5Clicked(object sender, EventArgs e)
        {
            NavigateToWordLearningPage(5);
        }

        private async void On10Clicked(object sender, EventArgs e)
        {
            NavigateToWordLearningPage(10);
        }

        private async void On15Clicked(object sender, EventArgs e)
        {
            NavigateToWordLearningPage(15);
        }

        private async void On20Clicked(object sender, EventArgs e)
        {
            NavigateToWordLearningPage(20);
        }

        private async void NavigateToWordLearningPage(int account)
        {
            var nextPage = new WordLearningPage(account); 
            await Navigation.PushAsync(nextPage);
        }

        // Gesture return
        private async void OnSwiped(object sender, SwipedEventArgs e)
        {
            if (e.Direction == SwipeDirection.Right)
            {
                await Shell.Current.GoToAsync("//main");
            }
        }
    }

}

