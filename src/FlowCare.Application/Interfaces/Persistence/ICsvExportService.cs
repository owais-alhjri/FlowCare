namespace FlowCare.Application.Interfaces.Persistence;

public interface ICsvExportService
{
    byte[] Export<T>(List<T> data);
}