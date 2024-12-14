using System.Text.Json;
using System.Text.Json.Nodes;

using Shared.Enums;

namespace Frontend.Games.NumberMemory
{
    public class NumberMemoryManager : IGameManager
    {
        public int CurrentLevel { get; private set; } = 1;
        public string CurrentNumber { get; private set; } = string.Empty;
        public string UserInput { get; set; } = string.Empty;
        public bool ShowNumber { get; private set; } = true;
        public bool GameOver { get; private set; } = false;
        public int Score { get; private set; } = 0;
        public int HighScore { get; private set; } = 0;

        private readonly Random _random = new Random();
        public event Action? OnStatisticsChanged;
        public Action? OnStateChanged { get; set; }

        public AvailableGames GameID => AvailableGames.NumberMemory;

        public string SerializedStatistics
        {
            get
            {
                var obj = new
                {
                    HighScore
                };
                var json = JsonSerializer.Serialize(obj);

                return json.ToString();
            }
        }

        public async Task StartNewGame()
        {
            CurrentLevel = 1;
            Score = 0;
            GameOver = false;
            await GenerateNewNumber();
        }

        private async Task GenerateNewNumber()
        {
            ShowNumber = true;
            OnStateChanged?.Invoke();
            UserInput = string.Empty;
            CurrentNumber = new string(Enumerable.Range(0, CurrentLevel)
                                                 .Select(_ => _random.Next(0, 10).ToString()[0])
                                                 .ToArray());

            OnStateChanged?.Invoke();
            await Task.Delay(2000);
            ShowNumber = false;
            OnStateChanged?.Invoke();
            OnStatisticsChanged?.Invoke();

        }

        public async Task CheckUserInput()
        {
            if (UserInput == CurrentNumber)
            {
                Score++;
                CurrentLevel++;
                await GenerateNewNumber();
            }
            else
            {
                GameOver = true;
                UpdateHighScore();
            }
        }

        private void UpdateHighScore()
        {
            if (Score > HighScore)
            {
                HighScore = Score;
                OnStatisticsChanged?.Invoke();
            }
        }
        public void LoadStatisticsFromJSON(string? json)
        {
            if (json == null)
            {
                return;
            }

            var jsonObject = JsonNode.Parse(json)?.AsObject();

            if (jsonObject != null && jsonObject[nameof(HighScore)] != null)
            {
                HighScore = jsonObject[nameof(HighScore)].Deserialize<int>();
            }

        }

        public bool SetHighScore(int? highScore)
        {
            if (highScore == null || highScore.Value < HighScore)
            {
                return false;
            }

            HighScore = highScore.Value;
            return true;
        }
    }

}
