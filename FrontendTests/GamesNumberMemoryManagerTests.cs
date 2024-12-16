using System.Text.Json;

using Frontend.Games.NumberMemory;

using Xunit;

namespace FrontendTests
{
    public class NumberMemoryManagerTests
    {
        [Fact]
        public void SerializedStatistics_ReturnsCorrectJSON()
        {
            var manager = new NumberMemoryManager();
            manager.SetHighScore(10);

            string json = manager.SerializedStatistics;

            var expectedJson = JsonSerializer.Serialize(new { HighScore = 10 });
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void LoadStatisticsFromJSON_UpdatesHighScoreFromValidJSON()
        {
            var manager = new NumberMemoryManager();
            var json = JsonSerializer.Serialize(new { HighScore = 20 });

            manager.LoadStatisticsFromJSON(json);

            Assert.Equal(20, manager.HighScore);
        }

        [Fact]
        public void SetHighScore_UpdatesHighScoreIfHigher()
        {
            var manager = new NumberMemoryManager();
            manager.SetHighScore(10);

            Assert.True(manager.SetHighScore(15));
            Assert.Equal(15, manager.HighScore);

            Assert.False(manager.SetHighScore(5));
            Assert.Equal(15, manager.HighScore);
        }

        [Fact]
        public async Task StartNewGame_ResetsGameState()
        {
            var manager = new NumberMemoryManager();

            await manager.StartNewGame();

            Assert.Equal(1, manager.CurrentLevel);
            Assert.Equal(0, manager.Score);
            Assert.False(manager.GameOver);
            Assert.NotEmpty(manager.CurrentNumber);
        }

        [Fact]
        public async Task CheckUserInput_CorrectInput_AdvancesLevelAndScore()
        {
            var manager = new NumberMemoryManager();
            await manager.StartNewGame();

            manager.UserInput = manager.CurrentNumber;
            await manager.CheckUserInput();

            Assert.Equal(2, manager.CurrentLevel);
            Assert.Equal(1, manager.Score);
            Assert.False(manager.GameOver);
        }

        [Fact]
        public async Task CheckUserInput_IncorrectInput_SetsGameOver()
        {
            var manager = new NumberMemoryManager();
            await manager.StartNewGame();

            manager.UserInput = "wrong";
            await manager.CheckUserInput();

            Assert.True(manager.GameOver);
            Assert.Equal(0, manager.Score);
        }

        [Fact]
        public async Task GenerateNewNumber_ChangesCurrentNumber()
        {
            var manager = new NumberMemoryManager();
            await manager.StartNewGame();

            string initialNumber = manager.CurrentNumber;
            manager.UserInput = initialNumber;
            await manager.CheckUserInput();

            Assert.NotEqual(initialNumber, manager.CurrentNumber);
        }

        [Fact]
        public async Task CheckUserInput_TriggersOnStateChanged()
        {
            var manager = new NumberMemoryManager();
            bool eventTriggered = false;
            manager.OnStateChanged += () => eventTriggered = true;

            await manager.StartNewGame();
            manager.UserInput = manager.CurrentNumber;
            await manager.CheckUserInput();

            Assert.True(eventTriggered);
        }
    }
}
