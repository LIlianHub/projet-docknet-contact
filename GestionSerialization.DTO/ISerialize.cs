using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ballejos
{
    public interface ISerialize
    {
        void Serialize(DataStructure myds, String name, byte[] key);
        DataStructure Deserialize(String name, byte[] key);
    }
}
