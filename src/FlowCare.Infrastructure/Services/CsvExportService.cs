using System.Globalization;
using CsvHelper;
using FlowCare.Application.Interfaces.Persistence;

namespace FlowCare.Infrastructure.Services;

public class CsvExportService : ICsvExportService
{
    public byte[] Export<T>(List<T> data)
    {
        using var ms = new MemoryStream();
        using var writer = new StreamWriter(ms);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(data);
        writer.Flush();
        return ms.ToArray();

    }
}