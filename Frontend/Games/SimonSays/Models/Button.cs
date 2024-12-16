namespace Frontend.Games.SimonSays.Models
{
    public class Button
    {
        public bool IsError { get; private set; } = false;
        public string Text { get; set; }
        public int Index { get; set; }
        public bool IsLit { get; set; } = false;
        private bool IsDisabled;
        private readonly SimonSaysManager gameInstance;

        public Button(string buttonText, int index, SimonSaysManager game)
        {
            Text = buttonText ?? throw new ArgumentNullException(nameof(buttonText));
            Index = index;
            gameInstance = game ?? throw new ArgumentNullException(nameof(game));
        }

        public async Task OnClick(Action buttonPressed)
        {
            if (gameInstance.IsShowingSequence || IsDisabled)
            return;

            if (gameInstance.GameOver)
            {
                IsError = true;
                buttonPressed.Invoke();
                await FlashRed();
                IsError = false;
                buttonPressed.Invoke(); 
                return;
            }

            IsDisabled = true;
            IsLit = true;
            buttonPressed.Invoke();
            await Task.Delay(100);
            IsLit = false;
            buttonPressed.Invoke();
            await gameInstance.HandleTileClick(Index - 1);
            IsDisabled = false;
        }

        public async Task FlashButton(Action? colorChanged, int duration = 400, int delayBeforeFlash = 0, bool disableButton = false)
        {
            await Task.Delay(delayBeforeFlash);
            IsLit = true;
            if (disableButton)
                gameInstance.IsShowingSequence = true; // If sequence is showingsequence is set true button cant be clicked

            colorChanged?.Invoke();
            await Task.Delay(duration);
            IsLit = false;
            colorChanged?.Invoke();
            if (disableButton)
                gameInstance.IsShowingSequence = false;
        }
        public async Task FlashRed()
        {
            IsError = true;
            await Task.Delay(500); // Adjust the delay as needed
            IsError = false;
        }
    }
}