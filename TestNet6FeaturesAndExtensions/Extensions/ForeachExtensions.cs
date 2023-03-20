namespace ForTests.Extensions;

public static class ForeachExtensions
{
    public static CustomRangeEnumerator GetEnumerator(this Range range)
    {
        return new CustomRangeEnumerator(range);
    }

    public static CustomRangeEnumerator GetEnumerator(this int number)
    {
        return new CustomRangeEnumerator(new Range(0, number));
    }

    public struct CustomRangeEnumerator
    {
        private int _current;
        private readonly int _end;

        public CustomRangeEnumerator(Range range)
        {
            if (range.End.IsFromEnd)
                throw new NotSupportedException("End of range is not defined");

            _current = range.Start.Value - 1;
            _end = range.End.Value;
        }

        public int Current => _current;

        public bool MoveNext()
        {
            _current++;
            return _current <= _end;
        }
    }
}