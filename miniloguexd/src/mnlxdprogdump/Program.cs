using System.Text.Json;
using System.Text.RegularExpressions;

namespace mnlxdprogdump;

internal class Program
{
    private static void PrintUsage()
    {
        var exeName = Path.GetFileName(Environment.ProcessPath);
        Console.WriteLine($"USAGE: {exeName} <LibraryFileName (.mnlgxdlib, .mnlgxdprog, .prog_bin)>");
    }

    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            PrintUsage();
            return;
        }

        var userUnitDescriptions = ReadUserOscillatorDescriptionsJson();
        var userUnitMappings = ReadUserMappingsJson();

        Console.WriteLine($"Processing {args[0]}...");

        var fileContent = File.ReadAllBytes(args[0]);
        var entries = LibraryFileReader.GetAllPrograms(fileContent);
        Console.WriteLine($"Found {entries.Count} programs...");

        var outputPathBase = Path.GetFileNameWithoutExtension(args[0]) + "_";

        var reportGenerators = new List<IReportGenerator>
        {
            new JsonReportGenerator(),
            new SVGGenerator()
        };

        foreach (var (Name, Content) in entries)
        {
            Console.WriteLine();
            Console.WriteLine($"Processing {Name}...");

            ProgramData program;
            try
            {
                program = ProgramParser.Read<ProgramData>(Content);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\tError reading ProgramData, skipping.");
                Console.WriteLine("\t" + ex.Message);
                continue;
            }

            // TODO: Sequencer
            var reportInput = new ReportGeneratorInput(program)
            {
                UserUnitDescriptions = userUnitDescriptions,
                UserUnitMappings = userUnitMappings
            };

            try
            {
                if (SequencerDataV2.IsSequencerV2Data(Content))
                {
                    reportInput.SequencerV2 = ProgramParser.Read<SequencerDataV2>(Content);
                }
                else if (SequencerDataV1.IsSequencerV1Data(Content))
                {
                    var seqV1 = ProgramParser.Read<SequencerDataV1>(Content);
                    reportInput.SequencerV2 = SequencerDataV2.ConvertFromV1(seqV1);
                }
                else
                {
                    Console.WriteLine($"Could not read Sequencer Data as either V1 or V2.");
                }
            }
            catch (Exception seqEx)
            {
                Console.WriteLine($"Error reading Sequencer data: {seqEx.Message}");
                reportInput.SequencerV2 = null;
            }

            var progName = program.ProgramName?.TrimEnd() ?? Name;
            var reportBaseFilename = outputPathBase + GetProgFileName(Name) + "_" + EscapeFileName(progName);

            foreach (var repGen in reportGenerators)
            {
                var reportOutputFilename = reportBaseFilename + repGen.PreferredFileExtension;
                Console.WriteLine($"Writing report to {reportOutputFilename}...");
                var report = repGen.GenerateReport(reportInput);
                File.WriteAllText(reportOutputFilename, report);
            }
        }

        Console.WriteLine("Done!");
    }

    private static string GetProgFileName(string input)
    {
        // "Prog_000" => Prog_001 to match the User Interface which starts at 001
        input = Path.GetFileNameWithoutExtension(input);
        var m = Regex.Match(input, "^Prog_(?<Num>\\d{3})$");
        if (m.Success)
        {
            var num = int.Parse(m.Groups["Num"].Value) + 1;
            input = num.ToString().PadLeft(3, '0');
        }
        return input;
    }

    private static string EscapeFileName(string? name)
    {
        if (name == null) { return ""; }
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }
        return name;
    }


    public static UserOscillatorDescriptions ReadUserOscillatorDescriptionsJson(string filename = "userOscillatorDescriptions.json")
    {
        if (File.Exists(filename))
        {
            try
            {
                var json = File.ReadAllText(filename);
                var dict = JsonSerializer.Deserialize<Dictionary<string, UserOscillatorDescription>>(json) ?? new Dictionary<string, UserOscillatorDescription>();
                return new UserOscillatorDescriptions
                {
                    UserOscillators = dict
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not load User Units from {filename}: {ex.Message}");
            }
        }
        return new UserOscillatorDescriptions();
    }

    public static UserUnitMappings ReadUserMappingsJson(string filename = "userUnitMappings.json")
    {
        if (File.Exists(filename))
        {
            try
            {
                var json = File.ReadAllText(filename);
                return JsonSerializer.Deserialize<UserUnitMappings>(json) ?? new UserUnitMappings();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not load User Unit Mappings from {filename}: {ex.Message}");
            }
        }
        return new UserUnitMappings();
    }
}