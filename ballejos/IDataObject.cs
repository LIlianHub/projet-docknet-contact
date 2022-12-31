using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ballejos
{   // La classe DataObject hérite de l'interface IDataObject l'obligeant à avoir une méthode ToString particulière
    public interface IDataObject
    {
        string ToString(int space);
    }
}
