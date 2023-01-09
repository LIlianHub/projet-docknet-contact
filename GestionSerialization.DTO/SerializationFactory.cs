using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ballejos
{
    public class SerializationFactory
    {

        // Retourne le sérialiseur désiré
        public static ISerialize GetSerializer(String type)
        {
            switch (type)
            {
                case "XML":
                    return new SerializeXML();
                case "Binary":
                    return new SerializeBinary();
                default:
                    return null;
            }
        }
    }
}
