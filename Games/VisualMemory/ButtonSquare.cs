namespace PSInzinerija1.Games.VisualMemory
{
    public class ButtonSquare(SequenceValue value, int index, bool pressed = false)
    {
        public SequenceValue Value = value;
        public int Index { get; } = index;
        public bool Pressed { get; set; } = pressed;
    }

}