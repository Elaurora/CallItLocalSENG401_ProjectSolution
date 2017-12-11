using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.Database
{
    public interface IAbstractDatabase
    {
        void createDB();

        void deleteDatabase();
    }
}
