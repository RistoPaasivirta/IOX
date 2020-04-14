using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit, Size = 4)]
public class OffsetInt
{
    public OffsetInt() { }
    public OffsetInt(int Value) { value = Value; }
    public OffsetInt(byte[] Bytes)
    {
        value = Bytes[0];
        value |= Bytes[1] << 8;
        value |= Bytes[2] << 16;
        value |= Bytes[3] << 24;
    }

    [FieldOffset(0)] public int value;
    [FieldOffset(0)] public byte byte0;
    [FieldOffset(1)] public byte byte1;
    [FieldOffset(2)] public byte byte2;
    [FieldOffset(3)] public byte byte3;

    public byte[] bytes
    {
        get => new byte[4]
        {
            (byte)(value),
            (byte)(value >> 8),
            (byte)(value >> 16),
            (byte)(value >> 24)
        };
    }

    public static implicit operator OffsetInt(int Value) => new OffsetInt(Value);
    public static implicit operator int(OffsetInt OffsetInt) => OffsetInt.value;
    public static explicit operator OffsetInt(byte[] Bytes) => new OffsetInt(Bytes);

    public override string ToString()
    {
        return value.ToString();
    }
}