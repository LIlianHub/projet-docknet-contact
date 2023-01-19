using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
        public String Societe;
        public RelationContact Relation;
        public DateTime CreationDate;
        public DateTime LastUpdate;

        public Contact() { }

        public Contact(String lastName, String firstName, String email, String societe, RelationContact relation)
        {
            LastName = lastName;
            FistName = firstName;
            Email = email;
            Societe = societe;
            Relation = relation;
            CreationDate = DateTime.Now;
            LastUpdate = DateTime.Now;
        }

        // Transforme à partir d'une chaine de caractère une relation en RelationContact
        public static RelationContact StringToRelationContact(String relation)
        {
            RelationContact rel;
            switch (relation)
            {
                case "Ami":
                    rel = RelationContact.Ami;
                    break;
                case "Collègue":
                    rel = RelationContact.Collègue;
                    break;
                case "Réseau":
                    rel = RelationContact.Réseau;
                    break;
                default:
                    rel = RelationContact.Relation;
                    break;
            }
            return rel;
        }
        
        // On affiche les éléments de contact plus ou moins décalés sur la droite en fonction de la profondeur dans l'arborescence
        public override string ToString(int space)
        {
            string retour = "|";
            for (int i = 0; i < space; i++)
            {
                retour += "_";
            }
            retour += "[C] " + LastName + " " + FistName + " (" + Email + ") (" + Societe + ") ("+ Relation + ") (" + CreationDate + ") (" + LastUpdate + ")\n";
            return retour;
        }
    }
}

