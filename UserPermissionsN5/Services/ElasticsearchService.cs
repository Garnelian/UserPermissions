using Nest;
using UserPermissionsN5.Common;
using UserPermissionsN5.Models;

namespace UserPermissionsN5.Services
{
    public class ElasticsearchService<T> : IElasticsearchService<T> where T : class
    {
        private readonly ElasticClient _client;

        public ElasticsearchService(IConfiguration configuration)
        {
            var settings = new ConnectionSettings(new Uri(configuration["Elasticsearch:Uri"] ?? Constants.ELASTIC_SEARCH_DEFAULT_URI))
                .DefaultIndex(configuration["Elasticsearch:Index"] ?? Constants.ELASTIC_SEARCH_DEFAULT_INDEX);
            _client = new ElasticClient(settings);
        }

        public async Task IndexDocumentAsync(T document)
        {
            await _client.IndexDocumentAsync(document);
        }
    }

}
