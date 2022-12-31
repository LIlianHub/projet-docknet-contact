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
        public static void Serialize(DataStructure myds, String name)
        {
            // Nom d'utilisateur dans  la case 1 (dans la case 0 nom du pc)
            String[] formatedName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\');

            // Nom formatté de cette manière: dsBin.nomUtilisateur.nomRepertoire.dat
            String fileName = "dsXML-" + formatedName[1] + "-" + name + ".xml";

            // Chemin vers le dossier "Mes Documents" de l'utilisateur
            FileStream fs = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName), FileMode.Create);

            // Creation d'une instance de XmlSerializer
            XmlSerializer serializer = new XmlSerializer(typeof(DataStructure), new Type[] { typeof(DataStructure), typeof(Contact) });
            try
            {
                // On essaye d'écrire dans le fichier
                XmlWriter writer = new XmlTextWriter(fs, Encoding.Unicode);
                serializer.Serialize(writer, myds);
            }
            catch (Exception e)
            {
                // Erreur imprévue
                Console.WriteLine("Impossible de sauvegarder: " + e.Message);
                throw;
            }
            finally
            {
                // On ferme le fichier
                fs.Close();
            }
        }

        public static DataStructure Deserialize(String name)
        {
            DataStructure myds = null;

            // Nom d'utilisateur dans  la case 1 (dans la case 0 nom du pc)
            String[] formatedName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\');

            // Nom formatté de cette manière: dsBin.nomUtilisateur.nomRepertoire.dat
            String fileName = "dsXML-" + formatedName[1] + "-" + name + ".xml";
            
            try
            {
                // Chemin vers le dossier "Mes Documents" de l'utilisateur
                FileStream fs = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName), FileMode.Open);

                // Creation d'une instance de XmlSerializer
                XmlSerializer serializer = new XmlSerializer(typeof(DataStructure), new Type[] { typeof(DataStructure), typeof(Contact) });

                // On essaye de lire le fichier
                myds = serializer.Deserialize(fs) as DataStructure;

                // On ferme le fichier
                fs.Close();
            }

            // Exception levée car le fichier existe pas (erreur lors de fs)
            catch (FileNotFoundException)
            {
                Console.WriteLine("Fichier Inconnu ou corrompu !");
            }

            // Exception imprévue
            catch (Exception e)
            {
                Console.WriteLine("Impossible de charger: " + e.Message);
                throw;
            }
            
            return myds;
        }
    }
}
