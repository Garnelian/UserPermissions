namespace UserPermissionsN5.Services
{
    public interface IQueryServerEngine<T> where T : class
    {
        Task IndexDocumentAsync(T document);
    }
}
