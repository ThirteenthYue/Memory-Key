namespace Memory_Key
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            var navigationPage = new NavigationPage(new MainPage());

        }
    }
}
