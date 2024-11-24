using Newtonsoft.Json.Linq;
using Microsoft.CognitiveServices.Speech;


/*using System.Net.Http;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Maui;*/

namespace Memory_Key
{

    public partial class WordLearningPage : ContentPage
    {

        private const string WordsApiUrl = "https://wordsapiv1.p.rapidapi.com/words/hatchback/typeOf";
        private readonly HttpClient _httpClient = new HttpClient();
        private TextToSpeechService ttsService = new TextToSpeechService("44d3f99fe5bd45beb7b256174fb4ea39", "eastus");


        private int account;
        private int Tnum;
        private int Fnum;
        private int totalAccount;
        string word;


        public WordLearningPage()
        {
            InitializeComponent();

            word = WordLabel.Text;
            Shell.SetNavBarIsVisible(this, false);

            next.IsEnabled = false;
            speakWord.IsEnabled = false;

            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-key", "9d9634babfmshc931f2147dc7481p117420jsn631a5633fffe");
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-host", "wordsapiv1.p.rapidapi.com");

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await FetchRandomWordAsync();
            // When the word is loaded, enable the button
            next.IsEnabled = true;
            speakWord.IsEnabled = true;
        }

        public WordLearningPage(int account) : this()
        {
            this.account = account;
            totalAccount = account;
            ac.Text = $"{account} questions in total. Come on~";
        }

        // Next button
        private async void OnNewWordClicked(object sender, EventArgs e)
        {

            // Disable button to prevent repeated clicks when loading the next word
            (sender as Button).IsEnabled = false;

            if (wordEntry.Text == WordLabel.Text)
            {
                Tnum += 1;
            }
            else
            {
                Fnum += 1;
            }
            ChangeText();

            if (account > 1)
            {
                await FetchRandomWordAsync();
                account--;
                UpdateButtonText();
            }
            else
            {
                NavigateToResultPage(totalAccount, Tnum, Fnum);
            }

            wordEntry.Text = "";

            // Word load complete, re-enable button
            if (account > 0)
            {
                (sender as Button).IsEnabled = true;
            }

            word = WordLabel.Text;

        }



        // Change button text
        private void UpdateButtonText()
        {
            if (account == 1)
            {
                next.Text = "COMPLETE";
            }
        }

        // Change lable(ac) text
        private async Task ChangeText()
        {
            if (account == 1)
            {
                ac.Text = $"Now you correct {Tnum}, incorrect {Fnum}, it is last, come on!";
            }
            else
            {
                ac.Text = $"Now you correct {Tnum}, incorrect {Fnum}, {account - 1} left";
            }
        }

        // Get a new word
        private async Task FetchRandomWordAsync()
        {
            string apiKey = "9d9634babfmshc931f2147dc7481p117420jsn631a5633fffe";
            string requestUrl = "https://wordsapiv1.p.rapidapi.com/words/?random=true";


            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-key", apiKey);
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-host", "wordsapiv1.p.rapidapi.com");

            try
            {

                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();


                var json = JObject.Parse(responseBody);


                string randomWord = json["word"]?.ToString();
                string definition = json["results"]?[0]?["definition"]?.ToString();

                // If no definition is found, call the method again to get a new random word
                if (string.IsNullOrEmpty(definition))
                {
                    await FetchRandomWordAsync();
                }
                else
                {

                    Dispatcher.Dispatch(() =>
                    {
                        // Set to hidden
                        isWordHidden = true;

                        word = randomWord ?? "未找到";
                        WordLabel.Text = word;
                        DefinitionLabel.Text = definition ?? "未找到";

                        // Make sure that this method shows or hides WordLabel text based on the value of isWordHidden
                        ApplyWordVisibility();

                    });

                }
            }
            catch (HttpRequestException ex)
            {
                // if error
                await DisplayAlert("Error", $"Request exception: {ex.Message}", "OK");
            }
        }

        private bool isWordHidden = true;

        private void OnToggleButtonClicked(object sender, EventArgs e)
        {
            // change
            isWordHidden = !isWordHidden;

            // Apply the current word display status
            ApplyWordVisibility();
        }

        private void ApplyWordVisibility()
        {
            if (isWordHidden)
            {
                // If there are already set words, replace each letter with an asterisk to hide it, but keep the Spaces
                WordLabel.Text = word != null
                    ? string.Join("", word.Select(c => c == ' ' ? ' ' : '*'))
                    : "******";
            }
            else
            {
                WordLabel.Text = word;
            }
        }



        // 手势返回
        private async void OnSwiped(object sender, SwipedEventArgs e)
        {
            if (e.Direction == SwipeDirection.Right)
            {
                // 返回上一个页面
                await Shell.Current.GoToAsync("//study");
            }
        }

        private async void NavigateToResultPage(int Tnum, int Fnum, int totalAccount)
        {
            var nextPage = new ResultPage(Tnum, Fnum, totalAccount);
            await Navigation.PushAsync(nextPage);
        }




        // speak button
        private async void OnSpeakButtonClicked(object sender, EventArgs e)
        {
            string textToSpeak = word;

            if (string.IsNullOrWhiteSpace(textToSpeak))
            {
                await DisplayAlert("Error", "The text to speak cannot be empty.", "OK");
                return;
            }

            try
            {
                await ttsService.TextToSpeechAsync(textToSpeak);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while trying to speak: {ex.Message}", "OK");
            }
        }


        public class TextToSpeechService
        {
            private readonly string subscriptionKey;
            private readonly string serviceRegion;

            public TextToSpeechService(string key, string region)
            {
                subscriptionKey = key;
                serviceRegion = region;
            }

            public async Task TextToSpeechAsync(string word)
            {
                var config = SpeechConfig.FromSubscription(subscriptionKey, serviceRegion);

                using (var synthesizer = new SpeechSynthesizer(config))
                {
                    using (var result = await synthesizer.SpeakTextAsync(word))
                    {
                        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                        {
                            Console.WriteLine($"Speech synthesized to speaker for text [{word}]");
                        }
                        else if (result.Reason == ResultReason.Canceled)
                        {
                            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                            Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                            if (cancellation.Reason == CancellationReason.Error)
                            {
                                Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                                Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                            }
                        }
                    }
                }
            }
        }

    }
}

