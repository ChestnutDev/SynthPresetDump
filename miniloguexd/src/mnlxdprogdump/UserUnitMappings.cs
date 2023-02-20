namespace mnlxdprogdump;

public class UserUnitMappings
{
    public string GetUserOscillator(byte oscSlotNum) => oscSlotNum switch
    {
        1 => UserOscillator1,
        2 => UserOscillator2,
        3 => UserOscillator3,
        4 => UserOscillator4,
        5 => UserOscillator5,
        6 => UserOscillator6,
        7 => UserOscillator7,
        8 => UserOscillator8,
        9 => UserOscillator9,
        10 => UserOscillator10,
        11 => UserOscillator11,
        12 => UserOscillator12,
        13 => UserOscillator13,
        14 => UserOscillator14,
        15 => UserOscillator15,
        16 => UserOscillator16,
        _ => throw new ArgumentOutOfRangeException(nameof(oscSlotNum), "User Oscillator Slot number needs to be between 1 and 16")
    };

    public string GetUserModFx(byte modFxSlotNum) => modFxSlotNum switch
    {
        1 => UserModFx1,
        2 => UserModFx2,
        3 => UserModFx3,
        4 => UserModFx4,
        5 => UserModFx5,
        6 => UserModFx6,
        7 => UserModFx7,
        8 => UserModFx8,
        9 => UserModFx9,
        10 => UserModFx10,
        11 => UserModFx11,
        12 => UserModFx12,
        13 => UserModFx13,
        14 => UserModFx14,
        15 => UserModFx15,
        16 => UserModFx16,
        _ => throw new ArgumentOutOfRangeException(nameof(modFxSlotNum), "User Mod FX Slot number needs to be between 1 and 16")
    };

    public string GetUserDelayFx(byte delayFxSlotNum) => delayFxSlotNum switch
    {
        1 => UserDelayFx1,
        2 => UserDelayFx2,
        3 => UserDelayFx3,
        4 => UserDelayFx4,
        5 => UserDelayFx5,
        6 => UserDelayFx6,
        7 => UserDelayFx7,
        8 => UserDelayFx8,
        _ => throw new ArgumentOutOfRangeException(nameof(delayFxSlotNum), "User Delay FX Slot number needs to be between 1 and 8")
    };

    public string GetUserReverbFx(byte reverbFxSlotNum) => reverbFxSlotNum switch
    {
        1 => UserReverbFx1,
        2 => UserReverbFx2,
        3 => UserReverbFx3,
        4 => UserReverbFx4,
        5 => UserReverbFx5,
        6 => UserReverbFx6,
        7 => UserReverbFx7,
        8 => UserReverbFx8,
        _ => throw new ArgumentOutOfRangeException(nameof(reverbFxSlotNum), "User Reverb FX Slot number needs to be between 1 and 8")
    };


    // I know this could be done with arrays or other list structures,
    // but I like the KISS simplicity of the JSON file and not needing to deal
    // with skipping empty entries, enforcing the correct array length, etc.
    public string UserOscillator1 { get; set; } = "";
    public string UserOscillator2 { get; set; } = "";
    public string UserOscillator3 { get; set; } = "";
    public string UserOscillator4 { get; set; } = "";
    public string UserOscillator5 { get; set; } = "";
    public string UserOscillator6 { get; set; } = "";
    public string UserOscillator7 { get; set; } = "";
    public string UserOscillator8 { get; set; } = "";
    public string UserOscillator9 { get; set; } = "";
    public string UserOscillator10 { get; set; } = "";
    public string UserOscillator11 { get; set; } = "";
    public string UserOscillator12 { get; set; } = "";
    public string UserOscillator13 { get; set; } = "";
    public string UserOscillator14 { get; set; } = "";
    public string UserOscillator15 { get; set; } = "";
    public string UserOscillator16 { get; set; } = "";

    public string UserModFx1 { get; set; } = "";
    public string UserModFx2 { get; set; } = "";
    public string UserModFx3 { get; set; } = "";
    public string UserModFx4 { get; set; } = "";
    public string UserModFx5 { get; set; } = "";
    public string UserModFx6 { get; set; } = "";
    public string UserModFx7 { get; set; } = "";
    public string UserModFx8 { get; set; } = "";
    public string UserModFx9 { get; set; } = "";
    public string UserModFx10 { get; set; } = "";
    public string UserModFx11 { get; set; } = "";
    public string UserModFx12 { get; set; } = "";
    public string UserModFx13 { get; set; } = "";
    public string UserModFx14 { get; set; } = "";
    public string UserModFx15 { get; set; } = "";
    public string UserModFx16 { get; set; } = "";

    public string UserDelayFx1 { get; set; } = "";
    public string UserDelayFx2 { get; set; } = "";
    public string UserDelayFx3 { get; set; } = "";
    public string UserDelayFx4 { get; set; } = "";
    public string UserDelayFx5 { get; set; } = "";
    public string UserDelayFx6 { get; set; } = "";
    public string UserDelayFx7 { get; set; } = "";
    public string UserDelayFx8 { get; set; } = "";

    public string UserReverbFx1 { get; set; } = "";
    public string UserReverbFx2 { get; set; } = "";
    public string UserReverbFx3 { get; set; } = "";
    public string UserReverbFx4 { get; set; } = "";
    public string UserReverbFx5 { get; set; } = "";
    public string UserReverbFx6 { get; set; } = "";
    public string UserReverbFx7 { get; set; } = "";
    public string UserReverbFx8 { get; set; } = "";
}
