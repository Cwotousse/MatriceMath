using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatriceMath
{
    class FichierMatrice
    {
        #region Variables
        private string chemin;
        private string fichierResultat;
        #endregion

        #region Constructeurs
        public FichierMatrice() { }
        public FichierMatrice(string chemin) { this.chemin = chemin; }
        public FichierMatrice(string chemin, string fichierResultat)
        {
            this.chemin = chemin;
            this.fichierResultat = fichierResultat;
        }
        #endregion

        #region test
        /*public void fonctionDeTest(string nomFich) => afficherMatrice(lireMatrice(nomFich)); 

        public double[][] lireMatrice(string nomFich)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("Lecture du fichier...");
                string path = System.IO.Directory.GetCurrentDirectory();
                path += "\\" + nomFich;
                Console.WriteLine("Le fichier contenant la matrice est : {0}", path);


                 Read each line of the file into a string array. Each element
                 of the array is one line of the file.
                string[] lignes = System.IO.File.ReadAllLines(@path);
                string[][] nbrElem = new string[(lignes.Length - 5)][];
                double[][] matrice = new double[(lignes.Length - 5)][];
                for (int i = 5; i < lignes.Length; i++)
                {
                    Console.WriteLine(lignes[i]);
                    
                    nbrElem[i-5] = lignes[i].Split('|');
                    matrice[i-5] = Array.ConvertAll(nbrElem[i-5], item => Convert.ToDouble(item));
                }

                return matrice;


                return Array.ConvertAll(nbrElem, item => (int)item);
            }
            catch (Exception) { throw; }
        }

       public void afficherMatrice(double[][] matrice) {
            try
            {
                for (int i = 0; i < matrice.Length; i++)
                {
                    for (int j = 0; j < matrice[i].Length; j++)
                    {
                        Console.Write(matrice[i][j]);
                        Console.Write("  ");
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception) { throw; }
       }*/
        #endregion

        #region Méthodes
        // Methode esthétique pour afficher un encadré
        public void afficherEncadre(int nombre, string texte)
        {
            EcritureFichierEtAffichage("");
            EcritureFichierEtAffichage("┌─┐");
            EcritureFichierEtAffichage("│" + nombre + "│ " + texte);
            EcritureFichierEtAffichage("└─┘");
            EcritureFichierEtAffichage("");
        }

        // Utile juste pour réduire le nombre de lignes de code. Car ce qui doit être affiché doit aussi l'être dans le fichier.
        public void EcritureFichierEtAffichage(string elem)
        {
            Console.WriteLine(elem);
            WriteFile(elem + "\r\n");
        }

        // écrit une ligne  dans le fichier résultat
        public void WriteFile(string s)
        {
            string fullrep;
            // Si le chemin n'est pas null on ajoute le fichier créé à la fin
            if (String.IsNullOrEmpty(Chemin)) { fullrep = Chemin + "\\" + fichierResultat; }
            else { fullrep = fichierResultat; }
            using (StreamWriter str_writer = new StreamWriter(fullrep, true))
            {
                //Ecriture de la ligne reçue en param dans le fichier
                str_writer.WriteLine(s);
            }
        }

        // Lecture d'un fichier txt contenant les informations permettant de créer une matrice 
        public Matrice ReadFile(string fichier)
        {
            string fullrep;
            string[] termes;
            int dimensionMatrice;
            int precision;

            // Si le chemin n'est pas null on ajoute le fichier créé à la fin
            if (String.IsNullOrEmpty(Chemin)) { fullrep = Chemin + "\\" + fichier; }
            else { fullrep = fichier; }
            try
            {
                Console.Clear();
                Console.WriteLine("Lecture du fichier...");
                afficherEncadre(1, "Informations sur le fichier : ");
                FileInfo infoFichier = new FileInfo(fullrep);
                EcritureFichierEtAffichage("#Nom du fichier         : " + fichier);
                EcritureFichierEtAffichage("#Emplacement            : " + infoFichier.DirectoryName);
                EcritureFichierEtAffichage("#Date de création       : " + infoFichier.CreationTime);
                EcritureFichierEtAffichage("#Date de modification   : " + infoFichier.LastWriteTime);
                EcritureFichierEtAffichage("#Poids du fichier       : " + infoFichier.Length);
                EcritureFichierEtAffichage("");

                //Read each line of the file into a string array. Each element
                // of the array is one line of the file.
                string[] lignes = System.IO.File.ReadAllLines(@fullrep);
                // La dimension, on retire 3 car les 3 premières lignes sont utilisées pour affiches d'autres informations
                dimensionMatrice = lignes.Length - 3;
                // On récupère la précision à la ligne 1 du fichier
                precision = Convert.ToInt32(lignes[1]);

                // nouvelles matrice avec la précision et la dimension connue.
                Matrice m = new Matrice(dimensionMatrice, precision);
                for (int i = 3; i < lignes.Length; i++)
                {
                    termes = lignes[i].Split('|');
                    for (int j = 0; j < dimensionMatrice; j++)
                    {
                        //Récupération des coefficients un par un.
                        m.Mat[i-3][j] = double.Parse(termes[j]);
                    }
                }

                return m;
            }
            catch (Exception) { throw; }
        }
        #endregion

        #region Propriétés
        public string Chemin
        {
            get { return chemin; }
            set { chemin = value; }
        }

        public string FichierResultat
        {
            get { return fichierResultat; }
            set { fichierResultat = value; }
        }
        #endregion
    }
}
