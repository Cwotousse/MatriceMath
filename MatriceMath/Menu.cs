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
                    if (choix < 4 && choix > 0)
                    {
                        switch (choix)
                        {
                            case 1: break;
                            case 2: GetContenuRepertoireactuel(); break;
                            case 3: ConstruireNouvelleMatrice(); break;

                        }
                        getNomfichierDestination();
                        AppelAlgoLU();
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

        public void AppelAlgoLU() {
            Console.Clear();
            try
            {
                FichierMatrice FM = new FichierMatrice(nomFichierDeBase, cheminNouveauFichier);
                Matrice matrice;

                // Lecture du fichier
                matrice = FM.ReadFile(nomFichierDeBase);
                // On initialise le fichier avec l'instance créée plus haut
                matrice.Fichier = FM;
                //On calcule LU
                matrice.ResolutionInversionMatriceLU();
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
                string chemin = System.IO.Directory.GetCurrentDirectory();
                // La personne entre le nom du fichier et on enregistre la nouvelle matrice à l'intérieur de celle-ci
                Console.WriteLine("Entrez le nom de votre fichier : ");
                string nomFichier = Console.ReadLine();
                chemin += "\\" + nomFichier + ".txt";
                // On retourne le chemin pour qu'on puisse lire le fichier
                cheminFichierDeBase = chemin;
                nomFichierDeBase = nomFichier + ".txt";
                // On supprime le fichier existant si les noms sont les mêmes
                if (File.Exists(cheminFichierDeBase)) { File.Delete(cheminFichierDeBase); }
                FichierMatrice fichier = new FichierMatrice(nomFichierDeBase, cheminFichierDeBase);
                
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
                string chemin = System.IO.Directory.GetCurrentDirectory();
                DirectoryInfo d = new DirectoryInfo(chemin);//Assuming Test is your Folder
                FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files
                int i = 1;
                foreach (FileInfo file in Files)
                {
                    Console.WriteLine("[" + i + "]" + file.Name);
                    repertoire.Add(file.Name);
                    i++;
                }
                Console.WriteLine("Quel fichier voulez-vous charger (entrez son n°) : ");
                // On rentre le nom de fichier contenu dans "repertoire" à l'indice choisi par l'utilisateur
                //AppelAlgoLU(repertoire.ElementAt(Convert.ToInt32(Console.ReadLine()) - 1));
                cheminFichierDeBase = chemin;
                nomFichierDeBase = repertoire.ElementAt(Convert.ToInt32(Console.ReadLine()) - 1);
            }
            catch(Exception) { throw; }
        }

        public void getNomfichierDestination() {
            try
            {
                string chemin = System.IO.Directory.GetCurrentDirectory();

                // On retire le premier .txt pour l'ajouter à la fin.
                string pattern = @".txt";
                Regex rgx = new Regex(pattern);
                string nomFichierDeBaseSansTxt= rgx.Replace(nomFichierDeBase, "");
                nomNouveauFichier = nomFichierDeBaseSansTxt + "Resolu";
                chemin += "\\MatriceInversees\\" + nomNouveauFichier + ".txt";
                cheminNouveauFichier = chemin;
                // On écrase l'ancien fichier pour ne pas écrire à la suite.
                if (File.Exists(CheminNouveauFichier)) { File.Delete(CheminNouveauFichier); }

            }
            catch (Exception) { throw; }
        }

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
