using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

using Shared.Enums;
using Frontend.Services;
using Frontend.Games.NumberMemory;
using Frontend.Games;
using Frontend.Extensions;

namespace Frontend.Components.Pages.NumberMemory
{
    public partial class NumberMemory : ComponentBase
    {
#pragma warning disable CS8618
        [Inject]
        HighScoreAPIService HighScoreAPIService { get; set; }
        [Inject]
        ProtectedSessionStorage SessionStorage { get; set; }
        [Inject]
        ILogger<NumberMemory> Logger { get; set; }
#pragma warning restore CS8618
        NumberMemoryManager Manager { get; set; } = new NumberMemoryManager();

        protected override async Task OnInitializedAsync()
        {
            Manager.OnStateChanged = StateHasChanged;
            Manager.OnStatisticsChanged += async () =>
            {
                await SaveToDB(Manager);
                await SessionStorage.SaveStateSessionStorage(Manager);
            };
            await Manager.StartNewGame();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await FetchDataAsync();
            }
            if (!Manager.ShowNumber && !Manager.GameOver)
            {
                await InputElement.FocusAsync();
            }
        }


        // TODO: iskelti kitur
        private async Task SaveToDB(IGameManager gameManager)
        {
            var highScore = Manager.HighScore;
            var res = await HighScoreAPIService.SaveHighScoreToDbAsync(gameManager.GameID, highScore);

            Logger.LogInformation(res ? "Saved to database." : "Failed to save to database.");
        }

        private async Task DeleteHS(IGameManager gameManager)
        {
            var res = await HighScoreAPIService.DeleteFromDbAsync(gameManager.GameID);

            Logger.LogInformation(res ? "Deleted successfully" : "Failed to delete");
        }

        private async Task FetchDataAsync()
        {
            var res = await HighScoreAPIService.GetHighScoreAsync(AvailableGames.NumberMemory);
            if (res != null)
            {
                Manager.SetHighScore(res.Value);
                Logger.LogInformation("Loaded from database.");
            }
            else
            {
                await SessionStorage.LoadFromSessionStorage(Manager);
                Logger.LogInformation("Loaded from session storage.");
            }
            StateHasChanged();
        }
    }
}