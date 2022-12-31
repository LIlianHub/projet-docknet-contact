using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;


namespace ballejos
{
    internal class SerializeBinary: ISerialize
    {

        public static void Serialize(DataStructure myds, String name, String key)
        {
            // Nom d'utilisateur dans  la case 1 (dans la case 0 nom du pc)
            String[] formatedName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\');

            // Nom formatté de cette manière: dsBin.nomUtilisateur.nomRepertoire.dat
            String fileName = "dsBin-" + formatedName[1] + "-" + name + ".dat";

            // On ouvre un stream pour écrire dans le fichier et enregistrer celui-ci dans "Mes Documents"
            FileStream fs = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName), FileMode.Create);
            
            BinaryFormatter formatter = new BinaryFormatter();

            // On essaye d'écrire dans le fichier
            try
            {
                // On chiffre en écrivant dans le stream
                DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();
                cryptic.Key = ASCIIEncoding.ASCII.GetBytes(key);
                cryptic.IV = ASCIIEncoding.ASCII.GetBytes(key);
                CryptoStream crStream = new CryptoStream(fs, cryptic.CreateEncryptor(), CryptoStreamMode.Write);

                // On enregistre notre objet
                formatter.Serialize(crStream, myds);
                
                crStream.Close();
            }

            // Exception levée si la clef n'est pas bien formattée
            catch (ArgumentException)
            {
                Console.WriteLine("La clef doit faire 8 caractères");
            }
            // Exception levée si erreur dans l'écriture (imprévue)
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


            // Nom d'utilisateur idem que précédement
            String[] formatedName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\');
            String fileName = "dsBin-" + formatedName[1] + "-" + name + ".dat";
            
            try
            {
                // On essaye d'ouvrir le fichier demandé
                FileStream fs = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName), FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                
                // On essaye de déchiffrer le flux
                DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();
                cryptic.Key = ASCIIEncoding.ASCII.GetBytes(key);
                cryptic.IV = ASCIIEncoding.ASCII.GetBytes(key);
                CryptoStream crStream = new CryptoStream(fs,cryptic.CreateDecryptor(), CryptoStreamMode.Read);

                // Lecture
                myds = (DataStructure)formatter.Deserialize(crStream);
                
                crStream.Close();
                fs.Close();
            }
            // Exception levée car le fichier existe pas (erreur lors de fs)
            catch (FileNotFoundException)
            {
                Console.WriteLine("Fichier Inconnu ou corrompu !");
            }
            // Exception levée car la clef n'est pas celle qu'il faut pour déchiffrer
            catch (SerializationException)
            {
                Console.WriteLine("Erreur lors du déchiffrage");
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
