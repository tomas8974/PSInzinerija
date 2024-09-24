using System.Collections;

namespace PSInzinerija1.Games.VisualMemory
{
    public enum SequenceValue
    {
        Valid,
        Invalid
    }

    public class Sequence : IEnumerable<ButtonSquare>
    {
        public Sequence()
        {
            GenerateNewSequence();
        }

        public int GridSize { get; private set; } = 3;
        public int Length { get => CurrentSequence.Count; }
        public int ValidButtonAmount { get; private set; } = 3;
        private List<ButtonSquare> CurrentSequence { get; set; } = [];

        public void GenerateNewSequence()
        {
            var rand = new Random();
            int gridElements = GridSize * GridSize;
            CurrentSequence = Enumerable.Range(0, gridElements)
                .Select(i => new ButtonSquare(i >= ValidButtonAmount ? SequenceValue.Invalid : SequenceValue.Valid, i))
                .OrderBy(_ => rand.Next())
                .ToList();
        }

        public IEnumerator<ButtonSquare> GetEnumerator()
        {
            return CurrentSequence.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void IncreaseDifficulty()
        {
            ValidButtonAmount++;
            if (ValidButtonAmount > Length / 2)
            {
                GridSize++;
            }
        }
    }

}