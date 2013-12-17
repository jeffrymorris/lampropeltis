using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Couchbase
{
    public interface IBucket
    {
        string Name { get; }

        string Username { get; }

        string Password { get; }
    }
}
