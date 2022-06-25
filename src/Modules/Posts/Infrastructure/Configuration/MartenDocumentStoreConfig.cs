using DevMikroblog.Modules.Posts.Infrastructure.Model;

using Marten;

using Microsoft.Extensions.Configuration;

using Weasel.Core;

namespace DevMikroblog.Modules.Posts.Infrastructure.Configuration;

internal static class MartenDocumentStoreConfig
{
    public static Action<StoreOptions> Configure(string connectionString, bool isDev)
    {
        ArgumentNullException.ThrowIfNull(connectionString, nameof(connectionString));
        return options =>
        {
            options.Connection(connectionString);
            if (isDev)
            {
                options.AutoCreateSchemaObjects = AutoCreate.All;
            }
            options.Schema.For<MartenPost>().Index(x => x.CreatedAt).Index(x => x.AuthorId);
        };
    }
}