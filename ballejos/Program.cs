﻿using System;
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
                // Affichage de l'interface et récupération de l'entrée utilisateur
                Console.Write("{0}~ ", localisation);
                input = Console.ReadLine();
                inputElement = input.Split(' ');

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
                        dataStructure = ChargerBinaire(inputElement, dataStructure);
                        currentFolder = dataStructure.Root;
                        break;

                    // Charge un fichier XML: charger nomFichier
                    case "chargerXML":
                        dataStructure = ChargerXML(inputElement, dataStructure);
                        currentFolder = dataStructure.Root;
                        break;

                    // Enregistre la structure de donnée en Binaire: enregistrer nomFichier Clef(optionnelle)
                    case "enregistrerBinaire":
                        EnregistrerBinaire(inputElement, dataStructure);
                        break;

                    // Enregistre la structure de donnée en XML: enregistrer nomFichier
                    case "enregistrerXML":
                        EnregistrerXML(inputElement, dataStructure);
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
                        Console.WriteLine("Instruction inconnue.\n");
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

        private static DataStructure ChargerBinaire(String[] inputElement, DataStructure actual)
        {
            DataStructure tool2 = null;
            // Pas de clef donnée
            if (inputElement.Length == 2)
            {
                // On charge en déchiffrant à l'aide du SID
                tool2 = SerializeBinary.Deserialize(inputElement[1], FormatKeyFromUser());
            }

            // Clef donnée
            else if(inputElement.Length == 3)
            {
                // On charge en déchiffrant avec la clef donnée
                tool2 = SerializeBinary.Deserialize(inputElement[1], inputElement[2]);
            }
            else
                Console.WriteLine("Erreur de syntaxe");
            
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

        private static DataStructure ChargerXML(String[] inputElement, DataStructure actual)
        {
            DataStructure tool2 = null;
            // Bon nombre d'argument
            if (inputElement.Length == 2)
            {
                // On charge le fichier avec le nom donné
                tool2 = SerializeXML.Deserialize(inputElement[1]);
            }

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

        private static void EnregistrerBinaire(String[] inputElement, DataStructure actual)
        {
            // Pas de clef donnée
            if (inputElement.Length == 2)
            {
                // On enregistre en chiffrant à l'aide du SID
                SerializeBinary.Serialize(actual, inputElement[1], FormatKeyFromUser());
            }
            // Clef donnée et valide
            else if(inputElement.Length == 3 && ValidKey(inputElement[2]))
            {
                // On enregistre en chiffrant avec la clef donnée
                SerializeBinary.Serialize(actual, inputElement[1], inputElement[2]);
            }
            else
                Console.WriteLine("Erreur de syntaxe");
        }

        private static void EnregistrerXML(String[] inputElement, DataStructure actual)
        {
            // Bon nombre d'argument
            if (inputElement.Length == 2)
            {
                // On enregistre en XML
                SerializeXML.Serialize(actual, inputElement[1]);
            }
            else
                Console.WriteLine("Erreur de syntaxe");
        }

        private static void AjouterDossier(String[] inputElement, Dossier currentFolder)
        {
            if (inputElement.Length == 2)
                currentFolder.AddElementInFolder(new Dossier(inputElement[1], currentFolder));
            else
                Console.WriteLine("Erreur de syntaxe");
        }

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
                Console.WriteLine("Erreur de syntaxe");
        }

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
                Console.WriteLine("Erreur de syntaxe");

            return currentFolder;
        }


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

        private static String FormatKeyFromUser()
        {
            // Utilisation du chiffrage DES donc on recupère les 8 premiers caractères du SID
            String key = WindowsIdentity.GetCurrent().User.ToString().Replace("-", "").Substring(0, 8);
            return key;
        }

        private static bool ValidKey(String key)
        {
            try
            {
                DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();
                cryptic.Key = ASCIIEncoding.ASCII.GetBytes(key);
                cryptic.IV = ASCIIEncoding.ASCII.GetBytes(key);
            }
            catch (Exception)
            {
                Console.Write("Clef invalide, elle doit faire 8 caractères: ");
                return false;
            }
            return true;
        }

    }
}
