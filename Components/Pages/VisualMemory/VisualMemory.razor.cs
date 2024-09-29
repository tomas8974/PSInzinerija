using PSInzinerija1.Games.VisualMemory;

namespace PSInzinerija1.Components.Pages.VisualMemory
{
    public partial class VisualMemory
    {
        VisualMemoryManager Manager { get; } = new VisualMemoryManager();

        protected override async Task OnInitializedAsync()
        {
            await Manager.StartNewGame();
        }
    }
}