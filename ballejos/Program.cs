using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ballejos
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool exit = false;

            string input;
            string[] inputElement;

            DataStructure dataStructure = new DataStructure();
            Dossier currentFolder = dataStructure.Root;

            FormatKeyFromUser();

            while (!exit)
            {
                Console.Write("Votre Choix: ");
                input = Console.ReadLine();
                inputElement = input.Split(' ');
                switch (inputElement[0])
                {
                    case "sortir":
                        exit = true;
                        break;
                    case "afficher":
                        Afficher(dataStructure);
                        break;
                    case "charger":
                        dataStructure = Charger(inputElement, dataStructure);
                        currentFolder = dataStructure.Root;
                        break;
                    case "enregistrer":
                        Enregistrer(inputElement, dataStructure);
                        break;
                    case "ajouterdossier":
                        AjouterDossier(inputElement, currentFolder);
                        break;
                    case "ajoutercontact":
                        AjouterContact(inputElement, currentFolder);
                        break;
                    case "reculer":
                        currentFolder = currentFolder.goBackFolder();
                        break;
                    case "avancer":
                        currentFolder = Avancer(inputElement, currentFolder);
                        break;
                    default:
                        Console.WriteLine("Instruction inconnue.\n");
                        break;
                }
            }
        }
        
        private static void Afficher(DataStructure ds)
        {
            Console.WriteLine(ds.ToString());
        }

        private static DataStructure Charger(string[] inputElement, DataStructure actual)
        {
            DataStructure tool2 = null;
            if (inputElement.Length == 2)
            {
                tool2 = SerializeBinary.Deserialize(inputElement[1], FormatKeyFromUser());
            }
            else if(inputElement.Length == 3)
            {
                tool2 = SerializeBinary.Deserialize(inputElement[1], inputElement[2]);
            }
            else
                Console.WriteLine("Erreur de syntaxe");
            
            if (tool2 != null)
            {
                return tool2;
            }
            else
            {
                return actual;
            }
            
        }

        private static void Enregistrer(string[] inputElement, DataStructure actual)
        {
            if (inputElement.Length == 2)
            {
                SerializeBinary.Serialize(actual, inputElement[1], FormatKeyFromUser());
            }
            else if(inputElement.Length == 3)
            {
                SerializeBinary.Serialize(actual, inputElement[1], inputElement[2]);
            }
            else
                Console.WriteLine("Erreur de syntaxe");
        }

        private static void AjouterDossier(string[] inputElement, Dossier currentFolder)
        {
            if (inputElement.Length == 2)
                currentFolder.AddElementInFolder(new Dossier(inputElement[1], currentFolder));
            else
                Console.WriteLine("Erreur de syntaxe");
        }

        private static void AjouterContact(string[] inputElement, Dossier currentFolder)
        {
            if (inputElement.Length == 5)
                currentFolder.AddElementInFolder(new Contact(inputElement[1], inputElement[2], inputElement[3], inputElement[4]));
            else
                Console.WriteLine("Erreur de syntaxe");
        }

        private static Dossier Avancer(string[] inputElement, Dossier currentFolder)
        {
            if (inputElement.Length == 2)
            {
                Dossier tool = currentFolder.goToFolder(inputElement[1]);
                
                
                if (tool != null)
                {
                    return tool;
                }
                else
                {
                    Console.WriteLine("Le dossier n'existe pas");
                }
            }
            else
                Console.WriteLine("Erreur de syntaxe");

            return currentFolder;
        }

        private static String FormatKeyFromUser()
        {
            String key = WindowsIdentity.GetCurrent().User.ToString().Replace("-", "").Substring(0, 8);
            return key;
        }

    }
}
