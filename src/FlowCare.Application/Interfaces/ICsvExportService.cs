namespace FlowCare.Application.Interfaces;

public interface ICsvExportService
{
    byte[] Export<T>(List<T> data);
}