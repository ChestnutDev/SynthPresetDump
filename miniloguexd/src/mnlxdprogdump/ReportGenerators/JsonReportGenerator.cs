using System.Text.Json;

namespace mnlxdprogdump;

public class JsonReportGenerator : IReportGenerator
{
    public string PreferredFileExtension => ".json";

    public string GenerateReport(ReportGeneratorInput input)
    {
        return JsonSerializer.Serialize(input, new JsonSerializerOptions
        {
            IncludeFields = true,
            WriteIndented = true
        });
    }
}
