using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ballejos
{
    [Serializable]
    public class Contact : DataObject
    {
        public String LastName;
        public String FistName;
        public String Email;
        public String Link;
        public DateTime CreationDate;
        public DateTime LastUpdate;

        public Contact() { }

        public Contact(String lastName, String firstName, String email, String link)
        {
            LastName = lastName;
            FistName = firstName;
            Email = email;
            Link = link;
            CreationDate = DateTime.Now;
            LastUpdate = DateTime.Now;
        }

        // On affiche les éléements de contact plus ou moins décalés sur la droite en fonction de la profondeur dans l'arborescence
        public override string ToString(int space)
        {
            string retour = "|";
            for (int i = 0; i < space; i++)
            {
                retour += "_";
            }
            retour += "[C] " + LastName + " " + FistName + " (" + Email + ") (" + Link + ") (" + CreationDate + ") (" + LastUpdate + ")\n";
            return retour;
        }
    }
}

