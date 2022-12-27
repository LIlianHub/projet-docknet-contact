using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ballejos
{
    [Serializable]
    public class Dossier : DataObject
    {
        public String Name;
        public DateTime CreationDate;
        public DateTime LastUpdate;
        public List<DataObject> Children;
        public Dossier Parent;

        public Dossier() { }

        public Dossier(String name)
        {
            Name = name;
            CreationDate = DateTime.Now;
            LastUpdate = DateTime.Now;
            Children = new List<DataObject>();
            Parent = null;
        }
        
        public Dossier(String name, Dossier pere) : this(name)
        {
            Parent = pere;
        }
        
        public override string ToString(int space)
        {
            string retour = "|";
            
            for (int i = 0; i < space; i++)
            {
                retour += "_";

            }
            
            retour +=  "[D] " + Name + " (" + CreationDate + ") (" + LastUpdate + ")\n";
            foreach (DataObject child in Children)
            {
                retour += child.ToString(space + 4);
            }

            return retour;
        }

        public void AddElementInFolder(DataObject element)
        {
            Children.Add(element);
            LastUpdate = DateTime.Now;
        }

        
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

        public Dossier goBackFolder()
        {
            if (Parent != null)
            {
                return Parent;
            }
            return this;
        }


    }
}
