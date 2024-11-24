namespace Memory_Key
{
    public partial class ResultPage : ContentPage
    {

        private int totalAccount;
        private int Tnum;
        private int Fnum;
        private int finalScore = 0;

        // get info in WordLearningPage
        public ResultPage(int totalAccount, int Tnum, int Fnum) : this()
        {
            this.Tnum = Tnum;
            this.Fnum = Fnum;
            this.totalAccount = totalAccount;
            finalScore = totalAccount != 0 ? (int)((double)Tnum / totalAccount * 100) : 0;
            score.Text = finalScore.ToString();
            start.Text = $"Your finial score is {finalScore}, {totalAccount} questions in total, correct {Tnum}, incorrect{Fnum} ";
        }

        public ResultPage()
        {
            InitializeComponent();

        }


        private async void OnToMainClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//main");
        }

        private async void OnRestartClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//study");
        }





    }

}