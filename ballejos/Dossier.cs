using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ballejos
{
    [Serializable]
    public class Dossier : DataObject
    {
        // Attribut nécessaire
        public String Name;
        public DateTime CreationDate;
        public DateTime LastUpdate;

        // Arborésence
        public List<DataObject> Children;

        //Ignoré pendant la sérialisation XML car sinon on a une boucle non prit en compte par le sérialiseur
        [XmlIgnore]
        public Dossier Parent;

        public Dossier() { }

        // Sans le père précisé
        public Dossier(String name)
        {
            Name = name;
            CreationDate = DateTime.Now;
            LastUpdate = DateTime.Now;
            Children = new List<DataObject>();
            Parent = null;
        }

        // Avec le père précisé
        public Dossier(String name, Dossier pere) : this(name)
        {
            Parent = pere;
        }

        // On appelle en cascade les méthodes ToString de chaque enfant
        public override string ToString(int space)
        {
            // Plus on va profond plus on met des espaces pour l'affichage
            string retour = "|";
            
            for (int i = 0; i < space; i++)
            {
                retour += "_";

            }
            
            // On ecrit les infos de l'élement courant
            retour +=  "[D] " + Name + " (" + CreationDate + ") (" + LastUpdate + ")\n";

            // On appelle les méthodes ToString de chaque enfant
            foreach (DataObject child in Children)
            {
                retour += child.ToString(space + 4);
            }

            return retour;
        }

        // On ajoute un élement à la liste des enfants du dossier
        public void AddElementInFolder(DataObject element)
        {
            Children.Add(element);
            LastUpdate = DateTime.Now;
        }

        // Si le nom du dossier à visiter existe dans la liste des enfants on renvoie l'instance de ce dossier, sinon on renvoit null
        public Dossier goToFolder(String name)
        {
            foreach (DataObject element in Children)
            {
                if (element is Dossier)
                {
                    Dossier folder = (Dossier)element;
                    if (folder.Name == name)
                    {
                        return folder;
                    }
                }
            }
            return null;

        }
        // On donne l'instance du dossier parent ou notre propre instance si c'est la racine
        public Dossier goBackFolder()
        {
            if (Parent != null)
            {
                return Parent;
            }
            return this;
        }
        
        public void RepairFolderParent()
        {
            foreach (DataObject element in Children)
            {
                if (element is Dossier)
                {
                    Dossier folder = (Dossier)element;
                    folder.Parent = this;
                    folder.RepairFolderParent();
                }
            }
        }


    }
}
