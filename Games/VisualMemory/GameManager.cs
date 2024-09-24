namespace PSInzinerija1.Games.VisualMemory
{
    public class GameManager
    {
        private readonly int _roundStartDelay = 1000;
        public int Score { get; private set; } = 0;
        private int _mistakeCount = 0;
        private int _correctCount = 0;
        public Sequence Sequence { get; private set; } = new();

        public async Task StartNewGame()
        {
            Score = 0;
            Sequence = new();
            ResetRound();
            await BeginRound();
        }

        private void DisableButtons()
        {
            foreach (var button in Sequence)
            {
                button.Pressed = true;
            }
        }

        private void EnableButtons()
        {
            foreach (var button in Sequence)
            {
                button.Pressed = false;
            }
        }

        public async Task HandleInput(ButtonSquare buttonSquare)
        {
            if (buttonSquare.Pressed)
            {
                return;
            }
            buttonSquare.Pressed = true;

            if (buttonSquare.Value == SequenceValue.Invalid)
            {
                _mistakeCount++;
            }
            else
            {
                _correctCount++;
            }

            if (_mistakeCount >= 3)
            {
                // game over
                await StartNewGame();
            }
            else if (_correctCount >= Sequence.ValidButtonAmount)
            {
                ResetRound();
                await Advance();
            }

        }

        private void ResetRound()
        {
            _correctCount = 0;
            _mistakeCount = 0;
        }

        private async Task Advance()
        {
            Sequence.IncreaseDifficulty();
            Sequence.GenerateNewSequence();
            Score++;
            await BeginRound();
        }

        private async Task BeginRound()
        {
            DisableButtons();
            await Task.Delay(_roundStartDelay);
            EnableButtons();
        }
    }
}