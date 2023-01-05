using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ballejos
{
    internal class Program
    {
        // Pour l'affichage: où on est dans l'aborescence
        private static String localisation = "Root/";
        static void Main(string[] args)
        {
            bool exit = false;

            // Gestion des Inputs
            string input;
            string[] inputElement;

            // Notre instance de Stucture de donnée
            DataStructure dataStructure = new DataStructure();
            // Notre Dossier actuel
            Dossier currentFolder = dataStructure.Root;

            // Boucle Principale adaptable pour du graphique
            while (!exit)
            {
                // Quand l'utilisateur ecrit c'est vert
                Console.ForegroundColor = ConsoleColor.Green;
                
                // Affichage de l'interface et récupération de l'entrée utilisateur
                Console.Write("{0}~ ", localisation);
                input = Console.ReadLine();
                inputElement = input.Split(' ');

                // Quand le programme repond c'est blanc
                Console.ForegroundColor = ConsoleColor.White;

                switch (inputElement[0])
                {
                    // Fin de boucle
                    case "sortir":
                        exit = true;
                        break;

                    // Affichage
                    case "afficher":
                        Afficher(dataStructure);
                        break;

                    // Charge un fichier binaire: charger nomFichier Clef(optionnelle)
                    case "chargerBinaire":
                        dataStructure = Charger(inputElement, dataStructure, SerializationFactory.GetSerializer("Binary"));
                        currentFolder = dataStructure.Root;
                        break;

                    // Charge un fichier XML: charger nomFichier
                    case "chargerXML":
                        dataStructure = Charger(inputElement, dataStructure, SerializationFactory.GetSerializer("XML"));
                        currentFolder = dataStructure.Root;
                        break;

                    // Enregistre la structure de donnée en Binaire: enregistrer nomFichier Clef(optionnelle)
                    case "enregistrerBinaire":
                        Enregistrer(inputElement, dataStructure, SerializationFactory.GetSerializer("Binary"));
                        break;

                    // Enregistre la structure de donnée en XML: enregistrer nomFichier
                    case "enregistrerXML":
                        Enregistrer(inputElement, dataStructure, SerializationFactory.GetSerializer("XML"));
                        break;

                    // Créer un dossier à l'emplacement courant: ajouterdossier nomDossier
                    case "ajouterdossier":
                        AjouterDossier(inputElement, currentFolder);
                        break;

                    // Créer un contact à l'emplacement courant: ajoutercontact Nom Prenom Email Lien
                    case "ajoutercontact":
                        AjouterContact(inputElement, currentFolder);
                        break;

                    // Va dans le dossier parent
                    case "reculer":
                        currentFolder = Reculer(currentFolder);
                        break;

                    // Va dans le dossier fils: avancer nomDossier
                    case "avancer":
                        currentFolder = Avancer(inputElement, currentFolder);
                        break;

                    // Mauvaise entrée
                    default:
                        Console.WriteLine("Instruction inconnue.");
                        break;
                }
            }
        }

        /***********************************************************************************/
        /* Fonction statique appelée par la boucle principale: cela permet si l'on décide  */
        /* de passer en graphique de préserver leur comportement. On a juste a les appeler */
        /* autre part ! Elles sont indépendantes de la boucle au dessus !                  */
        /***********************************************************************************/

        // Affiche la structure de donnée
        private static void Afficher(DataStructure ds)
        {
            Console.WriteLine(ds.ToString());
        }

        // Gère l'entrée utilisateur pour charger une structure selon le serializer voulu
        private static DataStructure Charger(String[] inputElement, DataStructure actual, ISerialize serializer)
        {
            DataStructure tool2 = null;
            // Pas de clef donnée
            if (inputElement.Length == 2)
            {
                // On charge en déchiffrant à l'aide du SID
                tool2 = serializer.Deserialize(inputElement[1], FormatKeyFromUser());
            }

            // Clef donnée et valide
            else if(inputElement.Length == 3)
            {
                // On charge en déchiffrant avec la clef donnée
                tool2 = serializer.Deserialize(inputElement[1], FormatKeyFromString(inputElement[2]));
            }
            else
                Console.WriteLine("Erreur de syntaxe: chargerBinaire nomFichier Clef(optionnelle)");

            // Si il n'y a pas eu de soucis on change
            if (tool2 != null)
            {
                return tool2;
            }
            // Sinon on reste comme avant
            else
            {
                return actual;
            }
        }

        // Gère l'entrée utilisateur pour enregistrer la structure selon le serializer voulu
        private static void Enregistrer(String[] inputElement, DataStructure actual, ISerialize serializer)
        {
            // Pas de clef donnée
            if (inputElement.Length == 2)
            {
                serializer.Serialize(actual, inputElement[1], FormatKeyFromUser());
            }
            // Clef donnée et valide
            else if(inputElement.Length == 3)
            {
                serializer.Serialize(actual, inputElement[1], FormatKeyFromString(inputElement[2]));
            }
            else
                Console.WriteLine("Erreur de syntaxe: enregistrerBinaire nomFichier Clef(optionnelle)");
        }

        // Gère l'entrée utilisateur pour ajouter un dossier
        private static void AjouterDossier(String[] inputElement, Dossier currentFolder)
        {
            if (inputElement.Length == 2)
                currentFolder.AddElementInFolder(new Dossier(inputElement[1], currentFolder));
            else
                Console.WriteLine("Erreur de syntaxe: ajouterdossier nomDossier");
        }

        // Gère l'entrée utilisateur pour ajouter un contact
        private static void AjouterContact(String[] inputElement, Dossier currentFolder)
        {
            if (inputElement.Length == 5)
            {
                // On teste la validité du Mail
                MailAddress mailAddress = null;
                try
                {
                    mailAddress = new MailAddress(inputElement[3]);
                }
                catch(FormatException)
                {
                    Console.WriteLine("Mail mal formatté !"); ;
                }
                // Si l'email est valide on enregistre le contact !
                if (mailAddress != null)
                {
                    currentFolder.AddElementInFolder(new Contact(inputElement[1], inputElement[2], inputElement[3], inputElement[4]));
                }
            }
            else
                Console.WriteLine("Erreur de syntaxe: ajoutercontact nom prenom mail lien");
        }

        // Gère l'entrée utilisateur pour aller dans un dossier
        private static Dossier Avancer(String[] inputElement, Dossier currentFolder)
        {
            if (inputElement.Length == 2)
            {
                Dossier tool = currentFolder.goToFolder(inputElement[1]);
                
                // Si on a réussit à avancer on met à jour la position
                if (tool != null)
                {
                    localisation += inputElement[1] + "/";
                    return tool;
                }
                else
                {
                    Console.WriteLine("Le dossier n'existe pas");
                }
            }
            else
                Console.WriteLine("Erreur de syntaxe: avancer nomDossier");

            return currentFolder;
        }

        // Gère l'entrée utilisateur pour revenir en arrière
        private static Dossier Reculer(Dossier currentFolder)
        {
            // Si on est pas à la racine on met à jour sinon non
            if (currentFolder.Parent != null)
            {
                String[] buffer = localisation.Split('/');
                // je supprime l'avant dernier element, c'est à dire le dossier qu'on vient de quitter 
                buffer = buffer.Where((source, index) => index != (buffer.Length - 2)).ToArray();
                localisation = String.Join("/", buffer);
            }
            currentFolder = currentFolder.goBackFolder();
            return currentFolder;
        }

        // Créé un tableau de byte à partir du SID de l'utilisateur
        private static byte[] FormatKeyFromUser()
        {
            // Utilisation du chiffrage AES donc on crée une clef a partir du SID
            UnicodeEncoding UE = new UnicodeEncoding();
            return UE.GetBytes(WindowsIdentity.GetCurrent().User.ToString());
        }

        // Créé un tableau de byte à partir d'une chaine de caractère
        private static byte[] FormatKeyFromString(String key)
        {
            // Utilisation du chiffrage AES donc on crée une clef a partir de la clef en String
            UnicodeEncoding UE = new UnicodeEncoding();
            return UE.GetBytes(key);
        }

    }
}
