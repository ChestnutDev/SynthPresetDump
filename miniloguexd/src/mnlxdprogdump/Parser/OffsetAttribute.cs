namespace mnlxdprogdump;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
public sealed class OffsetAttribute : Attribute
{
    public OffsetAttribute(int offset)
    {
        Value = offset;
    }

    public OffsetAttribute(int offset, int bit) : this(offset, bit, bit) { }

    public OffsetAttribute(int offset, int startBit, int endBit) : this(offset)
    {
        if (startBit < 0 || startBit > 7)
        {
            throw new ArgumentOutOfRangeException(nameof(startBit), $"{nameof(startBit)} needs to be between 0 and 7");
        }

        if (endBit < 0 || endBit > 7)
        {
            throw new ArgumentOutOfRangeException(nameof(endBit), $"{nameof(endBit)} needs to be between 0 and 7");
        }

        if (endBit < startBit)
        {
            throw new ArgumentOutOfRangeException(nameof(endBit), $"{nameof(endBit)} ({endBit}) cannot be smaller than {nameof(startBit)} ({startBit})");
        }

        StartBit = startBit;
        EndBit = endBit;
    }

    public int Value { get; }
    public int? StartBit { get; }
    public int? EndBit { get; }
    public bool HasBitRange => StartBit.HasValue;
}
