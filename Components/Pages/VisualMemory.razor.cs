using System.Collections;

namespace PSInzinerija1.Components.Pages
{
    public partial class VisualMemory
    {
        protected Sequence sequence = new();
        public enum SequenceValue
        {
            Valid,
            Invalid
        }

        public class Sequence : IEnumerable<SequenceValue>
        {
            #region Inner classes
            public class SequenceEnumerator : IEnumerator<SequenceValue>
            {
                private readonly int _totalNums;
                private int _currentNum = -1;

                private List<int> _validSequence { get; }

                public SequenceValue Current => _validSequence.Contains(_currentNum) ?
                    SequenceValue.Valid : SequenceValue.Invalid;

                object IEnumerator.Current => throw new NotImplementedException();

                public SequenceEnumerator(List<int> validNums, int totalNums)
                {
                    _validSequence = validNums;
                    _totalNums = totalNums;
                }

                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    _currentNum++;
                    return _currentNum < _totalNums;
                }

                public void Reset()
                {
                    _currentNum = -1;
                }
            }
            #endregion

            public Sequence()
            {
                GenerateNewSequence();
            }

            public int GridSize { get; private set; } = 3;
            public int SequenceLength { get; private set; } = 9;
            public int ValidAmount { get; private set; } = 3;
            public List<int> CurrentSequence { get; private set; } = [];

            public void GenerateNewSequence()
            {
                var rand = new Random();
                int gridElements = GridSize * GridSize;
                CurrentSequence = Enumerable.Range(0, gridElements)
                    .OrderBy(i => rand.Next())
                    .Take(ValidAmount)
                    .ToList();
                SequenceLength = gridElements;
            }

            public void IncreaseDifficulty()
            {
                ValidAmount++;
                if (ValidAmount > SequenceLength / 2)
                {
                    GridSize++;
                }
            }

            public IEnumerator<SequenceValue> GetEnumerator()
            {
                return new SequenceEnumerator(CurrentSequence, SequenceLength);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

    }
}