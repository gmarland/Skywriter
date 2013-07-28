using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywriter.Model
{
    public enum CreateUserResult
    {
        Successful,
        ErroredConnection,
        AlreadyExisting
    }
}
