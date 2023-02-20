using System.Collections.Immutable;
using System.IO.Compression;
using System.Text;
using System.Xml;

namespace mnlxdprogdump;

public static class LibraryFileReader
{
    // Valid files:
    // .mnlgxdlib - for the entire library
    // .mnlgxdprog - for a single program
    // .prog_bin - the program after extracting from one of the above
    public static ImmutableList<(string Name, byte[] Content)> GetAllPrograms(byte[] input)
    {
        // If we load a Prog_000.prog_bin, we can short circuit
        var strBuf = Encoding.ASCII.GetString(input, 0, 2);
        if (strBuf == "PR")
        {
            return new List<(string, byte[])> {
                ("Prog_000.prog_bin", input)
            }.ToImmutableList();
        }
        else if (strBuf == "PK")
        {
            return GetAllProgramsFromZip(input);
        }

        throw new InvalidOperationException("Unsupported file format.");
    }

    private static ImmutableList<(string Name, byte[] Content)> GetAllProgramsFromZip(byte[] input)
    {
        using var ms = new MemoryStream(input);
        using var za = new ZipArchive(ms, ZipArchiveMode.Read);
        var zentries = za.Entries;

        var fileInfo = GetZipArchiveEntryContent(zentries, "FileInformation.xml");
        var fix = new XmlDocument();
        fix.LoadXml(Encoding.UTF8.GetString(fileInfo));

        /* FileInformation.xml:

         <KorgMSLibrarian_Data>
          <Product>minilogue xd</Product>
          <Contents NumFavoriteData="1" NumProgramData="500" NumPresetInformation="0"
                    NumTuneScaleData="6" NumTuneOctData="6">
            <ProgramData>
              <Information>Prog_000.prog_info</Information>
              <ProgramBinary>Prog_000.prog_bin</ProgramBinary>
            </ProgramData>
            <ProgramData>
              <Information>Prog_001.prog_info</Information>
              <ProgramBinary>Prog_001.prog_bin</ProgramBinary>
            </ProgramData> */
        var productNode = fix.SelectSingleNode("/KorgMSLibrarian_Data/Product");
        if (!string.Equals(productNode?.InnerText, "minilogue xd", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Unsupported file, must be a Minilogue XD library file, Product of the file is " + productNode?.InnerText);
        }

        var contentsNode = fix.SelectSingleNode("/KorgMSLibrarian_Data/Contents") ?? throw new InvalidOperationException("There was no Contents node in the FileInformation.xml");

        var result = ImmutableList.CreateBuilder<(string Name, byte[] Content)>();
        foreach (var cn in contentsNode.ChildNodes.OfType<XmlElement>().Where(x => x.LocalName == "ProgramData"))
        {
            var filename = cn.ChildNodes.OfType<XmlElement>().Single(x => x.LocalName == "ProgramBinary").InnerText;
            var filecontent = GetZipArchiveEntryContent(zentries, filename);
            result.Add((filename, filecontent));
        }
        return result.ToImmutable();
    }

    private static byte[] GetZipArchiveEntryContent(ICollection<ZipArchiveEntry> entries, string filename)
    {
        var entry = entries?.FirstOrDefault(e => string.Equals(e.Name, filename, StringComparison.OrdinalIgnoreCase));
        if (entry == null)
        {
            throw new InvalidOperationException("Not found in Zip Archive: " + filename);
        }
        return GetZipArchiveEntryContent(entry);
    }

    private static byte[] GetZipArchiveEntryContent(ZipArchiveEntry zae)
    {
        using var ms = new MemoryStream();
        using var source = zae.Open();
        source.CopyTo(ms);
        return ms.ToArray();
    }
}
