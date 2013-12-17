
using System.Collections.Generic;

namespace Couchbase.Configuration.Client
{
    public interface IConfigProviderConfig
    {
        SortedList<int, ProviderSettings> Providers { get; }  
    }
}
