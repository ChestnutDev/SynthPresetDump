namespace mnlxdprogdump;

public class UserOscillatorDescriptions
{
    public Dictionary<string, UserOscillatorDescription>? UserOscillators { get; set; }

    public UserOscillatorDescription GetUserOscillatorDescription(string name)
    {
        if (UserOscillators?.ContainsKey(name) == true && UserOscillators[name] != null)
        {
            return UserOscillators[name];
        }
        return UserOscillatorDescription.CreateGeneric(name);
    }
}

public class UserOscillatorDescription
{
    public string Name { get; set; } = "USER OSC";
    public string UserParamLabel1 { get; set; } = "Param 1";
    public string UserParamLabel2 { get; set; } = "Param 2";
    public string UserParamLabel3 { get; set; } = "Param 3";
    public string UserParamLabel4 { get; set; } = "Param 4";
    public string UserParamLabel5 { get; set; } = "Param 5";
    public string UserParamLabel6 { get; set; } = "Param 6";

    public string? GetUserParamLabel(byte paramNum) => paramNum switch
    {
        1 => UserParamLabel1,
        2 => UserParamLabel2,
        3 => UserParamLabel3,
        4 => UserParamLabel4,
        5 => UserParamLabel5,
        6 => UserParamLabel6,
        _ => throw new ArgumentOutOfRangeException(nameof(paramNum), "Parameter Number must be between 1 and 6")
    };

    public static string FormatParamValue(UserParamType type, byte value)
    {
        if (type == UserParamType.PercentBipolar)
        {

            int signed = value - 100;
            if (signed <= 0) { return $"{signed}%"; }
            return $"{signed}%";
        }
        else if (type == UserParamType.PercentType)
        {
            return $"{value}%";
        }
        else if (type == UserParamType.Select || type == UserParamType.Count)
        {
            // It's an Index, but Indexes start at 1 in the UI
            return (value + 1).ToString();
        }
        return value.ToString();
    }

    public static UserOscillatorDescription CreateGeneric(string? name = null) => new()
    {
        Name = string.IsNullOrEmpty(name) ? "USER OSC" : name,
    };
}
