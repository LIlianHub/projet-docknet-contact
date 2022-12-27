using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ballejos
{
    [Serializable]
    public class DataStructure
    {
        public Dossier Root;
        
        public DataStructure()
        {
            Root = new Dossier("Root");
        }

 

        public override string ToString()
        {
            return Root.ToString(0);
        }

    }
}
