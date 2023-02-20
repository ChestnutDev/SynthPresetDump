using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace mnlxdprogdump;

public static class ProgramParser
{
    public static T Read<T>(ReadOnlySpan<byte> input) where T : new()
    {
        if (!BitConverter.IsLittleEndian)
        {
            throw new PlatformNotSupportedException("This is only supported on Little Endian platforms.");
        }

        var result = new T();

        var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        var fields = typeof(T).GetFields(bindingFlags);
        var properties = typeof(T).GetProperties(bindingFlags);

        foreach (var field in fields)
        {
            var offset = field.GetCustomAttribute<OffsetAttribute>();
            if (offset == null) { continue; }

            var value = GetValueToSet(field.FieldType, field, input, offset, field.Name);
            ValidateValueToSet(field, value);
            field.SetValue(result, value);
        }

        foreach (var prop in properties)
        {
            var offset = prop.GetCustomAttribute<OffsetAttribute>();
            if (offset == null) { continue; }

            var value = GetValueToSet(prop.PropertyType, prop, input, offset, prop.Name);
            ValidateValueToSet(prop, value);
            prop.SetValue(result, value);
        }

        return result;
    }

    private static void ValidateValueToSet(MemberInfo mi, object? value)
    {
        var attrs = mi.GetCustomAttributes<ValidationAttribute>();
        foreach (var attr in attrs)
        {
            if (!attr.IsValid(value))
            {
                throw new InvalidOperationException(attr.FormatErrorMessage(mi.Name) + " Value was: " + value);
            }
        }
    }

    private static object? GetValueToSet(Type targetType, MemberInfo mi, ReadOnlySpan<byte> input, OffsetAttribute offset, string name)
    {
        if (targetType == typeof(bool))
        {
            return ReadBool(input, offset, name);
        }
        else if (IsIntegerField(targetType))
        {
            return GetIntegerValue(targetType, input, offset, name);
        }
        else if (targetType.IsEnum)
        {
            var rawEnumValue = GetIntegerValue(targetType.GetEnumUnderlyingType(), input, offset, name);
            if (!Enum.IsDefined(targetType, rawEnumValue))
            {
                throw new InvalidOperationException($"Undefined Enum value for {name}: {rawEnumValue}");
            }
            return rawEnumValue;
        }
        else if (targetType == typeof(string))
        {
            if (offset.HasBitRange) { throw new InvalidOperationException($"Bit Ranges are not supported on strings. Member name: {name}"); }

            var len = mi.GetCustomAttribute<StringLengthAttribute>();
            if (len == null)
            {
                throw new InvalidOperationException($"Strings need a {nameof(StringLengthAttribute)}, but {name} did not.");
            }
            return Encoding.ASCII.GetString(input.Slice(offset.Value, len.MaximumLength)).Replace("\0", "");
        }
        else if (targetType == typeof(StepEventData))
        {
            return Read<StepEventData>(input.Slice(offset.Value));
        }
        else if (targetType == typeof(MotionData))
        {
            return Read<MotionData>(input.Slice(offset.Value));
        }
        else if (targetType == typeof(SequencerData))
        {
            return Read<SequencerData>(input);
        }
        else
        {
            throw new InvalidOperationException($"Unsupported type for {name}: {targetType.FullName}");
        }
    }

    private static byte ReadByte(ReadOnlySpan<byte> input, OffsetAttribute offset, string name)
    {
        var result = input[offset.Value];
        if (!offset.HasBitRange) { return result; }

        var br = new BitArray(new byte[] { result });
        for (int i = 0; i < offset.StartBit!.Value; i++)
        {
            br.Set(i, false);
        }
        for (int i = (br.Length-1); i > offset.EndBit!.Value; i--)
        {
            br.Set(i, false);
        }
        br.RightShift(offset.StartBit.Value);
        var temp = new byte[1];
        br.CopyTo(temp, 0);
        return temp[0];
    }

    private static bool ReadBool(ReadOnlySpan<byte> input, OffsetAttribute offset, string name)
    {
        var val = ReadByte(input, offset, name);
        return val switch
        {
            0x00 => false,
            0x01 => true,
            _ => throw new InvalidOperationException($"Value for {name} was not between 0-1: " + val)
        };
    }

    private static bool IsIntegerField(Type fieldType) =>
        fieldType == typeof(byte) ||
        fieldType == typeof(ushort) || fieldType == typeof(short) ||
        fieldType == typeof(uint) || fieldType == typeof(int) ||
        fieldType == typeof(ulong) || fieldType == typeof(long);

    private static object GetIntegerValue(Type integerType, ReadOnlySpan<byte> input, OffsetAttribute offsetAttr, string name)
    {
        if (integerType == typeof(byte))
        {
            return ReadByte(input, offsetAttr, name);
        }

        if (offsetAttr.HasBitRange)
        {
            throw new InvalidOperationException($"Bit Ranges are not supported on integers other than bytes. Member name: {name}");
        }

        var offset = offsetAttr.Value;

        if (integerType == typeof(ushort))
        {
            return BitConverter.ToUInt16(input.Slice(offset, 2));
        }
        if (integerType == typeof(short))
        {
            return BitConverter.ToInt16(input.Slice(offset, 2));
        }
        if (integerType == typeof(uint))
        {
            return BitConverter.ToUInt32(input.Slice(offset, 4));
        }
        if (integerType == typeof(int))
        {
            return BitConverter.ToInt32(input.Slice(offset, 4));
        }
        if (integerType == typeof(ulong))
        {
            return BitConverter.ToUInt64(input.Slice(offset, 8));
        }
        if (integerType == typeof(long))
        {
            return BitConverter.ToInt64(input.Slice(offset, 8));
        }
        throw new ArgumentException($"Unhandled Integer Type for {name}: {integerType.FullName}");
    }
}