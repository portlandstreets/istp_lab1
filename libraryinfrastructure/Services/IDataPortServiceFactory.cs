using tournamentdomain.Model;

namespace libraryinfrastructure.Services
{
    public interface IDataPortServiceFactory<TEntity> where TEntity : class
    {
        IImportService<TEntity> GetImportService(string contentType);
        IExportService<TEntity> GetExportService(string contentType);
    }
}
