using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace ballejos
{
    internal class SerializeXML
    {
        public static void Serialize(DataStructure myds)
        {
            //nom dans la case 1
            String[] formatedName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\');

            String fileName = "dsXML-" + formatedName[1] + ".xml";
            FileStream fs = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName), FileMode.Create);
            XmlSerializer serializer  = new XmlSerializer(typeof(DataStructure));
            try
            {
                XmlWriter writer = new XmlTextWriter(fs, Encoding.Unicode);
                serializer.Serialize(writer, myds);
            }
            catch (Exception e)
            {
                Console.WriteLine("Impossible de sauvegarder: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }

        public static DataStructure Deserialize()
        {
            DataStructure myds = null;

            String[] formatedName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\');
            String fileName = "dsXML-" + formatedName[1] + ".xml";
            FileStream fs = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName), FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                myds = (DataStructure)formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Impossible de charger: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
            return myds;
        }
    }
}
