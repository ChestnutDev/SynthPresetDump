namespace mnlxdprogdump;

public class ReportGeneratorInput
{
    public ProgramData Program { get; set; }
    public SequencerDataV2? SequencerV2 { get; set; }
    public UserOscillatorDescriptions UserUnitDescriptions { get; set; } = new UserOscillatorDescriptions();
    public UserUnitMappings UserUnitMappings { get; set; } = new UserUnitMappings();

    public ReportGeneratorInput(ProgramData program)
    {
        Program = program ?? throw new ArgumentNullException(nameof(program));
    }

    public UserOscillatorDescription GetUserOscillatorDescription()
    {
        var slotNum = (byte)(Program.SelectedMultiOscUser + 1);
        var userOscMapping = UserUnitMappings?.GetUserOscillator(slotNum) ?? "";
        var userOsc = UserUnitDescriptions?.GetUserOscillatorDescription(userOscMapping) ?? UserOscillatorDescription.CreateGeneric(userOscMapping);
        return userOsc;
    }

    public string GetUserModFxName()
    {
        var modFxSlotNum = (byte)(Program.ModFxUser + 1);
        var modFxName = UserUnitMappings?.GetUserModFx(modFxSlotNum);
        if (string.IsNullOrEmpty(modFxName)) { modFxName = "USER MOD"; }
        return $"{modFxName} (#{modFxSlotNum})";
    }

    public string GetReverbFxName()
    {
        byte? reverbFxSlotNum = Program.ReverbSubType switch
        {
            ReverbSubType.User1 => 1,
            ReverbSubType.User2 => 2,
            ReverbSubType.User3 => 3,
            ReverbSubType.User4 => 4,
            ReverbSubType.User5 => 5,
            ReverbSubType.User6 => 6,
            ReverbSubType.User7 => 7,
            ReverbSubType.User8 => 8,
            _ => null
        };

        if (reverbFxSlotNum == null)
        {
            // Not a User Reverb FX
            return Program.ReverbSubType.ToString();
        }

        var revName = UserUnitMappings?.GetUserReverbFx(reverbFxSlotNum.Value);
        if (string.IsNullOrEmpty(revName)) { revName = "USER REV"; }
        return $"{revName} (#{reverbFxSlotNum})";
    }

    public string GetDelayFxName()
    {
        byte? delayFxSlotNum = Program.DelaySubType switch
        {
            DelaySubType.User1 => 1,
            DelaySubType.User2 => 2,
            DelaySubType.User3 => 3,
            DelaySubType.User4 => 4,
            DelaySubType.User5 => 5,
            DelaySubType.User6 => 6,
            DelaySubType.User7 => 7,
            DelaySubType.User8 => 8,
            _ => null
        };

        if (delayFxSlotNum == null)
        {
            // Not a User Delay FX
            return Program.DelaySubType.ToString();
        }

        var delayName = UserUnitMappings?.GetUserDelayFx(delayFxSlotNum.Value);
        if (string.IsNullOrEmpty(delayName)) { delayName = "USER DELAY"; }
        return $"{delayName} (#{delayFxSlotNum})";
    }
}