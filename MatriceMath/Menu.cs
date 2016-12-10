using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;

namespace MatriceMath
{
    class Menu
    {
        #region Variables
        private string cheminFichierDeBase;
        private string cheminNouveauFichier;
        private string nomFichierDeBase;
        private string nomNouveauFichier;
        #endregion

        #region Affichages
        public void MenuRedirection(string nomFich) {
            try
            {
                int choix = -1;
                do
                {
                    choix = AfficherMenu();
                    nomFichierDeBase = nomFich;
                    cheminFichierDeBase = System.IO.Directory.GetCurrentDirectory();
                    CheminNouveauFichier = cheminFichierDeBase + "\\..\\MatriceInversees";
                    if (choix < 4 && choix > 0)
                    {
                        switch (choix)
                        {
                            case 1: break;
                            case 2: GetContenuRepertoireactuel(); break;
                            case 3: ConstruireNouvelleMatrice(); break;

                        }
                        // Le nouveau fichier -> l'ancien - .txt + Resolu.txt
                        nomNouveauFichier = nomFichierDeBase.Substring(0, nomFichierDeBase.Length - 4) + "Resolu.txt";
                        AppelAlgoLU();
                        System.Console.ReadKey();
                        if (AfficherMenuRecommencer() == 1) { MenuRedirection(nomFich); }
                        
                    }
                    else { Console.Clear(); }
                } while (choix > 3 || choix < 0);
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        public int AfficherMenu() {
            try
            {
                Console.Clear();
                Console.WriteLine("Menu.");
                Console.WriteLine();
                Console.WriteLine("1. Matrice par défaut");
                Console.WriteLine("2. Choisir parmi les matrices existantes (.txt uniquement)");
                Console.WriteLine("3. Matrice entrée par l'utilisateur");
                Console.WriteLine("0. Quitter");
                Console.WriteLine("Choix : ");

                return Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception) { throw; }
        }

        public int AfficherMenuRecommencer()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("Voulez-vous recommencer ? ");
                Console.WriteLine();
                Console.WriteLine("1. Oui");
                Console.WriteLine("2. Non");
                Console.WriteLine("Choix : ");
                return Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception) { throw; }
        }
        public void AppelAlgoLU() {
            Console.Clear();
            try
            {

                FichierMatrice FM = new FichierMatrice(CheminFichierDeBase, NomFichierDeBase, CheminNouveauFichier, NomNouveauFichier);
                // Lecture du fichier, initialisation de la matrice.
                Matrice matriceDepart = FM.ReadFile();
                //On calcule LU
                matriceDepart.ResolutionInversionMatriceLU();
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Dimension de la matrice incorrecte.");
                Console.ReadLine();
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine("Fichier inexistant...");
                Console.ReadLine();
            }
        }

        public void ConstruireNouvelleMatrice()
        {
            try
            {
                string precision;
                // La personne entre le nom du fichier et on enregistre la nouvelle matrice à l'intérieur de celle-ci
                Console.WriteLine("Entrez le nom de votre fichier : ");
                string nomFichier = Console.ReadLine();
                // On retourne le chemin pour qu'on puisse lire le fichier
                 
                nomFichierDeBase = nomFichier + ".txt";
                // On supprime le fichier existant si les noms sont les mêmes
                if (File.Exists(cheminFichierDeBase)) { File.Delete(cheminFichierDeBase); }
                FichierMatrice fichier = new FichierMatrice(cheminFichierDeBase, nomFichierDeBase);
                
                Console.Clear();

                Console.WriteLine("#La dimension : ");
                // La dimension ne peut pas être un double
                int dimension = Convert.ToInt32(VerifierNombreEntre());

                // Vérification
                Console.WriteLine("#Precision : ");
                fichier.WriteFile("#Precision : ");
                string preci = Console.ReadLine();

                if (verifierRegex(preci, new Regex(@"^[0-9]+$"))) { precision = preci; }
                else { throw new Exception("La précision est incorrecte. Uniquement des entiers sont acceptés."); }
                fichier.WriteFile(precision);
                // On lui ajoute la dimension comme param
                string[] tabMatrice = new string[dimension];
                string elemInsertDansMatrice;
                Console.WriteLine("#Matrice : ");
                fichier.WriteFile("#Matrice : ");
                for (int i = 0; i < dimension; i++)
                {
                    for (int j = 0; j < dimension; j++) 
                    {
                        // La ligne commence à 5
                        Console.Write("#[" + (i + 1) + "][" + (j + 1) + "] <~ ");
                        // On inscrit | si ça n'est pas le dernier élément de la ligne
                        // Vérification
                        elemInsertDansMatrice = Console.ReadLine();
                        if (verifierRegex(elemInsertDansMatrice, new Regex(@"^[0-9-]{0,10}([,][0-9]{0,10})?$"))) { tabMatrice[i] += elemInsertDansMatrice; }
                        else { throw new Exception("Uniquement une valeur comprise entre 0 et 9 SVP."); }
                        if (j != dimension - 1)
                            tabMatrice[i] += "|";
                    }
                    fichier.WriteFile(tabMatrice[i]);
                }
            }
            catch (Exception) { throw; }
        }

        // Récupère tous les fichiers .txt existants dans ce dossier.
        public void GetContenuRepertoireactuel() {
            try
            {
                System.Console.Clear();
                List<string> repertoire = new List<String>();
                //string chemin = System.IO.Directory.GetCurrentDirectory();
                // Accède au dossier ou se trouve les .txt
                DirectoryInfo d = new DirectoryInfo(CheminFichierDeBase);//Assuming Test is your Folder
                FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files
                int i = 1;
                foreach (FileInfo file in Files)
                {
                    Console.WriteLine("[" + i + "]" + file.Name);
                    // Contient la liste de tous les éléments .txt trouvés.
                    repertoire.Add(file.Name);
                    i++;
                }
                Console.WriteLine("Quel fichier voulez-vous charger (entrez son n°) : ");
                // On rentre le nom de fichier contenu dans "repertoire" à l'indice choisi par l'utilisateur
                nomFichierDeBase = repertoire.ElementAt(Convert.ToInt32(Console.ReadLine()) - 1);
                //cheminFichierDeBase = chemin;
            }
            catch(Exception) { throw; }
        }

       /* public void getNomfichierDestination() {
            try
            {
                nomNouveauFichier = nomFichierDeBase.Substring(0, nomFichierDeBase.Length -4) + "Resolu.txt";
                // On écrase l'ancien fichier pour ne pas écrire à la suite.
                if (File.Exists(CheminNouveauFichier)) { File.Delete(CheminNouveauFichier); }
            }
            catch (Exception) { throw; }
        }*/

        #endregion

        #region verifications
        //The ^ will anchor the beginning of the string, the $ will anchor the end of the string
        //and the + will match one or more of what precedes it (a number in this case).
        public bool verifierRegex(string rep, Regex regex) => regex.IsMatch(rep);

        public double VerifierNombreEntre() {
            double nbrReturn;
            try
            {
                do
                {
                    string nbr = Console.ReadLine();
                    bool isDouble = Double.TryParse(nbr, out nbrReturn);
                    if (isDouble) { return nbrReturn; }
                    else { Console.WriteLine("La valeur est incorrecte."); }
                } while (true);
            }
            catch (Exception) { throw new Exception("La valeur est incorrecte."); }               
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
