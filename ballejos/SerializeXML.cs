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
using System.Security.Cryptography;

namespace ballejos
{
    internal class SerializeXML: ISerialize
    {
        public SerializeXML()
        {
        }
        
        public void Serialize(DataStructure myds, String name, byte[] key)
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
                // On chiffre en écrivant dans le stream
                AesCryptoServiceProvider AES = new AesCryptoServiceProvider();
                AES.Key = SHA256Managed.Create().ComputeHash(key);
                AES.IV = MD5.Create().ComputeHash(key);

                // On enregistre notre objet
                using (CryptoStream crStream = new CryptoStream(fs, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    // On essaye d'écrire dans le fichier
                    XmlWriter writer = new XmlTextWriter(crStream, Encoding.Unicode);
                    serializer.Serialize(writer, myds);
                }

                Console.WriteLine("Sauvegarde effectuée sous le nom {0}", fileName);
            }
            // Exception levée si la clef n'est pas bien formattée
            catch (ArgumentException)
            {
                Console.WriteLine("La clef doit faire 8 caractères");
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

        public DataStructure Deserialize(String name, byte[] key)
        {
            DataStructure myds = null;

            // Nom d'utilisateur dans  la case 1 (dans la case 0 nom du pc)
            String[] formatedName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\');

            // Nom formatté de cette manière: dsBin.nomUtilisateur.nomRepertoire.dat
            String fileName = "dsXML-" + formatedName[1] + "-" + name + ".xml";

            // Creation d'une instance de XmlSerializer
            XmlSerializer serializer = new XmlSerializer(typeof(DataStructure), new Type[] { typeof(DataStructure), typeof(Contact) });

            try
            {
                using (FileStream fs = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName), FileMode.Open))
                {
                    // On essaye de déchiffrer le flux
                    AesCryptoServiceProvider AES = new AesCryptoServiceProvider();
                    AES.Key = SHA256Managed.Create().ComputeHash(key);
                    AES.IV = MD5.Create().ComputeHash(key);

                    using (CryptoStream crStream = new CryptoStream(fs, AES.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        // On essaye de lire le fichier
                        myds = serializer.Deserialize(crStream) as DataStructure;

                        // Si la lecture a réussi
                        if (myds != null)
                        {
                            // On répare les liens entre les dossiers
                            myds.RepairFoldersAfterSerialization();
                        }
                    }
                }
                Console.WriteLine("Chargement effectué du fichier {0}", fileName);
            }

            // Exception levée car le fichier existe pas (erreur lors de fs)
            catch (FileNotFoundException)
            {
                Console.WriteLine("Fichier Inconnu ou corrompu !");
            }
            // Exception levée car la clef n'est pas celle qu'il faut pour déchiffrer
            catch (CryptographicException)
            {
                Console.WriteLine("Erreur lors du déchiffrage: clef possiblement invalide");
            }
            // Exception levée car la clef n'est pas bien formattée
            catch (ArgumentException)
            {
                Console.WriteLine("La clef doit faire 8 caractères");
            }
            // Exception imprévue
            catch (Exception e)
            {
                Console.WriteLine("Erreur innatendu: {0}", e);
                throw;
            }

            return myds;
        }

       
    }
}
