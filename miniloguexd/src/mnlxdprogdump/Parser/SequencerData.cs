using System.ComponentModel.DataAnnotations;
using System.Text;

namespace mnlxdprogdump;

/// <summary>
/// Sequencer Data for Firmware Version 1.xx
/// </summary>
public record SequencerDataV1
{
    public static bool IsSequencerV1Data(ReadOnlySpan<byte> input)
        => input.Length > 164 && Encoding.ASCII.GetString(input.Slice(160, 4)) == "SEQD";

    /// <summary>
    /// SEQD
    /// </summary>
    [Offset(160)]
    [StringLength(4)]
    [RegularExpression("^SEQD$", ErrorMessage = "Version 1 Sequencer Data must start with SEQD.")]
    public string? SEQD;

    [Offset(164)]
    public SequencerData? SequencerData;
}

/// <summary>
/// Sequencer Data for Firmware Version 2
/// </summary>
/// <remarks>
/// Firmware 2.00 added an "Active Step" Feature to the sequencer.
/// The Sequencer Data Header was changed from "SEQD" to "SQ", which
/// allows to differentiate. The two bytes that are now free are used
/// to mark the active steps.
/// 
/// Active steps are steps that can be turned on/off while the sequence
/// is playing. Unlike regular step on/off which are meant to not be part
/// of the sequence, ever.
/// 
/// From Korg's Midi Implementation Guide: 
/// If data that was saved with firmware Ver1.xx is loaded, the
/// old 'SEQD' 4 byte header is automatically replaced with the
/// new 'SQ' 2 byte header and 2 byte Active Step data(all on).
/// 
/// V2 also added arpeggiator speed and gate time and
/// key trigger transpose, the latter lives on
/// <see cref="ProgramData.KeyTrig"/>.
/// </remarks>
public record SequencerDataV2
{
    public static bool IsSequencerV2Data(ReadOnlySpan<byte> input)
        => input.Length > 162 && Encoding.ASCII.GetString(input.Slice(160, 2)) == "SQ";

    public static SequencerDataV2 ConvertFromV1(SequencerDataV1 seqV1) => new()
    {
        SQ = "SQ",
        SequencerData = seqV1.SequencerData,
        Step1ActiveStepOnOff = true,
        Step2ActiveStepOnOff = true,
        Step3ActiveStepOnOff = true,
        Step4ActiveStepOnOff = true,
        Step5ActiveStepOnOff = true,
        Step6ActiveStepOnOff = true,
        Step7ActiveStepOnOff = true,
        Step8ActiveStepOnOff = true,
        Step9ActiveStepOnOff = true,
        Step10ActiveStepOnOff = true,
        Step11ActiveStepOnOff = true,
        Step12ActiveStepOnOff = true,
        Step13ActiveStepOnOff = true,
        Step14ActiveStepOnOff = true,
        Step15ActiveStepOnOff = true,
        Step16ActiveStepOnOff = true,
        ARPRate = ARPRate.Sixteen,
        ARPGateTime = 55 // 75%
    };

    /// <summary>
    /// SQ
    /// </summary>
    [Offset(160)]
    [StringLength(2)]
    [RegularExpression("^SQ$", ErrorMessage = "Version 2 Sequencer Data must start with SQ.")]
    public string? SQ;

    [Offset(162, 0)]
    public bool Step1ActiveStepOnOff;
    [Offset(162, 1)]
    public bool Step2ActiveStepOnOff;
    [Offset(162, 2)]
    public bool Step3ActiveStepOnOff;
    [Offset(162, 3)]
    public bool Step4ActiveStepOnOff;
    [Offset(162, 4)]
    public bool Step5ActiveStepOnOff;
    [Offset(162, 5)]
    public bool Step6ActiveStepOnOff;
    [Offset(162, 6)]
    public bool Step7ActiveStepOnOff;
    [Offset(162, 7)]
    public bool Step8ActiveStepOnOff;
    [Offset(163, 0)]
    public bool Step9ActiveStepOnOff;
    [Offset(163, 1)]
    public bool Step10ActiveStepOnOff;
    [Offset(163, 2)]
    public bool Step11ActiveStepOnOff;
    [Offset(163, 3)]
    public bool Step12ActiveStepOnOff;
    [Offset(163, 4)]
    public bool Step13ActiveStepOnOff;
    [Offset(163, 5)]
    public bool Step14ActiveStepOnOff;
    [Offset(163, 6)]
    public bool Step15ActiveStepOnOff;
    [Offset(163, 7)]
    public bool Step16ActiveStepOnOff;

    [Offset(164)]
    public SequencerData? SequencerData;

    /// <summary>
    /// 0~72 = 0%~100%
    /// 
    /// Actual testing shows
    /// that the value is 1-73,
    /// not 0-72.
    /// </summary>
    [Offset(1022)]
    [Range(1, 73)]
    public byte ARPGateTime;

    [Offset(1023)]
    public ARPRate ARPRate;
}

public record SequencerData
{
    /// <summary>
    /// 100~3000=10.0~300.0
    /// </summary>
    [Offset(164)]
    [Range(100, 3000)]
    public ushort BPM;

    [Offset(166)]
    [Range(1, 16)]
    public byte StepLength;

    /// <summary>
    /// 0~4 = 1/16,1/8,1/4,1/2,1/1
    /// </summary>
    [Offset(167)]
    [Range(0, 4)]
    public byte StepResolution;

    /// <summary>
    /// The manual says -75~+75,
    /// but it's not actually stored
    /// as a signed byte
    /// </summary>
    [Offset(168)]
    [Range(0, 150)]
    public byte Swing;

    /// <summary>
    /// 0~72=0%~100%
    /// </summary>
    [Offset(169)]
    [Range(0, 72)]
    public byte DefaultGateTime;

    [Offset(170, 0)]
    public bool Step1StepOnOff;
    [Offset(170, 1)]
    public bool Step2StepOnOff;
    [Offset(170, 2)]
    public bool Step3StepOnOff;
    [Offset(170, 3)]
    public bool Step4StepOnOff;
    [Offset(170, 4)]
    public bool Step5StepOnOff;
    [Offset(170, 5)]
    public bool Step6StepOnOff;
    [Offset(170, 6)]
    public bool Step7StepOnOff;
    [Offset(170, 7)]
    public bool Step8StepOnOff;
    [Offset(171, 0)]
    public bool Step9StepOnOff;
    [Offset(171, 1)]
    public bool Step10StepOnOff;
    [Offset(171, 2)]
    public bool Step11StepOnOff;
    [Offset(171, 3)]
    public bool Step12StepOnOff;
    [Offset(171, 4)]
    public bool Step13StepOnOff;
    [Offset(171, 5)]
    public bool Step14StepOnOff;
    [Offset(171, 6)]
    public bool Step15StepOnOff;
    [Offset(171, 7)]
    public bool Step16StepOnOff;

    [Offset(172, 0)]
    public bool Step1MotionOnOff;
    [Offset(172, 1)]
    public bool Step2MotionOnOff;
    [Offset(172, 2)]
    public bool Step3MotionOnOff;
    [Offset(172, 3)]
    public bool Step4MotionOnOff;
    [Offset(172, 4)]
    public bool Step5MotionOnOff;
    [Offset(172, 5)]
    public bool Step6MotionOnOff;
    [Offset(172, 6)]
    public bool Step7MotionOnOff;
    [Offset(172, 7)]
    public bool Step8MotionOnOff;
    [Offset(173, 0)]
    public bool Step9MotionOnOff;
    [Offset(173, 1)]
    public bool Step10MotionOnOff;
    [Offset(173, 2)]
    public bool Step11MotionOnOff;
    [Offset(173, 3)]
    public bool Step12MotionOnOff;
    [Offset(173, 4)]
    public bool Step13MotionOnOff;
    [Offset(173, 5)]
    public bool Step14MotionOnOff;
    [Offset(173, 6)]
    public bool Step15MotionOnOff;
    [Offset(173, 7)]
    public bool Step16MotionOnOff;

    [Offset(174, 0)]
    public bool MotionSlot1Parameter_MotionOnOff;

    [Offset(174, 1)]
    public bool MotionSlot1Parameter_SmoothOnOff;

    [Offset(175)]
    public MotionParameterId MotionSlot1ParameterId;

    [Offset(176, 0)]
    public bool MotionSlot2Parameter_MotionOnOff;

    [Offset(176, 1)]
    public bool MotionSlot2Parameter_SmoothOnOff;

    [Offset(177)]
    public MotionParameterId MotionSlot2ParameterId;

    [Offset(178, 0)]
    public bool MotionSlot3Parameter_MotionOnOff;

    [Offset(178, 1)]
    public bool MotionSlot3Parameter_SmoothOnOff;

    [Offset(179)]
    public MotionParameterId MotionSlot3ParameterId;

    [Offset(180, 0)]
    public bool MotionSlot4Parameter_MotionOnOff;

    [Offset(180, 1)]
    public bool MotionSlot4Parameter_SmoothOnOff;

    [Offset(181)]
    public MotionParameterId MotionSlot4ParameterId;

    [Offset(182, 0)]
    public bool MotionSlot1Step1OnOff;
    [Offset(182, 1)]
    public bool MotionSlot1Step2OnOff;
    [Offset(182, 2)]
    public bool MotionSlot1Step3OnOff;
    [Offset(182, 3)]
    public bool MotionSlot1Step4OnOff;
    [Offset(182, 4)]
    public bool MotionSlot1Step5OnOff;
    [Offset(182, 5)]
    public bool MotionSlot1Step6OnOff;
    [Offset(182, 6)]
    public bool MotionSlot1Step7OnOff;
    [Offset(182, 7)]
    public bool MotionSlot1Step8OnOff;
    [Offset(183, 0)]
    public bool MotionSlot1Step9OnOff;
    [Offset(183, 1)]
    public bool MotionSlot1Step10OnOff;
    [Offset(183, 2)]
    public bool MotionSlot1Step11OnOff;
    [Offset(183, 3)]
    public bool MotionSlot1Step12OnOff;
    [Offset(183, 4)]
    public bool MotionSlot1Step13OnOff;
    [Offset(183, 5)]
    public bool MotionSlot1Step14OnOff;
    [Offset(183, 6)]
    public bool MotionSlot1Step15OnOff;
    [Offset(183, 7)]
    public bool MotionSlot1Step16OnOff;

    [Offset(184, 0)]
    public bool MotionSlot2Step1OnOff;
    [Offset(184, 1)]
    public bool MotionSlot2Step2OnOff;
    [Offset(184, 2)]
    public bool MotionSlot2Step3OnOff;
    [Offset(184, 3)]
    public bool MotionSlot2Step4OnOff;
    [Offset(184, 4)]
    public bool MotionSlot2Step5OnOff;
    [Offset(184, 5)]
    public bool MotionSlot2Step6OnOff;
    [Offset(184, 6)]
    public bool MotionSlot2Step7OnOff;
    [Offset(184, 7)]
    public bool MotionSlot2Step8OnOff;
    [Offset(185, 0)]
    public bool MotionSlot2Step9OnOff;
    [Offset(185, 1)]
    public bool MotionSlot2Step10OnOff;
    [Offset(185, 2)]
    public bool MotionSlot2Step11OnOff;
    [Offset(185, 3)]
    public bool MotionSlot2Step12OnOff;
    [Offset(185, 4)]
    public bool MotionSlot2Step13OnOff;
    [Offset(185, 5)]
    public bool MotionSlot2Step14OnOff;
    [Offset(185, 6)]
    public bool MotionSlot2Step15OnOff;
    [Offset(185, 7)]
    public bool MotionSlot2Step16OnOff;

    [Offset(186, 0)]
    public bool MotionSlot3Step1OnOff;
    [Offset(186, 1)]
    public bool MotionSlot3Step2OnOff;
    [Offset(186, 2)]
    public bool MotionSlot3Step3OnOff;
    [Offset(186, 3)]
    public bool MotionSlot3Step4OnOff;
    [Offset(186, 4)]
    public bool MotionSlot3Step5OnOff;
    [Offset(186, 5)]
    public bool MotionSlot3Step6OnOff;
    [Offset(186, 6)]
    public bool MotionSlot3Step7OnOff;
    [Offset(186, 7)]
    public bool MotionSlot3Step8OnOff;
    [Offset(187, 0)]
    public bool MotionSlot3Step9OnOff;
    [Offset(187, 1)]
    public bool MotionSlot3Step10OnOff;
    [Offset(187, 2)]
    public bool MotionSlot3Step11OnOff;
    [Offset(187, 3)]
    public bool MotionSlot3Step12OnOff;
    [Offset(187, 4)]
    public bool MotionSlot3Step13OnOff;
    [Offset(187, 5)]
    public bool MotionSlot3Step14OnOff;
    [Offset(187, 6)]
    public bool MotionSlot3Step15OnOff;
    [Offset(187, 7)]
    public bool MotionSlot3Step16OnOff;

    [Offset(188, 0)]
    public bool MotionSlot4Step1OnOff;
    [Offset(188, 1)]
    public bool MotionSlot4Step2OnOff;
    [Offset(188, 2)]
    public bool MotionSlot4Step3OnOff;
    [Offset(188, 3)]
    public bool MotionSlot4Step4OnOff;
    [Offset(188, 4)]
    public bool MotionSlot4Step5OnOff;
    [Offset(188, 5)]
    public bool MotionSlot4Step6OnOff;
    [Offset(188, 6)]
    public bool MotionSlot4Step7OnOff;
    [Offset(188, 7)]
    public bool MotionSlot4Step8OnOff;
    [Offset(189, 0)]
    public bool MotionSlot4Step9OnOff;
    [Offset(189, 1)]
    public bool MotionSlot4Step10OnOff;
    [Offset(189, 2)]
    public bool MotionSlot4Step11OnOff;
    [Offset(189, 3)]
    public bool MotionSlot4Step12OnOff;
    [Offset(189, 4)]
    public bool MotionSlot4Step13OnOff;
    [Offset(189, 5)]
    public bool MotionSlot4Step14OnOff;
    [Offset(189, 6)]
    public bool MotionSlot4Step15OnOff;
    [Offset(189, 7)]
    public bool MotionSlot4Step16OnOff;

    [Offset(190)]
    public StepEventData? Step1EventData;
    [Offset(242)]
    public StepEventData? Step2EventData;
    [Offset(294)]
    public StepEventData? Step3EventData;
    [Offset(346)]
    public StepEventData? Step4EventData;
    [Offset(398)]
    public StepEventData? Step5EventData;
    [Offset(450)]
    public StepEventData? Step6EventData;
    [Offset(502)]
    public StepEventData? Step7EventData;
    [Offset(554)]
    public StepEventData? Step8EventData;
    [Offset(606)]
    public StepEventData? Step9EventData;
    [Offset(658)]
    public StepEventData? Step10EventData;
    [Offset(710)]
    public StepEventData? Step11EventData;
    [Offset(762)]
    public StepEventData? Step12EventData;
    [Offset(814)]
    public StepEventData? Step13EventData;
    [Offset(866)]
    public StepEventData? Step14EventData;
    [Offset(918)]
    public StepEventData? Step15EventData;
    [Offset(970)]
    public StepEventData? Step16EventData;
}

/// <summary>
/// *note S3 (Step Event Data)
/// </summary>
public record StepEventData
{
    [Offset(0)]
    [Range(0, 127)]
    public byte NoteNo1;
    [Offset(1)]
    [Range(0, 127)]
    public byte NoteNo2;
    [Offset(2)]
    [Range(0, 127)]
    public byte NoteNo3;
    [Offset(3)]
    [Range(0, 127)]
    public byte NoteNo4;
    [Offset(4)]
    [Range(0, 127)]
    public byte NoteNo5;
    [Offset(5)]
    [Range(0, 127)]
    public byte NoteNo6;
    [Offset(6)]
    [Range(0, 127)]
    public byte NoteNo7;
    [Offset(7)]
    [Range(0, 127)]
    public byte NoteNo8;

    /// <summary>
    /// 0,1~127=NoEvent,Velocity1~127
    /// </summary>
    [Offset(8)]
    [Range(0, 127)]
    public byte VelocityNo1;
    /// <summary>
    /// 0,1~127=NoEvent,Velocity1~127
    /// </summary>
    [Offset(9)]
    [Range(0, 127)]
    public byte VelocityNo2;
    /// <summary>
    /// 0,1~127=NoEvent,Velocity1~127
    /// </summary>
    [Offset(10)]
    [Range(0, 127)]
    public byte VelocityNo3;
    /// <summary>
    /// 0,1~127=NoEvent,Velocity1~127
    /// </summary>
    [Offset(11)]
    [Range(0, 127)]
    public byte VelocityNo4;
    /// <summary>
    /// 0,1~127=NoEvent,Velocity1~127
    /// </summary>
    [Offset(12)]
    [Range(0, 127)]
    public byte VelocityNo5;
    /// <summary>
    /// 0,1~127=NoEvent,Velocity1~127
    /// </summary>
    [Offset(13)]
    [Range(0, 127)]
    public byte VelocityNo6;
    /// <summary>
    /// 0,1~127=NoEvent,Velocity1~127
    /// </summary>
    [Offset(14)]
    [Range(0, 127)]
    public byte VelocityNo7;
    /// <summary>
    /// 0,1~127=NoEvent,Velocity1~127
    /// </summary>
    [Offset(15)]
    [Range(0, 127)]
    public byte VelocityNo8;

    /// <summary>
    /// 0~72,73~127=0%~100%,TIE
    /// 
    /// *note S3-1 (Gate Time)
    /// If the gate time is set to TIE(127) and the Trigger Switch for the next
    /// step is set to 0, the sound will continue into the next step.
    /// </summary>
    [Offset(16, 0, 6)]
    [Range(0, 127)]
    public byte GateTime1;
    [Offset(16, 7)]
    public bool TriggerSwitch1;

    [Offset(17, 0, 6)]
    [Range(0, 127)]
    public byte GateTime2;
    [Offset(17, 7)]
    public bool TriggerSwitch2;

    [Offset(18, 0, 6)]
    [Range(0, 127)]
    public byte GateTime3;
    [Offset(18, 7)]
    public bool TriggerSwitch3;

    [Offset(19, 0, 6)]
    [Range(0, 127)]
    public byte GateTime4;
    [Offset(19, 7)]
    public bool TriggerSwitch4;

    [Offset(20, 0, 6)]
    [Range(0, 127)]
    public byte GateTime5;
    [Offset(20, 7)]
    public bool TriggerSwitch5;

    [Offset(21, 0, 6)]
    [Range(0, 127)]
    public byte GateTime6;
    [Offset(21, 7)]
    public bool TriggerSwitch6;

    [Offset(22, 0, 6)]
    [Range(0, 127)]
    public byte GateTime7;
    [Offset(22, 7)]
    public bool TriggerSwitch7;

    [Offset(23, 0, 6)]
    [Range(0, 127)]
    public byte GateTime8;
    [Offset(23, 7)]
    public bool TriggerSwitch8;

    [Offset(24)]
    public MotionData? MotionSlot1;
    [Offset(31)]
    public MotionData? MotionSlot2;
    [Offset(38)]
    public MotionData? MotionSlot3;
    [Offset(45)]
    public MotionData? MotionSlot4;
}

/// <summary>
/// *note S3-2 (Motion Data)
/// </summary>
/// <remarks>
/// When Smooth motion is On, 5 motion data points are used in the following way:
/// - Between the start of the step and 1/4 of the step, points 1 and 2 are interpolated.
/// - Between the 1/4 of the step and 2/4 of the step, points 2 and 3 are interpolated.
/// - Between the 2/4 of the step and 3/4 of the step, points 3 and 4 are interpolated.
/// - Between the 3/4 of the step and end of the step, points 4 and 5 are interpolated.
/// 
/// When Smooth motion is Off, or if the parameter is of switch type, only DATA 1
/// is used and no interpolation takes place.
/// </remarks>
public record MotionData
{
    [Offset(0)]
    public byte Data1Bits2_9;
    [Offset(1)]
    public byte Data2Bits2_9;
    [Offset(2)]
    public byte Data3Bits2_9;
    [Offset(3)]
    public byte Data4Bits2_9;
    [Offset(4)]
    public byte Data5Bits2_9;

    [Offset(5, 0, 1)]
    public byte Data1Bits0_1;
    [Offset(5, 2, 3)]
    public byte Data2Bits0_1;
    [Offset(5, 4, 5)]
    public byte Data3Bits0_1;
    [Offset(5, 6, 7)]
    public byte Data4Bits0_1;
    [Offset(6, 0, 1)]
    public byte Data5Bits0_1;

    public ushort Data1 => (ushort)((Data1Bits2_9 << 2) + Data1Bits0_1);
    public ushort Data2 => (ushort)((Data2Bits2_9 << 2) + Data2Bits0_1);
    public ushort Data3 => (ushort)((Data3Bits2_9 << 2) + Data3Bits0_1);
    public ushort Data4 => (ushort)((Data4Bits2_9 << 2) + Data4Bits0_1);
    public ushort Data5 => (ushort)((Data5Bits2_9 << 2) + Data5Bits0_1);
}
