using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ballejos
{
    internal class SerializeBinary: ISerialize
    {

        public static void Serialize(DataStructure myds, String name, String key)
        {
            //nom dans la case 1
            String[] formatedName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\');
            
            String fileName = "dsBin-" + formatedName[1] + "-" + name + ".dat";
            FileStream fs = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName), FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();
                cryptic.Key = ASCIIEncoding.ASCII.GetBytes(key);
                cryptic.IV = ASCIIEncoding.ASCII.GetBytes(key);
                CryptoStream crStream = new CryptoStream(fs, cryptic.CreateEncryptor(), CryptoStreamMode.Write);
                formatter.Serialize(crStream, myds);
                crStream.Close();
            }
            catch (ArgumentException)
            {
                Console.WriteLine("La clef doit faire 8 caractères");
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

        public static DataStructure Deserialize(String name, String key)
        {
            DataStructure myds = null;

            String[] formatedName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\');
            String fileName = "dsBin-" + formatedName[1] + "-" + name + ".dat";
            try
            {
                FileStream fs = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName), FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();
                cryptic.Key = ASCIIEncoding.ASCII.GetBytes(key);
                cryptic.IV = ASCIIEncoding.ASCII.GetBytes(key);
                CryptoStream crStream = new CryptoStream(fs,cryptic.CreateDecryptor(), CryptoStreamMode.Read);
                myds = (DataStructure)formatter.Deserialize(crStream);
                crStream.Close();
                fs.Close();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Fichier Inconnu ou corrompu !");
            }
            catch (SerializationException)
            {
                Console.WriteLine("Erreur lors du déchiffrage");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("La clef doit faire 8 caractères");
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur innatendu: {0}", e);
                throw;
            }
            return myds;
        }

    }
}
