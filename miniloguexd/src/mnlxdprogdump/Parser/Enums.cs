namespace mnlxdprogdump;

/// <summary>
/// *note P3 (VOICE MODE TYPE)
/// </summary>
public enum VoiceModeType : byte
{
    None = 0,
    Arp = 1,
    Chord = 2,
    Unison = 3,
    Poly = 4
}

/// <summary>
/// *note P4 (VCO1 WAVE, VCO2 WAVE, LFO WAVE)
/// </summary>
public enum VcoWave : byte
{
    SQR = 0,
    TRI = 1,
    SAW = 2
}

public enum MultiOscType : byte
{
    Noise = 0,
    VPM = 1,
    User = 2
}

/// <summary>
/// *note P6 (SELECT NOISE)
/// </summary>
public enum MultiOscNoise : byte
{
    High = 0,
    Low = 1,
    Peak = 2,
    Decim = 3
}

/// <summary>
/// *note P7 (SELECT VPM)
/// </summary>
public enum MultiOscVPM : byte
{
    Sin1 = 0,
    Sin2 = 1,
    Sin3 = 2,
    Sin4 = 3,
    Saw1 = 4,
    Saw2 = 5,
    Squ1 = 6,
    Squ2 = 7,
    Fat1 = 8,
    Fat2 = 9,
    Air1 = 10,
    Air2 = 11,
    Decay1 = 12,
    Decay2 = 13,
    Creep = 14,
    Throat = 15
}

public enum EGTarget : byte
{
    Cutoff = 0,
    Pitch2 = 1,
    Pitch = 2
}

/// <summary>
/// *note P12 (MOD FX TYPE)
/// </summary>
public enum ModFxType : byte
{
    None = 0,
    Chorus = 1,
    Ensemble = 2,
    Phaser = 3,
    Flanger = 4,
    User = 5
}

/// <summary>
/// *note P13 (MOD FX CHORUS)
/// </summary>
public enum ModFxChorus : byte
{
    Stereo = 0,
    Light = 1,
    Deep = 2,
    Triphase = 3,
    Harmonic = 4,
    Mono = 5,
    Feedback = 6,
    Vibrato = 7
}

/// <summary>
/// *note P14 (MOD FX ENSEMBLE)
/// </summary>
public enum ModFxEnsemble : byte
{
    Stereo = 0,
    Light = 1,
    Mono = 2
}

/// <summary>
/// *note P15 (MOD FX PHASER)
/// </summary>
public enum ModFxPhaser : byte
{
    Stereo = 0,
    Fast = 1,
    Orange = 2,
    Small = 3,
    SmallReso = 4,
    Black = 5,
    Formant = 6,
    Twinkle = 7
}

/// <summary>
/// *note P16 (MOD FX FLANGER)
/// </summary>
public enum ModFxFlanger : byte
{
    Stereo = 0,
    Light = 1,
    Mono = 2,
    HighSweep = 3,
    MidSweep = 4,
    PanSweep = 5,
    MonoSweep = 6,
    Triphase = 7
}

/// <summary>
/// *note P17 (DELAY SUB TYPE)
/// </summary>
public enum DelaySubType : byte
{
    Stereo = 0,
    Mono = 1,
    PingPong = 2,
    Hipass = 3,
    Tape = 4,
    OneTap = 5,
    StereoBPM = 6,
    MonoBPM = 7,
    PingBPM = 8,
    HipassBPM = 9,
    TapeBPM = 10,
    Doubling = 11,
    User1 = 12,
    User2 = 13,
    User3 = 14,
    User4 = 15,
    User5 = 16,
    User6 = 17,
    User7 = 18,
    User8 = 19
}

/// <summary>
/// *note P18 (REVERB SUB TYPE)
/// </summary>
public enum ReverbSubType : byte
{
    Hall = 0,
    Smooth = 1,
    Arena = 2,
    Plate = 3,
    Room = 4,
    EarlyRef = 5,
    Space = 6,
    Riser = 7,
    Submarine = 8,
    Horror = 9,
    User1 = 10,
    User2 = 11,
    User3 = 12,
    User4 = 13,
    User5 = 14,
    User6 = 15,
    User7 = 16,
    User8 = 17
}

/// <summary>
/// *note P19 (JOYSTICK ASSIGN, CN IN ASSIGN, MIDI AFTER TOUCH ASSIGN)
/// </summary>
public enum AssignTarget : byte
{
    GateTime = 0,
    Portamento = 1,
    VMDepth = 2,
    Vco1Pitch = 3,
    Vco1Shape = 4,
    Vco2Pitch = 5,
    Vco2Shape = 6,
    CrossMod = 7,
    MultiShape = 8,
    Vco1Level = 9,
    Vco2Level = 10,
    MultiLevel = 11,
    FilterCutoff = 12,
    FilterResonance = 13,
    AmpEGAttack = 14,
    AmpEGDecay = 15,
    AmpEGSustain = 16,
    AmpEGRelease = 17,
    EGAttack = 18,
    EGDecay = 19,
    EGInt = 20,
    LFORate = 21,
    LFOInt = 22,
    ModFxSpeed = 23,
    ModFxDepth = 24,
    ReverbTime = 25,
    ReverbDepth = 26,
    DelayTime = 27,
    DelayDepth = 28
}

/// <summary>
/// *note P20 (CV IN MODE)
/// </summary>
public enum CVInMode : byte
{
    Modulation = 0,

    /// <summary>
    /// CV/Gate(+)
    /// </summary>
    CVGatePlus = 1,

    /// <summary>
    /// CV/Gate(-)
    /// </summary>
    CVGateMinus = 2
}

/// <summary>
/// *note P21 (MICRO TUNING)
/// </summary>
public enum MicroTuning : byte
{
    EqualTemp = 0,
    PureMajor = 1,
    PureMinor = 2,
    Pythagorean = 3,
    Werckmeister = 4,
    Kirnburger = 5,
    Slendro = 6,
    Pelog = 7,
    Ionian = 8,
    Dorian = 9,
    Aeolian = 10,
    MajorPenta = 11,
    MinorPenta = 12,
    Reverse = 13,
    AFX001 = 14,
    AFX002 = 15,
    AFX003 = 16,
    AFX004 = 17,
    AFX005 = 18,
    AFX006 = 19,
    DC001 = 20,
    DC002 = 21,
    DC003 = 22,
    UserScale1 = 128,
    UserScale2 = 129,
    UserScale3 = 130,
    UserScale4 = 131,
    UserScale5 = 132,
    UserScale6 = 133,
    UserOctave1 = 134,
    UserOctave2 = 135,
    UserOctave3 = 136,
    UserOctave4 = 137,
    UserOctave5 = 138,
    UserOctave6 = 139
}

/// <summary>
/// *note P22 (LFO TARGET OSC)
/// </summary>
public enum LFOTargetOsc : byte
{
    All = 0,
    Vco1And2 = 1,
    Vco2 = 2,
    Multi = 3
}

public enum LFOMode : byte
{
    OneShot = 0,
    Normal = 1,
    BPM = 2
}

public enum LFOTarget : byte
{
    Cutoff = 0,
    Shape = 1,
    Pitch = 2
}

public enum MultiRouting : byte
{
    PreVCF = 0,
    PostVCF = 1
}

public enum PortamentoMode : byte
{
    Auto = 0,
    On = 1
}

/// <summary>
/// *note P24 (USER PARAM1~6)
/// </summary>
public enum UserParamType : byte
{
    /// <summary>
    /// (USER PARAMETER : 0~101 :    0 ~ 100%)
    /// </summary>
    PercentType = 0,

    /// <summary>
    /// (USER PARAMETER : 0~200 : -100 ~ 100)
    /// </summary>
    PercentBipolar = 1,

    /// <summary>
    /// (USER PARAMETER : 0~100 :    1 ~ 101)
    /// </summary>
    Select = 2,

    Count = 3
}

/// <summary>
/// *note S4 says that these should
/// start at 0, but actual testing
/// shows that they actually start
/// at 1 for 64th.
/// </summary>
public enum ARPRate : byte
{
    SixtyFour = 1,
    FourtyEight = 2,
    ThirtyTwo = 3,
    TwentyFour = 4,
    Sixteen = 5,
    SixteenDotted = 6,
    Twelve = 7,
    Eight = 8,
    EightDotted = 9,
    Six = 10,
    Four = 11
}

/// <summary>
/// *note S2-1 (Motion Parameter ID)
/// </summary>
public enum MotionParameterId : byte
{
    None = 0,
    Portamento = 15,
    VoiceModeDepth = 16,
    VoiceModeType = 17,

    Vco1Wave = 18,
    Vco1Octave = 19,
    Vco1Pitch = 20,
    Vco1Shape = 21,

    Vco2Wave = 22,
    Vco2Octave = 23,
    Vco2Pitch = 24,
    Vco2Shape = 25,

    Sync = 26,
    RingMod = 27,
    CrossModDepth = 28,

    MultiEngineType = 29,
    MultiEngineNoiseType = 30,
    MultiEngineVPMType = 31,
    MultiShapeNoise = 33,
    MultiShapeVPM = 34,
    MultiShapeUser = 35,
    MultiShiftShapeNoise = 36,
    MultiShiftShapeVPM = 37,
    MultiShiftShapeUser = 38,

    Vco1Level = 39,
    Vco2Level = 40,
    MultiEngineLevel = 41,

    Cutoff = 42,
    Resonance = 43,

    KeyTrack = 45,

    AmpEGAttack = 46,
    AmpEGDecay = 47,
    AmpEGSustain = 48,
    AmpEGRelease = 49,
    EGAttack = 50,
    EGDecay = 51,
    EGInt = 52,
    EGTarget = 53,

    LFOWave = 54,
    LFOMode = 55,
    LFORate = 56,
    LFOInt = 57,
    LFOTarget = 58,

    ModFxOnOff = 59,
    ModFxTime = 66,
    ModFxDepth = 67,

    DelayOnOff = 68,
    DelayTime = 70,
    DelayDepth = 71,

    ReverbOnOff = 72,
    ReverbTime = 74,
    ReverbDepth = 75,

    /// <summary>
    /// In Firmware V1, Pitch Bend was
    /// apparently assigned to 124, and
    /// that's in use in two built-in
    /// presets:
    /// 093 Flat Lead and 189 TPL WahClav
    /// 
    /// In Firmware Version 2.x, it shows
    /// up as empty on the Minilogue XD
    /// menu and the Pitch Bend motion
    /// was assigned to 126 instead.
    /// 
    /// Unknown as to why.
    /// </summary>
    PitchBendV1 = 124,

    PitchBend = 126,
    GateTime = 129
}