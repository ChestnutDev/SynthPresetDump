using System.ComponentModel.DataAnnotations;
using System.Text;

namespace mnlxdprogdump;

/// <summary>
/// Adapted from the Minilogue XD MIDI Implementation Guide: https://www.korg.com/us/support/download/manual/0/811/4440/
/// Revision 1.01 (2020.2.10)
/// </summary>
public record ProgramData
{
    /// <summary>
    /// Must be PROG
    /// </summary>
    [Offset(0)]
    [StringLength(4)]
    [RegularExpression("^PROG$")]
    public string? Header;

    /// <summary>
    /// Up to 12 ASCII Characters for the Program Name
    /// </summary>
    [Offset(4)]
    [StringLength(12)]
    [RegularExpression("^[ !#$%&'()*,.:?+\\/\\-0-9A-Za-z]{0,12}$")]
    public string? ProgramName;

    /// <summary>
    /// Octave: -2 to +2
    /// </summary>
    [Offset(16)]
    [Range(0, 4)]
    public byte Octave;

    [Offset(17)]
    [Range(0, 127)]
    public byte Portamento;

    /// <summary>
    /// Key Trig On or Off
    /// </summary>
    /// <remarks>
    /// Added in Firmware 2.0
    /// 
    /// Press SHIFT+PLAY to turn on/off
    /// While holding a key, the sequence
    /// will play, but transposed to the
    /// key that is being held.
    /// </remarks>
    [Offset(18)]
    public bool KeyTrig;

    [Offset(19)]
    [Range(0, 1023)]
    public ushort VoiceModeDepth;

    [Offset(21)]
    public VoiceModeType VoiceModeType;

    [Offset(22)]
    public VcoWave Vco1Wave;

    /// <summary>
    /// 0~3=16',8',4',2'
    /// </summary>
    [Offset(23)]
    [Range(0, 3)]
    public byte Vco1Octave;

    [Offset(24)]
    [Range(0, 1023)]
    public ushort Vco1Pitch;

    [Offset(26)]
    [Range(0, 1023)]
    public ushort Vco1Shape;

    [Offset(28)]
    public VcoWave Vco2Wave;

    /// <summary>
    /// 0~3=16',8',4',2'
    /// </summary>
    [Offset(29)]
    [Range(0, 3)]
    public byte Vco2Octave;

    [Offset(30)]
    [Range(0, 1023)]
    public ushort Vco2Pitch;

    [Offset(32)]
    [Range(0, 1023)]
    public ushort Vco2Shape;

    [Offset(34)]
    public bool OscillatorSync;

    [Offset(35)]
    public bool RingMod;

    [Offset(36)]
    [Range(0, 1023)]
    public ushort CrossModDepth;

    [Offset(38)]
    public MultiOscType MultiOscType;

    [Offset(39)]
    public MultiOscNoise SelectedMultiOscNoise;

    [Offset(40)]
    public MultiOscVPM SelectedMultiOscVPM;

    [Offset(41)]
    [Range(0, 15)]
    public byte SelectedMultiOscUser;

    [Offset(42)]
    [Range(0, 1023)]
    public ushort ShapeNoise;

    [Offset(44)]
    [Range(0, 1023)]
    public ushort ShapeVPM;

    [Offset(46)]
    [Range(0, 1023)]
    public ushort ShapeUser;

    [Offset(48)]
    [Range(0, 1023)]
    public ushort ShiftShapeNoise;

    [Offset(50)]
    [Range(0, 1023)]
    public ushort ShiftShapeVPM;

    [Offset(52)]
    [Range(0, 1023)]
    public ushort ShiftShapeUser;

    [Offset(54)]
    [Range(0, 1023)]
    public ushort Vco1Level;

    [Offset(56)]
    [Range(0, 1023)]
    public ushort Vco2Level;

    [Offset(58)]
    [Range(0, 1023)]
    public ushort MultiLevel;

    [Offset(60)]
    [Range(0, 1023)]
    public ushort FilterCutoff;

    [Offset(62)]
    [Range(0, 1023)]
    public ushort FilterResonance;

    [Offset(64)]
    [Range(0, 2)]
    public byte FilterCutoffDrive;

    [Offset(65)]
    [Range(0, 2)]
    public byte FilterCutoffKeyboardTrack;

    [Offset(66)]
    [Range(0, 1023)]
    public ushort AmpEGAttack;

    [Offset(68)]
    [Range(0, 1023)]
    public ushort AmpEGDecay;

    [Offset(70)]
    [Range(0, 1023)]
    public ushort AmpEGSustain;

    [Offset(72)]
    [Range(0, 1023)]
    public ushort AmpEGRelease;

    [Offset(74)]
    [Range(0, 1023)]
    public ushort EGAttack;

    [Offset(76)]
    [Range(0, 1023)]
    public ushort EGDecay;

    [Offset(78)]
    [Range(0, 1023)]
    public ushort EGInt;

    [Offset(80)]
    public EGTarget EGTarget;

    [Offset(81)]
    public VcoWave LFOWave;

    [Offset(82)]
    public LFOMode LFOMode;

    [Offset(83)]
    [Range(0, 1023)]
    public ushort LFORate;

    [Offset(85)]
    [Range(0, 1023)]
    public ushort LFOInt;

    [Offset(87)]
    public LFOTarget LFOTarget;

    [Offset(88)]
    public bool ModFxOnOff;

    [Offset(89)]
    public ModFxType ModFxType;

    [Offset(90)]
    public ModFxChorus ModFxChorus;

    [Offset(91)]
    public ModFxEnsemble ModFxEnsemble;

    [Offset(92)]
    public ModFxPhaser ModFxPhaser;

    [Offset(93)]
    public ModFxFlanger ModFxFlanger;

    [Offset(94)]
    [Range(0, 15)]
    public byte ModFxUser;

    [Offset(95)]
    [Range(0, 1023)]
    public ushort ModFxTime;

    [Offset(97)]
    [Range(0, 1023)]
    public ushort ModFxDepth;

    [Offset(99)]
    public bool DelayOnOff;

    [Offset(100)]
    public DelaySubType DelaySubType;

    [Offset(101)]
    [Range(0, 1023)]
    public ushort DelayTime;

    [Offset(103)]
    [Range(0, 1023)]
    public ushort DelayDepth;

    [Offset(105)]
    public bool ReverbOnOff;

    [Offset(106)]
    public ReverbSubType ReverbSubType;

    [Offset(107)]
    [Range(0, 1023)]
    public ushort ReverbTime;

    [Offset(109)]
    [Range(0, 1023)]
    public ushort ReverbDepth;

    /// <summary>
    /// OFF~+12Note
    /// </summary>
    [Offset(111)]
    [Range(0, 12)]
    public byte BendRangePlus;

    /// <summary>
    /// OFF~-12Note
    /// </summary>
    [Offset(112)]
    [Range(0, 12)]
    public byte BendRangeMinus;

    [Offset(113)]
    public AssignTarget JoystickAssignPlus;

    /// <summary>
    /// 0~200=-100%~+100%
    /// </summary>
    [Offset(114)]
    [Range(0, 200)]
    public byte JoystickRangePlus;

    [Offset(115)]
    public AssignTarget JoystickAssignMinus;

    /// <summary>
    /// 0~200=-100%~+100%
    /// </summary>
    [Offset(116)]
    [Range(0, 200)]
    public byte JoystickRangeMinus;

    [Offset(117)]
    public CVInMode CVInMode;

    [Offset(118)]
    public AssignTarget CvIn1Assign;

    /// <summary>
    /// 0~200=-100%~+100%
    /// </summary>
    [Offset(119)]
    [Range(0, 200)]
    public byte CvIn1Range;

    [Offset(120)]
    public AssignTarget CvIn2Assign;

    /// <summary>
    /// 0~200=-100%~+100%
    /// </summary>
    [Offset(121)]
    [Range(0, 200)]
    public byte CvIn2Range;

    [Offset(122)]
    public MicroTuning MicroTuning;

    /// <summary>
    /// 0~24=-12Note~+12Note
    /// </summary>
    [Offset(123)]
    [Range(0, 24)]
    public byte ScaleKey;

    /// <summary>
    /// 0~100=-50Cent~+50Cent
    /// </summary>
    [Offset(124)]
    [Range(0, 100)]
    public byte ProgramTuning;

    [Offset(125)]
    public bool LFOKeySync;

    [Offset(126)]
    public bool LFOVoiceSync;

    [Offset(127)]
    public LFOTargetOsc LFOTargetOsc;

    [Offset(128)]
    [Range(0, 127)]
    public byte CutoffVelocity;

    [Offset(129)]
    [Range(0, 127)]
    public byte AmpVelocity;

    /// <summary>
    /// 0~3=16',8',4',2'
    /// </summary>
    [Offset(130)]
    [Range(0, 3)]
    public byte MultiOctave;

    [Offset(131)]
    public MultiRouting MultiRouting;

    [Offset(132)]
    public bool EGLegato;

    [Offset(133)]
    public PortamentoMode PortamentoMode;

    [Offset(134)]
    public bool PortamentoBPMSync;

    /// <summary>
    /// 12~132=-18dB~+6dB
    /// </summary>
    [Offset(135)]
    [Range(12, 132)]
    public byte ProgramLevel;

    /// <summary>
    /// 0~200=-100%~+100%
    /// </summary>
    [Offset(136)]
    [Range(0, 200)]
    public byte VPMParameter1Feedback;

    /// <summary>
    /// 0~200=-100%~+100%
    /// </summary>
    [Offset(137)]
    [Range(0, 200)]
    public byte VPMParameter2NoiseDepth;

    /// <summary>
    /// 0~200=-100%~+100%
    /// </summary>
    [Offset(138)]
    [Range(0, 200)]
    public byte VPMParameter3ShapeModInt;

    /// <summary>
    /// 0~200=-100%~+100%
    /// </summary>
    [Offset(139)]
    [Range(0, 200)]
    public byte VPMParameter4ModAttack;

    /// <summary>
    /// 0~200=-100%~+100%
    /// </summary>
    [Offset(140)]
    [Range(0, 200)]
    public byte VPMParameter5ModDecay;

    /// <summary>
    /// 0~200=-100%~+100%
    /// </summary>
    [Offset(141)]
    [Range(0, 200)]
    public byte VPMParameter6ModKeyTrack;

    [Offset(142)]
    public byte UserParam1;

    [Offset(143)]
    public byte UserParam2;

    [Offset(144)]
    public byte UserParam3;

    [Offset(145)]
    public byte UserParam4;

    [Offset(146)]
    public byte UserParam5;

    [Offset(147)]
    public byte UserParam6;

    [Offset(148)]
    public byte UserParam56Type;

    [Offset(149)]
    public byte UserParam1234Type;

    [Offset(149, 0, 1)]
    public UserParamType UserParam1Type;

    [Offset(149, 2, 3)]
    public UserParamType UserParam2Type;

    [Offset(149, 4, 5)]
    public UserParamType UserParam3Type;

    [Offset(149, 6, 7)]
    public UserParamType UserParam4Type;

    [Offset(148, 0, 1)]
    public UserParamType UserParam5Type;

    [Offset(148, 2, 3)]
    public UserParamType UserParam6Type;

    /// <summary>
    /// -12~+12 Note
    /// </summary>
    [Offset(150)]
    [Range(1, 25)]
    public byte ProgramTranspose;

    [Offset(151)]
    [Range(0, 1024)]
    public ushort DelayDryWet;

    [Offset(153)]
    [Range(0, 1024)]
    public ushort ReverbDryWet;

    [Offset(155)]
    public AssignTarget MidiAfterTouchAssign;

    /// <summary>
    /// PRED = PRogram EnD marker
    /// </summary>
    [Offset(156)]
    [StringLength(4)]
    [RegularExpression("^PRED$")]
    public string? ProgramEndMarker;
}



