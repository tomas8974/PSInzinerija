using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

using PSInzinerija1.Games.VisualMemory;

namespace PSInzinerija1.Components.Pages.VisualMemory
{
    public partial class VisualMemory
    {
        [Inject]
        ProtectedSessionStorage SessionStorage { get; set; }

        VisualMemoryManager Manager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Manager = new VisualMemoryManager(SessionStorage);
            await Manager.StartNewGame();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await FetchDataAsync();
            }
        }

        protected async Task FetchDataAsync()
        {
            await Manager.AttemptToFetchHighScore();
            StateHasChanged();
        }
    }
}