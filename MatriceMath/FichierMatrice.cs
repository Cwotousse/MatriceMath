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
        private string cheminFichierDeBase;
        private string cheminNouveauFichier;
        private string nomFichierDeBase;
        private string nomNouveauFichier;
        private string urlDepart;
        private string urlArrivee;
        #endregion

        #region Constructeurs
        public FichierMatrice() { }
        public FichierMatrice(string cheminFichierDeBase, string nomFichierDeBase)
        {
            this.cheminFichierDeBase = cheminFichierDeBase;
            this.nomFichierDeBase = nomFichierDeBase;
            urlDepart = cheminFichierDeBase + "\\" + nomFichierDeBase;
        }

        public FichierMatrice(string urlDepart) {
            this.urlDepart = urlDepart;
        }
        public FichierMatrice(string cheminFichierDeBase, string nomFichierDeBase, string cheminNouveauFichier, string nomNouveauFichier)
        {
            this.cheminFichierDeBase    = cheminFichierDeBase;
            this.nomFichierDeBase       = nomFichierDeBase;
            this.cheminNouveauFichier   = cheminNouveauFichier;
            this.nomNouveauFichier      = nomNouveauFichier;
            urlDepart                   = cheminFichierDeBase + "\\" + nomFichierDeBase;
            urlArrivee                  = cheminNouveauFichier + "\\" + nomNouveauFichier;
        }
        #endregion

        #region Méthodes
        // Methode esthétique pour afficher un encadré
        public void afficherEncadre(int nombre, string texte)
        {
            EcritureFichierEtAffichage("┌─┐\n│"+ nombre + "│ " + texte + "\n└─┘");
        }

        // Utile juste pour réduire le nombre de lignes de code. Car ce qui doit être affiché doit aussi l'être dans le fichier.
        public void EcritureFichierEtAffichage(string elem)
        {
            try
            {
                Console.WriteLine(elem);
                WriteFile(elem + "\r\n");
            }
            catch (Exception) { throw; }
        }

        // écrit une ligne  dans le fichier résultat
        public void WriteFile(string s)
        {
            try
            {
                // Si l'url d'arrivée est null on assigne dans la valeur urlPourEcrire 
                string urlPourEcrire = urlArrivee ?? urlDepart;
                using (StreamWriter str_writer = new StreamWriter(urlPourEcrire, true))
                {
                    //Ecriture de la ligne reçue en param dans le fichier d'arrivée
                    str_writer.WriteLine(s);
                }
            }
            catch (Exception) { throw; }         
        }

        // Lecture d'un fichier txt contenant les informations permettant de créer une matrice 
        public Matrice ReadFile()
        {
            string[] termes;
            int dimensionMatrice;
            int precision;

            try
            {
                Console.Clear();
                if (File.Exists(urlArrivee)) { File.Delete(urlArrivee); }
                afficherEncadre(1, "Informations sur le fichier : ");
                // On donne l'url du fichier de départ
                FileInfo infoFichier = new FileInfo(urlDepart);
                EcritureFichierEtAffichage("#Nom du fichier         : " + nomFichierDeBase);
                EcritureFichierEtAffichage("#Emplacement            : " + infoFichier.DirectoryName);
                EcritureFichierEtAffichage("#Date de création       : " + infoFichier.CreationTime);
                EcritureFichierEtAffichage("#Date de modification   : " + infoFichier.LastWriteTime);
                EcritureFichierEtAffichage("#Poids du fichier       : " + infoFichier.Length);
                EcritureFichierEtAffichage("");

                //Read each line of the file into a string array. Each element
                // of the array is one line of the file.
                // On lit les lignes du fichier de départ
                string[] lignes = System.IO.File.ReadAllLines(urlDepart);
                // La dimension, on retire 3 car les 3 premières lignes sont utilisées pour affiches d'autres informations
                dimensionMatrice = lignes.Length - 3;
                // On récupère la précision à la ligne 1 du fichier
                precision = Convert.ToInt32(lignes[1]);

                // nouvelles matrice avec la précision et la dimension connue. Aucune pivotation.
                Matrice m = new Matrice(dimensionMatrice, precision, this, new System.Collections.ArrayList());
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
        public string CheminFichierDeBase
        {
            get { return cheminFichierDeBase; }
            set { cheminFichierDeBase = value; }
        }
        public string CheminNouveauFichier
        {
            get { return cheminNouveauFichier; }
            set { cheminNouveauFichier = value; }
        }
        public string NomFichierDeBase
        {
            get { return nomFichierDeBase; }
            set { nomFichierDeBase = value; }
        }
        public string NomNouveauFichier
        {
            get { return nomNouveauFichier; }
            set { nomNouveauFichier = value; }
        }
        #endregion
    }
}
