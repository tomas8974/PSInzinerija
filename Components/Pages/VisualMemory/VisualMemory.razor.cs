using PSInzinerija1.Games.VisualMemory;

namespace PSInzinerija1.Components.Pages.VisualMemory
{
    public partial class VisualMemory
    {
        GameManager Manager { get; } = new GameManager();

        protected override async Task OnInitializedAsync()
        {
            await Manager.StartNewGame();
        }
    }
}