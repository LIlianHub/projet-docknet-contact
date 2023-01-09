using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ballejos
{
    [Serializable]
    public class DataStructure
    {
        // La racine de l'arborecence
        public Dossier Root;
        
        public DataStructure()
        {
            Root = new Dossier("Root");
        }

        public override string ToString()
        {
            // On va parcourir l'arborescence et afficher chaque dossier en partant de la racine
            return Root.ToString(0);
        }

        public void RepairFoldersAfterSerialization()
        {
            // On va parcourir l'arborescence et réparer les liens entre les dossiers
            Root.RepairFolderParent();
        }

    }
}
