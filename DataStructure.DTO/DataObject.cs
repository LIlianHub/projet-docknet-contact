using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ballejos
{
    [Serializable]
    public class DataObject : IDataObject
    {
        //Class globale pour les objets de données, elle hérite de l'interface IDataObject
        public virtual string ToString(int space)
        {
            return "DataObject";
        }
    }
}
