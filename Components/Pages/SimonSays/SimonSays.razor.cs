namespace PSInzinerija1.Components.Pages.SimonSays
{
    public partial class SimonSays 
    {
        protected List<Button> Buttons { get; } = Enumerable.Range(1, 9)
           .Select(index => new Button(index.ToString()))
           .ToList();

        public class Button(string buttonText)
        {
            public string buttonText { get; set; } = buttonText;
            public bool IsClicked { get; set; } = false;


            public async Task OnClick()
            {
                IsClicked = true;
                await Task.Delay(100);
                IsClicked = false;
            }
        }
    }
}