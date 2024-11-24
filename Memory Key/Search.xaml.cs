using Newtonsoft.Json.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using Microsoft.CognitiveServices.Speech;



namespace Memory_Key
{
    public partial class Search : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private TextToSpeechService ttsService = new TextToSpeechService("44d3f99fe5bd45beb7b256174fb4ea39", "eastus");


        public Search()
        {
            InitializeComponent();
        }

        string wordMeaning;
        string word;

        
        private async void OnNewWordClicked(object sender, EventArgs e)
        {
            // clean all text
            translatedTextLabel.Text = ""; 
            wordLabel.Text = " "; 
            wordMeaningLabel.Text = " "; 

            string apiKey = "9d9634babfmshc931f2147dc7481p117420jsn631a5633fffe";


            string word = wordEntry.Text;
            string requestUrl = $"https://wordsapiv1.p.rapidapi.com/words/{word}";

            if (!string.IsNullOrEmpty(word))
            {
                word = Uri.EscapeDataString(word);


                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-rapidapi-key", apiKey);
                _httpClient.DefaultRequestHeaders.Add("x-rapidapi-host", "wordsapiv1.p.rapidapi.com");
            }
            else
            {
                await DisplayAlert("Error", "Please enter a word to get its definition.", "OK");
            }

            // Get the request and transform
            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var json = JObject.Parse(responseBody);

                string wordResult = json["word"]?.ToString();
                string definition = json["results"]?[0]?["definition"]?.ToString();

                // update UI
                Dispatcher.Dispatch(() =>
                {
                    wordLabel.Text = wordResult ?? "Not Fount";
                    word = wordResult;
                    wordMeaningLabel.Text = definition ?? "Not Definition";
                    wordMeaning = definition;

                });
            }
            catch (HttpRequestException ex)
            {
                // if error
                Console.WriteLine($"Request exception: {ex.Message}");
            }
        }



        // translation button
        private async void TranslateButtonClicked(object sender, EventArgs e)
        {
            // Check that wordLabel displays a valid word.
            if (string.IsNullOrWhiteSpace(wordLabel.Text) || wordLabel.Text == "Not Found")
            {
                await DisplayAlert("Sorry", "Please get Meaning First", "OK");
            }
            else
            {
                // If there are valid words in wordLabel, perform the translation operation.
                string wordToTranslate = wordLabel.Text;
                string translatedText = await TranslateTextAsync(wordToTranslate);

                // update UI
                if (!translatedText.StartsWith("Error:"))
                {
                    translatedTextLabel.Text = translatedText;
                }
                else
                {
                    await DisplayAlert("Error", translatedText, "OK");
                }
            }
        }


        // traslation function
        private async Task<string> TranslateTextAsync(string textToTranslate)
        {
            string appId = "20240223001971331";
            string secretKey = "Ie03Vlv2Rp6DN0fATa0A";
            Random rd = new Random();
            string salt = rd.Next(100000).ToString();
            string sign = EncryptString(appId + textToTranslate + salt + secretKey);
            string url = "https://api.fanyi.baidu.com/api/trans/vip/translate";
            url += "?q=" + Uri.EscapeDataString(textToTranslate);
            url += "&from=en&to=zh";
            url += "&appid=" + appId;
            url += "&salt=" + salt;
            url += "&sign=" + sign;

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                string responseBody = await response.Content.ReadAsStringAsync();
                JObject jsonResponse = JObject.Parse(responseBody);
                if (jsonResponse["trans_result"]?[0]?["dst"] != null)
                {
                    string translatedText = jsonResponse["trans_result"]?[0]?["dst"]?.ToString() ?? " ";
                    translatedText = HttpUtility.HtmlDecode(translatedText);
                    return translatedText;
                }
                else
                {
                    return "Error: Translation result not found.";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        // Baidu Translation verification
        public static string EncryptString(string str)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(str);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        // translate sentence
        private async void TranslateButtonClickedS(object sender, EventArgs e)
        {
            // Check the wordMeaningLabel for a valid definition.
            if (string.IsNullOrWhiteSpace(wordMeaningLabel.Text) || wordMeaningLabel.Text == "Not Found")
            {
                await DisplayAlert("Sorry", "Please get mearning first", "ok");
            }
            else
            {
                // if found, transalate
                string meaningToTranslate = wordMeaningLabel.Text;
                string translatedText = await TranslateTextAsync(meaningToTranslate);

                // update UI
                if (!translatedText.StartsWith("Error:"))
                {
                    wordMeaningLabel.Text = translatedText;
                }
                else
                {
                    await DisplayAlert("error", translatedText, "OK");
                }
            }
        }


        // tts button
        private async void OnSpeakButtonClicked(object sender, EventArgs e)
        {
            // If no word is found, prompt the user
            if (string.IsNullOrWhiteSpace(wordLabel.Text) || wordLabel.Text == "Not Fount")
            {
                await DisplayAlert("Sorry", "Please get the word first or enter a correct word", "OK");
            }
            else
            {
                // If find it, speak
                string textToSpeak = wordLabel.Text;
                await ttsService.TextToSpeechAsync(textToSpeak);
            }
        }


        // tts function
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

