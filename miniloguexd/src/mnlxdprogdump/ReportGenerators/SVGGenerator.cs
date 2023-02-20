using System.Text;
using System.Xml;

namespace mnlxdprogdump;

// There are a LOT of hardcoded numbers found through trial and error in here.
// Some TODOs:
// * Make the numbers more calculated/easy to adjust, now that we kinda have a working layout
// * Optimize the SVG (e.g., remove all <g> tags that don't have elements in them)
// * Store the XML as UTF-8 instead of UTF-16, but that requires the IReportGenerator to support a byte[]

public class SVGGenerator : IReportGenerator
{
    private readonly static string SvgNamespace = "http://www.w3.org/2000/svg";

    private const int padding = 10;

    private const int strokeWidth = 3;

    private const string backgroundColor = "white";
    private const string strokeColor = "black";
    private const string switchActiveColor = "red";

    private const int KnobDiameter = 70;
    private const int KnobRadius = KnobDiameter / 2;
    private const int LedDiameter = 20;
    private const int LedRadius = LedDiameter / 2;


    private const int synthWidth = 2300;
    private const int synthHeight = 970;
    private const int headerOffset = 35;
    private const int rowSpacing = 255;
    private const int firstRowY = 90;
    private const int secondRowY = firstRowY + rowSpacing;
    private const int thirdRowY = secondRowY + rowSpacing + 20;

    public string PreferredFileExtension => ".svg";

    public string GenerateReport(ReportGeneratorInput input)
    {
        var programData = input.Program;

        const int viewBoxWidth = synthWidth + padding + padding;
        const int viewBoxHeight = synthHeight + padding + padding;

        var xmlDoc = new XmlDocument();
        var svgElem = xmlDoc.CreateElement("svg", SvgNamespace);
        svgElem.SetAttribute("version", "1.1");
        svgElem.SetAttribute("width", "1900");
        svgElem.SetAttribute("preserveAspectRatio", "xMidYMid meet");
        svgElem.SetAttribute("viewBox", $"0 0 {viewBoxWidth} {viewBoxHeight}");

        var outerRect = xmlDoc.CreateElement("rect", SvgNamespace);
        outerRect.SetAttribute("x", padding.ToString());
        outerRect.SetAttribute("y", padding.ToString());
        outerRect.SetAttribute("width", synthWidth.ToString());
        outerRect.SetAttribute("height", synthHeight.ToString());
        outerRect.SetAttribute("stroke", strokeColor);
        outerRect.SetAttribute("stroke-width", padding.ToString());
        outerRect.SetAttribute("fill", backgroundColor);
        outerRect.SetAttribute("rx", "30");
        outerRect.SetAttribute("ry", "30");
        svgElem.AppendChild(outerRect);

        var programLabel = CreateLabel(xmlDoc, "Program: " + programData.ProgramName, 30, 30, TextSize.ProgramHeader, TextAnchor.start);
        programLabel.SetAttribute("font-weight", "bold");
        svgElem.AppendChild(programLabel);
        svgElem.AppendChild(CreateLogo(xmlDoc, 35, 890));

        // Voice Mode
        svgElem.AppendChild(CreateVoiceModeSection(xmlDoc, programData, 75));

        // VCO 1 & 2
        svgElem.AppendChild(CreateOscillatorSection(xmlDoc, programData, 400));

        // Multi Engine
        svgElem.AppendChild(CreateMultiEngineSection(xmlDoc, input, 400));

        // Mixer
        svgElem.AppendChild(CreateMixerSection(xmlDoc, programData, 1070));

        // Filter
        svgElem.AppendChild(CreateFilterSection(xmlDoc, programData, 1215));

        // Envelope Generator & LFO
        svgElem.AppendChild(CreateEnvelopeGeneratorSection(xmlDoc, programData, 1360));
        svgElem.AppendChild(CreateLFOSection(xmlDoc, programData, 1360));
        svgElem.AppendChild(CreateDividerLine(xmlDoc, 1850));

        // EFFECTS
        svgElem.AppendChild(CreateEffectsSection(xmlDoc, input, 1860));

        // Misc. Other stuff that didn't fit elsewhere
        svgElem.AppendChild(CreateMiscSection(xmlDoc, programData, 1860));

        xmlDoc.AppendChild(svgElem);

        var outSb = new StringBuilder();


        using (var tw = XmlWriter.Create(outSb, new XmlWriterSettings
        {
            Indent = true
        }))
        {
            xmlDoc.Save(tw);
        }
        return outSb.ToString();
    }

    private static XmlElement CreateMiscSection(XmlDocument xmlDoc, ProgramData program, int x)
    {
        var g = xmlDoc.CreateElement("g", SvgNamespace);

        var sb = new StringBuilder();
        sb.AppendLine("**Program Edit / CV Input");
        sb.AppendLine($"CV IN Mode: {program.CVInMode}");
        sb.AppendLine($"CV IN 1 Assign: {program.CvIn1Assign} | Range: {DisplayHelper.MinusToPlus100String(program.CvIn1Range)}");
        sb.AppendLine($"CV IN 2 Assign: {program.CvIn2Assign} | Range: {DisplayHelper.MinusToPlus100String(program.CvIn2Range)}");
        sb.AppendLine($"Modulation / MIDI Aftertouch: {program.MidiAfterTouchAssign}");
        // TODO: This is a sequencer feature
        sb.AppendLine($"Key Trigger Transpose (SHIFT+PLAY): {(program.KeyTrig ? "On" : "Off")}");

        g.AppendChild(CreateLabel(xmlDoc, sb.ToString(), x, synthHeight - 130, TextSize.Normal, TextAnchor.start));
        return g;
    }

    private static XmlElement CreateEffectsSection(XmlDocument xmlDoc, ReportGeneratorInput input, int x)
    {
        var g = xmlDoc.CreateElement("g", SvgNamespace);
        g.AppendChild(CreateLabel(xmlDoc, "**EFFECTS", x + 120, firstRowY - headerOffset, TextSize.SectionHeader, TextAnchor.start));
        g.AppendChild(CreateModFxSection(xmlDoc, input, x, firstRowY));
        g.AppendChild(CreateReverbFxSection(xmlDoc, input, x, secondRowY));
        g.AppendChild(CreateDelayFxSection(xmlDoc, input, x, thirdRowY));
        return g;
    }

    private static XmlElement CreateModFxSection(XmlDocument xmlDoc, ReportGeneratorInput input, int x, int y)
    {
        var program = input.Program;
        var g = xmlDoc.CreateElement("g", SvgNamespace);

        var subtype = program.ModFxType switch
        {
            ModFxType.Chorus => program.ModFxChorus.ToString(),
            ModFxType.Ensemble => program.ModFxEnsemble.ToString(),
            ModFxType.Flanger => program.ModFxFlanger.ToString(),
            ModFxType.Phaser => program.ModFxPhaser.ToString(),
            ModFxType.User => input.GetUserModFxName(),
            _ => ""
        };

        var typeString = program.ModFxType == ModFxType.User ? subtype : $"{program.ModFxType} / {subtype}";

        g.AppendChild(CreateLabel(xmlDoc, $"MOD FX ({(program.ModFxOnOff ? "On" : "Off")}): {typeString}", x, y, TextSize.SectionHeader, TextAnchor.start));
        g.AppendChild(CreateKnob(xmlDoc, $"MOD FX\nTIME\nRaw: {program.ModFxTime}", x + 20, y + 40, PercentFromValue(program.ModFxTime, 0, 1023)));
        g.AppendChild(CreateKnob(xmlDoc, $"MOD FX\nDEPTH\nRaw: {program.ModFxDepth}", x + 140, y + 40, PercentFromValue(program.ModFxDepth, 0, 1023)));
        return g;
    }

    private static XmlElement CreateReverbFxSection(XmlDocument xmlDoc, ReportGeneratorInput input, int x, int y)
    {
        var program = input.Program;
        var g = xmlDoc.CreateElement("g", SvgNamespace);

        g.AppendChild(CreateLabel(xmlDoc, $"REVERB FX ({(program.ReverbOnOff ? "On" : "Off")}): {input.GetReverbFxName()}", x, y, TextSize.SectionHeader, TextAnchor.start));
        g.AppendChild(CreateKnob(xmlDoc, $"REVERB FX\nTIME\nRaw: {program.ReverbTime}", x + 20, y + 40, PercentFromValue(program.ReverbTime, 0, 1023)));
        g.AppendChild(CreateKnob(xmlDoc, $"REVERB FX\nDEPTH\nRaw: {program.ReverbDepth}", x + 140, y + 40, PercentFromValue(program.ReverbDepth, 0, 1023)));

        // Wet 0.00% to BALANCED to Dry 0.00%
        g.AppendChild(CreateKnob(xmlDoc, $"REVERB FX\nDRY/WET\nRaw: {program.ReverbDryWet}", x + 260, y + 40, PercentFromValue(program.ReverbDryWet, 0, 1024)));

        return g;
    }

    private static XmlElement CreateDelayFxSection(XmlDocument xmlDoc, ReportGeneratorInput input, int x, int y)
    {
        var program = input.Program;
        var g = xmlDoc.CreateElement("g", SvgNamespace);

        g.AppendChild(CreateLabel(xmlDoc, $"DELAY FX ({(program.DelayOnOff ? "On" : "Off")}): {input.GetDelayFxName()}", x, y, TextSize.SectionHeader, TextAnchor.start));
        g.AppendChild(CreateKnob(xmlDoc, $"DELAY FX\nTIME\nRaw: {program.DelayTime}", x + 20, y + 40, PercentFromValue(program.DelayTime, 0, 1023)));
        g.AppendChild(CreateKnob(xmlDoc, $"DELAY FX\nDEPTH\nRaw: {program.DelayDepth}", x + 140, y + 40, PercentFromValue(program.DelayDepth, 0, 1023)));

        // Wet 0.00% to BALANCED to Dry 0.00%
        g.AppendChild(CreateKnob(xmlDoc, $"DELAY FX\nDRY/WET\nRaw: {program.DelayDryWet}", x + 260, y + 40, PercentFromValue(program.DelayDryWet, 0, 1024)));

        return g;
    }

    private static XmlElement CreateLFOSection(XmlDocument xmlDoc, ProgramData program, int x)
    {
        var g = xmlDoc.CreateElement("g", SvgNamespace);

        g.AppendChild(CreateLabel(xmlDoc, "**LFO", x + 220, thirdRowY - headerOffset, TextSize.SectionHeader));

        // Wave, Mode, Rate, Int, Target

        g.AppendChild(CreateVSwitch(xmlDoc, x, thirdRowY, "WAVE", (int)program.LFOWave, "SQR", "TRI", "SAW"));
        g.AppendChild(CreateVSwitch(xmlDoc, x + 90, thirdRowY, "MODE", (int)program.LFOMode, "1-Shot", "Normal", "BPM"));

        var lfoRateString = DisplayHelper.LFORate(program.LFORate, program.LFOMode);
        g.AppendChild(CreateKnob(xmlDoc, $"RATE\n{lfoRateString}\nRaw: {program.LFORate}", x + 200, thirdRowY, PercentFromValue(program.LFORate, 0, 1023)));

        g.AppendChild(CreateKnob(xmlDoc, $"INT\n{program.LFOInt - 512}\nRaw: {program.LFOInt}", x + 300, thirdRowY, PercentFromValue(program.LFOInt, 0, 1023)));

        g.AppendChild(CreateVSwitch(xmlDoc, x + 400, thirdRowY, "TARGET", (int)program.LFOTarget, "Cutoff", "Shape", "Pitch"));


        g.AppendChild(CreateLabel(xmlDoc, $"**Program Edit / LFO\nLFO Target Osc: {(program.LFOTargetOsc)}\nLFO Key Sync: {(program.LFOKeySync ? "On" : "Off")}\nLFO Voice Sync: {(program.LFOVoiceSync ? "On" : "Off")}", x, thirdRowY + KnobDiameter + 100, TextSize.Normal, TextAnchor.start));

        return g;
    }

    private static XmlElement CreateEnvelopeGeneratorSection(XmlDocument xmlDoc, ProgramData program, int x)
    {

        var g = xmlDoc.CreateElement("g", SvgNamespace);
        g.AppendChild(CreateLabel(xmlDoc, "**AMP EG", x + 220, firstRowY - headerOffset, TextSize.SectionHeader));

        var spacing = 120;

        g.AppendChild(CreateKnob(xmlDoc, $"ATTACK\nRaw: {program.AmpEGAttack}", x, firstRowY, PercentFromValue(program.AmpEGAttack, 0, 1023)));
        g.AppendChild(CreateKnob(xmlDoc, $"DECAY\nRaw: {program.AmpEGDecay}", x + spacing, firstRowY, PercentFromValue(program.AmpEGDecay, 0, 1023)));
        g.AppendChild(CreateKnob(xmlDoc, $"SUSTAIN\nRaw: {program.AmpEGSustain}", x + (spacing * 2), firstRowY, PercentFromValue(program.AmpEGSustain, 0, 1023)));
        g.AppendChild(CreateKnob(xmlDoc, $"RELEASE\nRaw: {program.AmpEGRelease}", x + (spacing * 3), firstRowY, PercentFromValue(program.AmpEGRelease, 0, 1023)));

        g.AppendChild(CreateLabel(xmlDoc, $"Program Edit / Modulation / Amp Velocity: {program.AmpVelocity}", x, firstRowY + KnobDiameter + 110, TextSize.Normal, TextAnchor.start));


        g.AppendChild(CreateLabel(xmlDoc, "**EG", x + 220, secondRowY - headerOffset, TextSize.SectionHeader));
        var egIntPercent = Math.Round(DisplayHelper.EGIntPercent(program.EGInt), 5);

        g.AppendChild(CreateKnob(xmlDoc, $"ATTACK\nRaw: {program.EGAttack}", x, secondRowY, PercentFromValue(program.EGAttack, 0, 1023)));
        g.AppendChild(CreateKnob(xmlDoc, $"DECAY\nRaw: {program.EGDecay}", x + spacing, secondRowY, PercentFromValue(program.EGDecay, 0, 1023)));
        g.AppendChild(CreateKnob(xmlDoc, $"EG INT\n{egIntPercent}%\nRaw: {program.EGInt}", x + (spacing * 2), secondRowY, PercentFromValue(program.EGInt, 0, 1023)));

        g.AppendChild(CreateVSwitch(xmlDoc, x + (spacing * 3), secondRowY, "TARGET", (int)program.EGTarget, "Cutoff", "Pitch 2", "Pitch"));

        g.AppendChild(CreateLabel(xmlDoc, $"Program Edit / Modulation / EG Cutoff Velocity: {program.CutoffVelocity}\nProgram Edit / Other / EG Legato: {(program.EGLegato ? "On" : "Off")}", x, secondRowY + KnobDiameter + 110, TextSize.Normal, TextAnchor.start));

        return g;
    }

    private static XmlElement CreateFilterSection(XmlDocument xmlDoc, ProgramData program, int x)
    {
        var g = xmlDoc.CreateElement("g", SvgNamespace);
        g.AppendChild(CreateLabel(xmlDoc, "**FILTER", x + KnobRadius, firstRowY - headerOffset, TextSize.SectionHeader));

        g.AppendChild(CreateKnob(xmlDoc, $"CUTOFF\nRaw: {program.FilterCutoff}\n({Percent1023String(program.FilterCutoff)})", x, firstRowY, PercentFromValue(program.FilterCutoff, 0, 1023)));
        g.AppendChild(CreateKnob(xmlDoc, $"RESONANCE\nRaw: {program.FilterResonance}\n({Percent1023String(program.FilterResonance)})", x, secondRowY, PercentFromValue(program.FilterResonance, 0, 1023)));

        g.AppendChild(CreateVSwitch(xmlDoc, x - KnobRadius - 5, thirdRowY, "DRIVE", program.FilterCutoffDrive, "0%", "50%", "100%"));
        g.AppendChild(CreateVSwitch(xmlDoc, x + KnobRadius + 5, thirdRowY, "KEY\nTRACK", program.FilterCutoffKeyboardTrack, "0%", "50%", "100%"));

        g.AppendChild(CreateDividerLine(xmlDoc, x + 125));
        return g;
    }

    private static XmlElement CreateMixerSection(XmlDocument xmlDoc, ProgramData program, int x)
    {
        var g = xmlDoc.CreateElement("g", SvgNamespace);
        g.AppendChild(CreateLabel(xmlDoc, "**MIXER", x + KnobRadius, firstRowY - headerOffset, TextSize.SectionHeader));

        g.AppendChild(CreateKnob(xmlDoc, $"VCO 1\nRaw: {program.Vco1Level}\n({Percent1023String(program.Vco1Level)})", x, firstRowY, PercentFromValue(program.Vco1Level, 0, 1023)));
        g.AppendChild(CreateKnob(xmlDoc, $"VCO 2\nRaw: {program.Vco2Level}\n({Percent1023String(program.Vco2Level)})", x, secondRowY, PercentFromValue(program.Vco2Level, 0, 1023)));
        g.AppendChild(CreateKnob(xmlDoc, $"MULTI\nRaw: {program.MultiLevel}\n({Percent1023String(program.MultiLevel)})", x, thirdRowY, PercentFromValue(program.MultiLevel, 0, 1023)));

        g.AppendChild(CreateDividerLine(xmlDoc, x + 90));

        return g;
    }

    private static XmlElement CreateVoiceModeSection(XmlDocument xmlDoc, ProgramData program, int x)
    {
        var g = xmlDoc.CreateElement("g", SvgNamespace);      

        g.AppendChild(CreateVoiceModeOctaveSwitch(xmlDoc, program, x - 20, firstRowY));
        g.AppendChild(CreateDividerLine(xmlDoc, x + 130));


        string scaleKeyToStr(byte val, int offset)
        {
            var valTransposed = (val - offset);
            var transposeStr = valTransposed > 0 ? $"+{valTransposed}" : valTransposed.ToString();
            return transposeStr;
        }

        var sb = new StringBuilder();
        sb.AppendLine("Program Level");
        sb.AppendLine(DisplayHelper.ProgramLevelDecibel(program.ProgramLevel));
        sb.AppendLine();
        sb.AppendLine("**ProgEdit / Pitch");
        sb.AppendLine("Microtuning");
        sb.AppendLine(program.MicroTuning.ToString());
        sb.AppendLine();
        sb.AppendLine("Scale Key");
        sb.AppendLine(scaleKeyToStr(program.ScaleKey, 12) + " Note(s)");
        sb.AppendLine();
        sb.AppendLine("Program Tuning");
        sb.AppendLine(scaleKeyToStr(program.ProgramTuning, 50) + " Cent");
        sb.AppendLine();
        sb.AppendLine("Program Transpose");
        sb.AppendLine(scaleKeyToStr(program.ProgramTranspose, 13) + " Note(s)");
        sb.AppendLine();
        sb.AppendLine("**ProgEdit / Joystick");
        sb.AppendLine("X+ Bend Range");
        sb.AppendLine(program.BendRangePlus == 0 ? "Off" : $"{program.BendRangePlus} Note(s)");
        sb.AppendLine();
        sb.AppendLine("X- Bend Range");
        sb.AppendLine(program.BendRangeMinus == 0 ? "Off" : $"{program.BendRangeMinus} Note(s)");
        sb.AppendLine();
        sb.AppendLine("Y+ Assign");
        sb.AppendLine(program.JoystickAssignPlus.ToString());
        sb.AppendLine();
        sb.AppendLine("Y+ Range");
        sb.AppendLine(DisplayHelper.MinusToPlus100String(program.JoystickRangePlus));
        sb.AppendLine();
        sb.AppendLine("Y- Assign");
        sb.AppendLine(program.JoystickAssignMinus.ToString());
        sb.AppendLine();
        sb.AppendLine("Y- Range");
        sb.AppendLine(DisplayHelper.MinusToPlus100String(program.JoystickRangeMinus));

        g.AppendChild(CreateLabel(xmlDoc, sb.ToString(), x + 38, firstRowY + 90));


        var secondX = x + 180;
        g.AppendChild(CreateDividerLine(xmlDoc, secondX + 125));            

        var portamentoPercent = Math.Round(PercentFromValue(program.Portamento, 0, 127), 2);
        var portamentoLabel = $"PORTAMENTO\n{portamentoPercent}%\nRaw: {program.Portamento}\nMode: {program.PortamentoMode}\nBPM Sync: {(program.PortamentoBPMSync ? "On" : "Off")}";
        g.AppendChild(CreateKnob(xmlDoc, portamentoLabel, secondX, firstRowY, portamentoPercent));

        var vmDepthPercent = PercentFromValue(program.VoiceModeDepth, 0, 1023);
        var vmDepthLabel = DisplayHelper.VoiceModeDepthLabel(program.VoiceModeType, program.VoiceModeDepth);
        g.AppendChild(CreateKnob(xmlDoc, $"VOICE MODE\nDEPTH\n\n{vmDepthLabel}\nRaw: {program.VoiceModeDepth}\n({Percent1023String(program.VoiceModeDepth)})", secondX, secondRowY, vmDepthPercent));            
        g.AppendChild(CreateVSwitch(xmlDoc, secondX-10, thirdRowY, "", (int)(program.VoiceModeType) - 1, "ARP/\nLATCH", "CHORD", "UNISON", "POLY"));
        
        return g;
    }

    private static XmlElement CreateOscillatorSection(XmlDocument xmlDoc, ProgramData program, int x)
    {
        var g = xmlDoc.CreateElement("g", SvgNamespace);

        g.AppendChild(CreateLabel(xmlDoc, "**VCO 1", x + 315, firstRowY - headerOffset, TextSize.SectionHeader));
        g.AppendChild(CreateLabel(xmlDoc, "**VCO 2", x + 315, secondRowY - headerOffset, TextSize.SectionHeader));

        g.AppendChild(CreateKnob(xmlDoc, $"PITCH\n{DisplayHelper.PitchCents(program.Vco1Pitch)} Cent\nRaw: {program.Vco1Pitch}\n({Percent1023String(program.Vco1Pitch)})", x + 200, firstRowY, PercentFromValue(program.Vco1Pitch, 0, 1023)));
        g.AppendChild(CreateKnob(xmlDoc, $"SHAPE\nRaw: {program.Vco1Shape}\n({Percent1023String(program.Vco1Shape)})", x + 355, firstRowY, PercentFromValue(program.Vco1Shape, 0, 1023)));
        g.AppendChild(CreateOscillatorSwitch(xmlDoc, x, firstRowY, program.Vco1Wave, program.Vco1Octave));

        g.AppendChild(CreateKnob(xmlDoc, $"PITCH\n{DisplayHelper.PitchCents(program.Vco2Pitch)} Cent\nRaw: {program.Vco2Pitch}\n({Percent1023String(program.Vco2Pitch)})", x + 200, secondRowY, PercentFromValue(program.Vco2Pitch, 0, 1023)));
        g.AppendChild(CreateKnob(xmlDoc, $"SHAPE\nRaw: {program.Vco2Shape}\n({Percent1023String(program.Vco2Shape)})", x + 355, secondRowY, PercentFromValue(program.Vco2Shape, 0, 1023)));
        g.AppendChild(CreateKnob(xmlDoc, $"CROSS MOD\nDEPTH\nRaw: {program.CrossModDepth}\n({Percent1023String(program.CrossModDepth)})", x + 510, secondRowY, PercentFromValue(program.CrossModDepth, 0, 1023)));
        g.AppendChild(CreateOscillatorSwitch(xmlDoc, x, secondRowY, program.Vco2Wave, program.Vco2Octave));

        g.AppendChild(CreateVSwitch(xmlDoc, x + 490, firstRowY, "SYNC", program.OscillatorSync ? 1 : 0, "Off", "On"));
        g.AppendChild(CreateVSwitch(xmlDoc, x + 580, firstRowY, "RING", program.RingMod ? 1 : 0, "Off", "On"));

        g.AppendChild(CreateDividerLine(xmlDoc, x + 650));
        return g;
    }

    private static XmlElement CreateMultiEngineSection(XmlDocument xmlDoc, ReportGeneratorInput input, int x)
    {
        var g = xmlDoc.CreateElement("g", SvgNamespace);


        g.AppendChild(CreateVSwitch(xmlDoc, x, thirdRowY, "", (int)input.Program.MultiOscType, "Noise", "VPM", "USR"));

        string title = "???";
        if (input.Program.MultiOscType == MultiOscType.User)
        {
            g.AppendChild(CreateMultiEngineUserSection(xmlDoc, input, x, out title));
        }
        else if (input.Program.MultiOscType == MultiOscType.VPM)
        {
            g.AppendChild(CreateMultiEngineVPMSection(xmlDoc, input.Program, x, out title));
        }
        else if (input.Program.MultiOscType == MultiOscType.Noise)
        {
            g.AppendChild(CreateMultiEngineNoiseSection(xmlDoc, input.Program, x, out title));
        }

        g.AppendChild(CreateLabel(xmlDoc, "**MULTI ENGINE: " + title, x + 315, thirdRowY - headerOffset, TextSize.SectionHeader));

        return g;
    }

    private static string MultiEngineProgramEditLabel(ProgramData program)
    {
        var octaveStr = program.MultiOctave switch
        {
            0 => "16'",
            1 => "8'",
            2 => "4'",
            3 => "2'",
            _ => $"??? (Raw: {program.MultiOctave})"
        };

        return $"\n**Program Edit / Other\nMulti Octave: {octaveStr}\nMulti Routing: {program.MultiRouting}";
    }

    private static XmlElement CreateMultiEngineUserSection(XmlDocument xmlDoc, ReportGeneratorInput input, int x, out string title)
    {
        var userOscDesc = input.GetUserOscillatorDescription();
        title = userOscDesc.Name + $" (#{(input.Program.SelectedMultiOscUser + 1)})";

        var g = xmlDoc.CreateElement("g", SvgNamespace);

        // SHIFT+SHAPE
        g.AppendChild(CreateKnob(xmlDoc, $"SHIFT+SHAPE\nRaw: {input.Program.ShiftShapeUser}", x + 355, thirdRowY, PercentFromValue(input.Program.ShiftShapeUser, 0, 1023)));

        // SHAPE
        g.AppendChild(CreateKnob(xmlDoc, $"SHAPE\nRaw: {input.Program.ShapeUser}", x + 510, thirdRowY, PercentFromValue(input.Program.ShapeUser, 0, 1023)));

        var userParams = "**Program Edit / Multi Engine\n"
            + $"{userOscDesc.UserParamLabel1}: {UserOscillatorDescription.FormatParamValue(input.Program.UserParam1Type, input.Program.UserParam1)}\n"
            + $"{userOscDesc.UserParamLabel2}: {UserOscillatorDescription.FormatParamValue(input.Program.UserParam2Type, input.Program.UserParam2)}\n"
            + $"{userOscDesc.UserParamLabel3}: {UserOscillatorDescription.FormatParamValue(input.Program.UserParam3Type, input.Program.UserParam3)}\n"
            + $"{userOscDesc.UserParamLabel4}: {UserOscillatorDescription.FormatParamValue(input.Program.UserParam4Type, input.Program.UserParam4)}\n"
            + $"{userOscDesc.UserParamLabel5}: {UserOscillatorDescription.FormatParamValue(input.Program.UserParam5Type, input.Program.UserParam5)}\n"
            + $"{userOscDesc.UserParamLabel6}: {UserOscillatorDescription.FormatParamValue(input.Program.UserParam6Type, input.Program.UserParam6)}\n"
            + MultiEngineProgramEditLabel(input.Program);
        g.AppendChild(CreateLabel(xmlDoc, userParams, x + 80, thirdRowY, TextSize.Normal, TextAnchor.start));

        return g;
    }

    private static XmlElement CreateMultiEngineVPMSection(XmlDocument xmlDoc, ProgramData program, int x, out string title)
    {
        var g = xmlDoc.CreateElement("g", SvgNamespace);
        //g.AppendChild(CreateLabel(xmlDoc, $"VPM Type: {program.SelectedMultiOscVPM}", x + 80, thirdRowY, TextSize.MultiOscName, TextAnchor.start));
        title = $"VPM / {program.SelectedMultiOscVPM}";

        // SHAPE: MOD DEPTH 0-15, but depends on Type - TODO
        g.AppendChild(CreateKnob(xmlDoc, $"SHAPE\nMOD DEPTH\nRaw: {program.ShapeNoise}\n({Percent1023String(program.ShapeVPM)})", x + 510, thirdRowY, PercentFromValue(program.ShapeNoise, 0, 1023)));

        // SHIFT+SHAPE: RATIO OFFSET, but depends on Type - TODO
        g.AppendChild(CreateKnob(xmlDoc, $"SHIFT+SHAPE\nRATIO OFFSET\nRaw: {program.ShiftShapeNoise}\n({Percent1023String(program.ShiftShapeVPM)})", x + 355, thirdRowY, PercentFromValue(program.ShiftShapeNoise, 0, 1023)));

        // 0~200=-100%~+100%
        var vpmSettings = "**Program Edit / Multi Engine\n" 
            + $"Feedback: {DisplayHelper.MinusToPlus100String(program.VPMParameter1Feedback)}\n"
            + $"Noise Depth: {DisplayHelper.MinusToPlus100String(program.VPMParameter2NoiseDepth)}\n"
            + $"Shape Mod Int: {DisplayHelper.MinusToPlus100String(program.VPMParameter3ShapeModInt)}\n"
            + $"Mod Attack: {DisplayHelper.MinusToPlus100String(program.VPMParameter4ModAttack)}\n"
            + $"Mod Decay: {DisplayHelper.MinusToPlus100String(program.VPMParameter5ModDecay)}\n"
            + $"Mod Key Track: {DisplayHelper.MinusToPlus100String(program.VPMParameter6ModKeyTrack)}\n"
            + MultiEngineProgramEditLabel(program);
        g.AppendChild(CreateLabel(xmlDoc, vpmSettings, x + 80, thirdRowY, TextSize.Normal, TextAnchor.start));

        return g;
    }

    private static XmlElement CreateMultiEngineNoiseSection(XmlDocument xmlDoc, ProgramData program, int x, out string title)
    {
        var g = xmlDoc.CreateElement("g", SvgNamespace);
        //g.AppendChild(CreateLabel(xmlDoc, $"Noise Type: {program.SelectedMultiOscNoise}", x + 80, thirdRowY, TextSize.MultiOscName, TextAnchor.start));
        title = $"Noise / {program.SelectedMultiOscNoise}";

        // TODO: The Shape values are not linear, and not officially documented
        string shapeLabel;
        if (program.SelectedMultiOscNoise == MultiOscNoise.Decim)
        {
            // SHIFT+SHAPE: KEY TRACK
            g.AppendChild(CreateKnob(xmlDoc, $"SHIFT+SHAPE\nKEY TRACK\nRaw: {program.ShiftShapeNoise}\n({Percent1023String(program.ShiftShapeNoise)})", x + 355, thirdRowY, PercentFromValue(program.ShiftShapeNoise, 0, 1023)));

            // SHAPE: RATE [240Hz...48.0kHz]
            shapeLabel = $"RATE";
        }
        else if (program.SelectedMultiOscNoise == MultiOscNoise.Peak)
        {
            // SHAPE: BANDWIDTH [110.0Hz...880.0Hz]
            shapeLabel = $"BANDWIDTH";
        }
        else // High/Low
        {
            // SHAPE: CUTOFF [10.0Hz...21.0kHz]
            shapeLabel = $"CUTOFF";
        }

        g.AppendChild(CreateKnob(xmlDoc, $"SHAPE\n{shapeLabel}\nRaw: {program.ShapeNoise}\n({Percent1023String(program.ShapeNoise)})", x + 510, thirdRowY, PercentFromValue(program.ShapeNoise, 0, 1023)));

        var noiseSettings = MultiEngineProgramEditLabel(program);
        g.AppendChild(CreateLabel(xmlDoc, noiseSettings, x + 80, thirdRowY, TextSize.Normal, TextAnchor.start));

        return g;
    }

    private static XmlElement CreateVSwitch(XmlDocument xmlDoc, int x, int y, string sectionName, int selectedIndex, params string[] values)
    {
        var g = xmlDoc.CreateElement("g", SvgNamespace);
        int cx = x + LedRadius;
        int cy = y + LedRadius;
        var labelX = cx + LedRadius + 4;
        var currentY = y + LedRadius;
        var yIncr = (LedRadius * 3) + 2;

        for (int i = (values.Length-1); i >= 0; i--)
        {
            var labelText = values[i];

            if (!string.IsNullOrEmpty(labelText))
            {
                var fill = i == selectedIndex ? switchActiveColor : "transparent";
                g.AppendChild(CreateCircle(xmlDoc, cx, currentY, LedRadius, fill));
                g.AppendChild(CreateLabel(xmlDoc, labelText, labelX, currentY - 8, TextSize.Normal, TextAnchor.start));
            }
            currentY += yIncr;
        }

        if (!string.IsNullOrEmpty(sectionName))
        {
            g.AppendChild(CreateLabel(xmlDoc, sectionName, labelX + 4, currentY - 8, TextSize.Normal, TextAnchor.middle));
        }

        return g;
    }

    private static XmlElement CreateOscillatorSwitch(XmlDocument xmlDoc, int x, int y, VcoWave wave, byte octave)
    {
        var g = xmlDoc.CreateElement("g", SvgNamespace);

        g.AppendChild(CreateVSwitch(xmlDoc, x, y, "WAVE", (int)VcoWave.SAW, "", "SQR", "TRI", "SAW"));
        g.AppendChild(CreateVSwitch(xmlDoc, x + 100, y, "OCTAVE", octave, "16'", "8'", "4'", "2'"));

        return g;
    }

    private static XmlElement CreateVoiceModeOctaveSwitch(XmlDocument xmlDoc, ProgramData program, int x, int y)
    {
        var g = xmlDoc.CreateElement("g", SvgNamespace);
        int cx = x + LedRadius;
        int cy = y + LedRadius;

        for (var i = 0; i < 5; i++)
        {
            var offset = i * ((LedRadius + 2) * 2);
            var circle = CreateCircle(xmlDoc, cx + offset, cy, LedRadius, fill: program.Octave == i ? switchActiveColor : "transparent");
            g.AppendChild(circle);
        }

        g.AppendChild(CreateLabel(xmlDoc, $"OCTAVE\n({program.Octave - 2})", x + 58, y + 40));
        return g;
    }

    private static XmlElement CreateKnob(XmlDocument xmlDoc, string text, int x, int y, double percent)
    {
        var g = xmlDoc.CreateElement("g", SvgNamespace);
        var cx = x + KnobRadius;
        var cy = y + KnobRadius;

        g.AppendChild(CreateCircle(xmlDoc, cx, cy, KnobRadius));
        
        if (!double.IsNaN(percent))
        {
            var indicator = CreateLine(xmlDoc, cx, cy, cx, cy + KnobRadius);
            var indicatorRotate = PercentToDegree(percent);
            indicator.SetAttribute("transform", $"rotate({indicatorRotate} {cx} {cy})");
            g.AppendChild(indicator);
        }

        if (!string.IsNullOrEmpty(text))
        {
            g.AppendChild(CreateLabel(xmlDoc, text, cx, cy + KnobDiameter));
        }

        return g;
    }

    private static XmlElement CreateLine(XmlDocument xmlDoc, int x1, int y1, int x2, int y2, int thickness = strokeWidth)
    {
        var line = xmlDoc.CreateElement("line", SvgNamespace);
        line.SetAttribute("stroke", strokeColor);
        line.SetAttribute("stroke-width", thickness.ToString());
        line.SetAttribute("x1", x1.ToString());
        line.SetAttribute("y1", y1.ToString());
        line.SetAttribute("x2", x2.ToString());
        line.SetAttribute("y2", y2.ToString());
        return line;
    }

    private static XmlElement CreateDividerLine(XmlDocument xmlDoc, int x)
    {
        const int lineY1 = firstRowY;
        const int lineY2 = synthHeight - 100;
        const int lineThickness = 2;
        return CreateLine(xmlDoc, x, lineY1, x, lineY2, lineThickness);
    }

    private static XmlElement CreateCircle(XmlDocument xmlDoc, int cx, int cy, int radius, string fill = "transparent", int thickness = strokeWidth)
    {
        var circle = xmlDoc.CreateElement("circle", SvgNamespace);
        circle.SetAttribute("cx", cx.ToString());
        circle.SetAttribute("cy", cy.ToString());
        circle.SetAttribute("r", radius.ToString());
        circle.SetAttribute("stroke", strokeColor);
        circle.SetAttribute("fill", fill);
        circle.SetAttribute("stroke-width", thickness.ToString());
        return circle;
    }

    private static XmlElement CreateLogo(XmlDocument xmlDoc, int x, int y)
    {
        var elem = xmlDoc.CreateElement("g", SvgNamespace);

        var korgLogo = xmlDoc.CreateElement("path", SvgNamespace);
        korgLogo.SetAttribute("d", "m 156.03997,1.1862376 h -22.7358 c -11.0293,0 -11.80454,10.7904014 -11.80454,10.7904014 v 29.967888 c 0.0158,9.625935 10.98025,10.864769 10.98025,10.864769 h 23.56009 V 24.833357 c 2.7055,0 2.51565,-5.045541 0,-5.045541 -2.27832,0 -14.98314,0 -14.98314,0 v 19.115786 c 0,3.553547 -5.1579,3.553547 -5.1579,0 0,-3.605772 0,-23.76737 0,-23.76737 0,-2.300473 2.57895,-2.58368 2.57895,-2.58368 h 17.56209 V 1.1862376 M 100.25106,19.272028 c 0,3.058334 -5.173688,3.395337 -5.173688,0 V 14.10466 c -0.142421,-3.547227 5.189508,-3.608932 5.173688,0 z m 12.3884,9.298415 -6.20209,-1.548945 c 4.84143,-1.272067 7.48364,-3.933277 7.56277,-8.300066 V 8.6604112 c 0,-1.7087456 -2.16758,-7.2162787 -9.11331,-7.4725899 H 80.616341 V 52.856765 H 95.077372 V 33.220439 c -0.332261,-1.015754 2.325808,-3.297241 3.069421,-0.314849 l 4.161107,19.951175 h 16.02737 L 112.63946,28.570443 M 59.935799,38.903602 c 0,3.229202 -5.157884,3.284585 -5.157884,0 v -23.76737 c 0,-3.251356 5.157884,-3.227625 5.157884,0 0,3.229212 0,20.541319 0,23.76737 z M 74.3652,11.884876 c 0,0 -0.506308,-10.4423256 -11.834646,-10.95811384 H 52.198982 C 50.869954,0.88093668 40.441876,2.391852 40.299476,12.812027 v 29.366664 c 0,0 0.901838,10.909063 11.899506,10.932794 h 10.331572 c 0,0 11.897916,-0.403469 11.88211,-11.880517 L 74.365184,11.884876 M 0,52.855174 V 1.1862376 H 14.476856 V 20.302021 c 0.237329,2.596339 2.879553,1.442938 3.069413,0.246819 L 20.678968,1.1862376 H 36.69211 l -5.679986,23.7673634 -6.202115,1.547361 6.708407,1.552111 5.695813,24.802101 H 21.186844 L 17.546269,31.921477 c -0.174043,-1.62014 -3.021944,-1.634381 -3.069413,0.265806 -0.04749,1.897023 0,20.667891 0,20.667891 H 0");
        korgLogo.SetAttribute("stroke", "none");
        korgLogo.SetAttribute("fill", strokeColor);
        korgLogo.SetAttribute("fill-rule", "nonzero");
        korgLogo.SetAttribute("transform", $"translate({padding + x}, {padding + y})");
        elem.AppendChild(korgLogo);

        var logoWidth = 180 + padding;

        var minxdLabel = CreateLabel(xmlDoc, "minilogue xd", x + logoWidth, y + padding - 8, TextSize.ProgramHeader, TextAnchor.start);
        minxdLabel.SetAttribute("font-weight", "bold");
        elem.AppendChild(minxdLabel);

        elem.AppendChild(CreateLabel(xmlDoc, "POLYPHONIC ANALOGUE SYNTHESIZER", x + logoWidth, y + padding + 40, TextSize.SynthName, TextAnchor.start));
        return elem;
    }

    private static XmlElement CreateLabel(XmlDocument xmlDoc, string text, int x, int y, TextSize size = TextSize.Normal, TextAnchor anchor = TextAnchor.middle)
    {
        string fontSize = size switch
        {
            TextSize.Normal => "1.2em",
            TextSize.ProgramHeader => "3em",
            TextSize.SectionHeader => "1.6em",
            TextSize.SynthName => "1.5em",
            TextSize.MultiOscName => "1.5em",
            _ => "1.2em"
        };

        int offsetIncr = size switch
        {
            TextSize.Normal => 20,
            TextSize.ProgramHeader => 50,
            TextSize.MultiOscName => 25,
            TextSize.SectionHeader => 26,
            TextSize.SynthName => 25,
            _ => 22
        };

        var g = xmlDoc.CreateElement("g", SvgNamespace);
        var lines = text.Trim('\n').Replace("\r", "").Split('\n');
        var yOffset = 0;

        var fontFamily = "Arial, sans-serif";

        foreach (var line in lines)
        {
            var bold = line.StartsWith("**");

            var label = xmlDoc.CreateElement("text", SvgNamespace);
            label.SetAttribute("x", x.ToString());
            label.SetAttribute("y", (y + yOffset).ToString());
            label.SetAttribute("dominant-baseline", "hanging");
            label.SetAttribute("text-anchor", anchor.ToString());
            label.SetAttribute("font-family", fontFamily);
            label.SetAttribute("font-size", fontSize);
            if (bold)
            {
                label.SetAttribute("font-weight", "bold");
            }
            label.SetAttribute("fill", strokeColor);
            label.InnerText = bold ? line[2..] : line;

            if (lines.Length == 1)
            {
                // No need to wrap into a <g> element if there's just one
                return label;
            }

            g.AppendChild(label);

            yOffset += offsetIncr;
        }

        return g;
    }

    private static string Percent1023String(ushort value)
    {
        var percent = (value / 1023f) * 100;
        return Math.Round(percent, 2) + "%";
    }

    private static double PercentFromValue(int value, int min, int max)
    {
        var flooredValue = value - min;
        var range = max - min;
        var percentFactor = range / 100d;
        var percent = flooredValue / percentFactor;
        return percent;
    }

    private static double PercentToDegree(double percent)
    {
        percent = Math.Clamp(percent, 0, 100);
        if (double.IsNaN(percent)) { return 0d; }

        // Assumes that 0 degrees is a line pointing straight down
        // min and max are because on a real Minilogue XD,
        // the knob can only turn a limited amount
        var minAngle = 35;
        var maxAngle = 360 - minAngle;
        var steps = (maxAngle - minAngle) / 100d;
        var result = minAngle + (steps * percent);
        return Math.Round(result, 2);
    }

    private enum TextAnchor
    {
        start,
        middle,
        end
    }

    private enum TextSize
    {
        Normal = 0,
        ProgramHeader,
        SectionHeader,
        SynthName,
        MultiOscName
    }
}
