namespace UserPermissionsN5.Services
{
    public interface IElasticsearchService<T> where T : class
    {
        Task IndexDocumentAsync(T document);
    }
}
