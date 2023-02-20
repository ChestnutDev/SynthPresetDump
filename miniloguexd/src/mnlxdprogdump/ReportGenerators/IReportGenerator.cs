namespace mnlxdprogdump;

public interface IReportGenerator
{
    string PreferredFileExtension { get; }
    string GenerateReport(ReportGeneratorInput input);
}
