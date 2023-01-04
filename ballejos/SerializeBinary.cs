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
        public SerializeBinary()
        {
        }

        public void Serialize(DataStructure myds, String name, byte[] key)
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
                AesCryptoServiceProvider AES = new AesCryptoServiceProvider();
                AES.Key = SHA256Managed.Create().ComputeHash(key);
                AES.IV = MD5.Create().ComputeHash(key);

                // On enregistre notre objet
                using (CryptoStream crStream = new CryptoStream(fs, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    // On essaye de lire le fichier
                    formatter.Serialize(crStream, myds);
                }

                Console.WriteLine("Sauvegarde effectuée sous le nom {0}", fileName);
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
                // On ferme le fichier quoi qu'il arrive
                fs.Close();
            }
        }

        public DataStructure Deserialize(String name, byte[] key)
        {
            DataStructure myds = null;


            // Nom d'utilisateur idem que précédement
            String[] formatedName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\');
            String fileName = "dsBin-" + formatedName[1] + "-" + name + ".dat";

            // 
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                // On essaye d'ouvrir le fichier demandé
                using (FileStream fs = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName), FileMode.Open))
                {
                    // On essaye de déchiffrer le flux
                    AesCryptoServiceProvider AES = new AesCryptoServiceProvider();
                    AES.Key = SHA256Managed.Create().ComputeHash(key);
                    AES.IV = MD5.Create().ComputeHash(key);

                    using (CryptoStream crStream = new CryptoStream(fs, AES.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        // On essaye de lire le fichier
                        myds = formatter.Deserialize(crStream) as DataStructure;
                    }
                    Console.WriteLine("Chargement effectué du fichier {0}", fileName);
                }
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
