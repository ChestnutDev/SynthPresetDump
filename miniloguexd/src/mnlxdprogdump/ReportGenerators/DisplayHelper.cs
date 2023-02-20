namespace mnlxdprogdump;

public static class DisplayHelper
{
    /// <summary>
    /// Maps <see cref="SequencerDataV2.ARPGateTime"/>, <see cref="SequencerData.DefaultGateTime"/>,
    /// <see cref="StepEventData.GateTime1"/> to human readable values.
    /// 
    /// Note that <see cref="SequencerDataV2.ARPGateTime"/> ranges from 1-73 instead of 0-72, so it needs
    /// to get shifted first.
    /// 
    /// Also note that <see cref="StepEventData.GateTime1"/> etc. has a TIE Value for values >72.
    /// </summary>
    private static byte[] GateTimeToPercentValues = new byte[73] { 0, 1, 2, 4, 5, 6, 8, 9, 11, 12, 13, 15, 16, 18, 19, 20, 22, 23, 25, 26, 27, 29, 30, 31, 33, 34, 36, 37, 38, 40, 41, 43, 44, 45, 47, 48, 50, 51, 52, 54, 55, 56, 58, 59, 61, 62, 63, 65, 66, 68, 69, 70, 72, 73, 75, 76, 77, 79, 80, 81, 83, 84, 86, 87, 88, 90, 91, 93, 94, 95, 97, 98, 100 };

    public static byte ARPGateTimeToPercent(byte arpGateTime)
        => GateTimeToPercentValues[arpGateTime - 1];

    public static byte SEQGateTimeToPercent(byte seqGateTime)
        => GateTimeToPercentValues[seqGateTime];

    public static string ProgramLevelDecibel(byte programLevel)
    {
        //  12~132=-18dB~+6dB - in 0.2dB increments
        var steps = (programLevel - 12) * 2d;
        var result = -180d + steps;
        var symbol = result == 0 ? "" : result > 0 ? "+" : "";
        var rounded = Math.Round(result / 10, 1);

        return $"{symbol}{rounded:0.0} dB";
    }

    /// <remarks>
    /// *note P5 (VCO1/2 PITCH)
    /// Korg lists the same value as the end of one and start of the next section,
    /// so that's not a mistake here.
    /// 
    ///    0 ~    4 : -1200        (Cent)
    ///    4 ~  356 : -1200 ~ -256 (Cent)
    ///  356 ~  476 :  -256 ~  -16 (Cent)
    ///  476 ~  492 :   -16 ~    0 (Cent)
    ///  492 ~  532 :     0        (Cent)
    ///  532 ~  548 :     0 ~   16 (Cent)
    ///  548 ~  668 :    16 ~  256 (Cent)
    ///  668 ~ 1020 :   256 ~ 1200 (Cent)
    /// 1020 ~ 1023 :  1200        (Cent)
    /// 
    /// Thanks to gekart on GitHub:
    /// https://gist.github.com/gekart/b187d3c16e6160571ccfcf6c597fea3f#file-mnlgxd-py-L386
    /// 
    /// That said, I'm not sure if those values are really correct in the Korg manual.
    /// For example, the Replicant XD Preset has a VCO2 Pitch of 553 (0x2902) which
    /// is +26 Cent according to the table, but the display on the Minilogue XD shows
    /// it's more like +10 Cent.
    /// 
    /// So, need to do some more testing to see if the values in the MIDI Implementation
    /// Manual are wrong/outdated.
    /// </remarks>
    public static string PitchCents(ushort value) => value switch
    {
        >= 0 and <= 4 => "-1200",
        >= 4 and <= 356 => $"{(value - 356) * 944 / 352f - 256}",
        >= 356 and <= 476 => $"{(value - 476) * 2 - 16}",
        >= 476 and <= 492 => $"{value - 492}",
        >= 492 and <= 532 => "0",
        >= 532 and <= 548 => $"+{value - 532}",
        >= 548 and <= 668 => $"+{(value - 548) * 2 + 16}",
        >= 668 and <= 1020 => $"+{(value - 668) * 944 / 352f + 256}",
        >= 1020 and <= 1023 => "+1200",
        _ => "???"
    };

    /// <summary>
    /// *note P10
    ///    0 ~   11 : -100 (%)
    ///   11 ~  492 : - ((492 - value) * (492 - value) * 4641 * 100) / 0x40000000 (%)
    ///  492 ~  532 : 0 (%)
    ///  532 ~ 1013 : ((value - 532) * (value - 532) * 4641 * 100) / 0x40000000 (%)
    /// 1013 ~ 1023 : 100 (%) 
    /// </summary>
    public static double EGIntPercent(ushort egIntVal)
    {
        var valDouble = (double)egIntVal;
        return egIntVal switch
        {
            <= 11 => -100d,
            > 11 and < 492 => -((492 - valDouble) * (492 - valDouble) * 4641 * 100) / 0x40000000,
            >= 492 and <= 532 => 0d,
            > 532 and < 1013 => ((valDouble - 532) * (valDouble - 532) * 4641 * 100) / 0x40000000,
            _ => 100d
        };
    }

    /// <summary>
    /// *note P11 (LFO RATE)
    /// [BPM SYNC OFF]
    ///  0 ~ 1023  : 0 ~ 1023 
    /// [BPM SYNC ON]
    ///     0 ~   63  : 4
    ///    64 ~  127  : 2
    ///   128 ~  191  : 1
    ///   192 ~  255  : 3/4
    ///   256 ~  319  : 1/2
    ///   320 ~  383  : 3/8
    ///   384 ~  447  : 1/3
    ///   448 ~  511  : 1/4
    ///   512 ~  575  : 3/16
    ///   576 ~  639  : 1/6
    ///   640 ~  703  : 1/8
    ///   704 ~  767  : 1/12
    ///   768 ~  831  : 1/16
    ///   832 ~  895  : 1/24
    ///   896 ~  959  : 1/32
    ///   960 ~ 1023  : 1/36
    /// </summary>
    public static string LFORate(ushort lfoRate, LFOMode lfoMode)
    {
        if (lfoMode != LFOMode.BPM) { return lfoRate.ToString(); }

        return lfoRate switch
        {
            <= 63 => "4",
            <= 127 => "2",
            <= 191 => "1",
            <= 255 => "3/4",
            <= 319 => "1/2",
            <= 383 => "3/8",
            <= 447 => "1/3",
            <= 511 => "1/4",
            <= 575 => "3/16",
            <= 639 => "1/6",
            <= 703 => "1/8",
            <= 767 => "1/12",
            <= 831 => "1/16",
            <= 895 => "1/24",
            <= 959 => "1/32",
            <= 1023 => "1/36",
            _ => "???"
        };
    }

    public static string VoiceModeDepthLabel(VoiceModeType voiceModeType, ushort voiceModeDepth)
    {
        switch (voiceModeType)
        {
            case VoiceModeType.Poly:
                if (voiceModeDepth <= 255) { return "Poly"; }
                // TODO: How to get the exact value, since the display shows 0-1023 even though 0-255 are for poly?
                return $"Duo";
            case VoiceModeType.Unison:
                var detune = Math.Round((voiceModeDepth * 50) / 1023f, 0);
                return $"Detune {detune} Cent";
            case VoiceModeType.Chord:
                return voiceModeDepth switch
                {
                    > 1023 => "???",
                    >= 951 => "Maj7b5",
                    >= 878 => "mMaj7",
                    >= 805 => "m7b5",
                    >= 732 => "dim",
                    >= 659 => "aug",
                    >= 586 => "Maj7",
                    >= 512 => "7sus4",
                    >= 439 => "7",
                    >= 366 => "m7",
                    >= 293 => "sus4",
                    >= 220 => "Maj",
                    >= 147 => "m",
                    >= 74 => "sus2",
                    _ => "5th"
                };
            case VoiceModeType.Arp:
                return voiceModeDepth switch
                {
                    > 1023 => "???",
                    >= 937 => "RANDOM 3",
                    >= 859 => "RANDOM 2",
                    >= 781 => "RANDOM 1",
                    >= 703 => "POLY 2",
                    >= 625 => "POLY 1",
                    >= 547 => "RISE FALL 2",
                    >= 469 => "RISE FALL 1",
                    >= 391 => "FALL 2",
                    >= 313 => "FALL 1",
                    >= 235 => "RISE 2",
                    >= 157 => "RISE 1",
                    >= 79 => "MANUAL 2",
                    _ => "MANUAL 1"
                };
            default: return voiceModeDepth.ToString();
        }
    }

    public static string MinusToPlus100String(byte value)
    {
        int signed = value - 100;
        if (signed <= 0) { return $"{signed}%"; }
        return $"{signed}%";
    }
}
