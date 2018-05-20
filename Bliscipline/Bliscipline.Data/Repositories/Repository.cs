using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Bliscipline.Data.Repositories
{
    public abstract class Repository
    {
        protected readonly Func<IDbConnection> OpenConnection;

        protected Repository(Func<IDbConnection> openConnection)
        {
            OpenConnection = openConnection;
        }
    }
}
