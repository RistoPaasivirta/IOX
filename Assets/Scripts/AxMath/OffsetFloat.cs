using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit, Size = 4)]
public class OffsetFloat
{
    public OffsetFloat() { }
    public OffsetFloat(float Value) { value = Value; }
    public OffsetFloat(byte[] Bytes) 
    {
        byte0 = Bytes[0];
        byte1 = Bytes[1];
        byte2 = Bytes[2];
        byte3 = Bytes[3];
    }

    [FieldOffset(0)] public float value;
    [FieldOffset(0)] public byte byte0;
    [FieldOffset(1)] public byte byte1;
    [FieldOffset(2)] public byte byte2;
    [FieldOffset(3)] public byte byte3;

    public byte[] bytes
    {
        get => new byte[4]
        {
            byte0,
            byte1,
            byte2,
            byte3
        };
    }

    public static implicit operator OffsetFloat(float Value) => new OffsetFloat(Value);
    public static implicit operator float(OffsetFloat OffsetFloat) => OffsetFloat.value;
    public static explicit operator OffsetFloat(byte[] Bytes) => new OffsetFloat(Bytes);

    public override string ToString()
    {
        return value.ToString();
    }
}