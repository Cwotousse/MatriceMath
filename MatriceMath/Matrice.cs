using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatriceMath
{
    class Matrice
    {
        #region Variables
        private int n;
        private int precision;
        private double[][] mat;
        private ArrayList pivotage = new ArrayList();
        private FichierMatrice fichier;
        #endregion

        #region Constructeurs
        public Matrice()
        {
            this.n          = 0;
            this.precision  = 0;
            this.mat[0]     = new double[n];
            this.pivotage   = null;
            this.fichier    = new FichierMatrice();
        }

        public Matrice(int n, int precision, FichierMatrice FM, ArrayList pivotage)
        {
            this.mat        = new double[n][];
            this.precision  = precision;
            this.n          = n;
            this.fichier    = FM;
            this.pivotage   = pivotage;
            for (int i = 0; i < n; i++) { this.mat[i] = new double[n]; }
            

        }

        public Matrice(int n, string cheminArrivee, string fichierResultat, int precision, ArrayList pivotage)
        {
            this.mat        = new double[n][];
            for (int i = 0; i < n; i++) { this.mat[i] = new double[n]; }
            this.n          = n;
            this.precision  = precision;
            this.pivotage   = pivotage;
            // Car on ne veut faire qu'écrire le résultat, pas de lecture donc pas besoin du fichier de départ
            fichier         = new FichierMatrice("", "", cheminArrivee, fichierResultat);
        }
        #endregion

        #region Méthodes
        public void ResolutionInversionMatriceLU()
        {
            try
            {
                Matrice matriceDepart = this;
                Matrice matCompactee = Gauss();
                Matrice L = matCompactee.SplitMatriceLetU('L');
                Matrice U = matCompactee.SplitMatriceLetU('U');
                // Pas besoin du déterminant de L car il vaut 1.
                double determinantA = U.Determinant(); 
                fichier.EcritureFichierEtAffichage("Déterminant de A : " + U.Determinant());
                // si le determinant de A = 0, la matrice n'est pas inversible et on ne va pas plus loin
                if (determinantA != 0) {                 
                    fichier.EcritureFichierEtAffichage("~> La matrice est inversible.");

                    // Matrice L-1
                    L.Fichier = this.fichier;
                    Matrice LInverse = L.InversionL();

                    // Matrice U-1
                    U.Fichier = this.fichier;
                    Matrice UInverse = U.InversionU();

                    // Matrice A-1
                    Matrice AInverse = new Matrice(U.N, this.precision, U.fichier, U.pivotage);
                    AInverse.Pivotage = matCompactee.Pivotage;
                    AInverse.Fichier = this.fichier;
                    AInverse.CalculerInverseA(UInverse, LInverse);
                }
                else { fichier.EcritureFichierEtAffichage("~> La matrice n'est pas inversible.\nFin du programme."); }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        // Resolution de la matrice avec Gauss.
        public Matrice Gauss()
        {
            try
            {
                Matrice U = new Matrice(n, this.fichier.CheminNouveauFichier, this.fichier.NomNouveauFichier, this.precision, this.pivotage);
                Matrice L = InitMatrice(n);
                U.Mat = this.Mat;
                // la valeur qui sera insérée dans la matrice
                double m = 0; 
                // Si le pivot est égal à true à un moment donné, alors on arrête tout ça ne sert à rien.
                bool pivotnull = false;
                //-> Affichage à l'écran
                fichier.afficherEncadre(2, "Matrice de départ :");
                fichier.EcritureFichierEtAffichage("Precision : " + precision);
                fichier.EcritureFichierEtAffichage(U.ToString());
                fichier.afficherEncadre(3, "Début de Gauss.");
                for (int k = 0; k < n-1 && !pivotnull; k++)
                {
                    // Le pivot vaut zéro => on permute
                    if (U.mat[k][k] == 0)
                    {
                        fichier.EcritureFichierEtAffichage("/!\\ Nécéssité de pivoter la ligne " + (k + 1) + "/!\\");
                        // Si la permutation n'a pas fonctionné le pivot reste nul et on ne peut pas continuer.
                        if (!PermuterMax(U, L, k))
                        {
                            fichier.EcritureFichierEtAffichage("Le pivot est null");
                            pivotnull = true;
                        }
                    }

                    for (int i = 0; i < n && !pivotnull; i++)
                    {
                        // On ne s'occupe que du triangle inférieur car nous faisons Gauss
                        if (i > k)
                        {
                            // On ne sait pas diviser par 0, donc si le dénominateur = 0 on ne change rien
                            // Le cas échéant, M vaut la matrice U
                            m = U.mat[k][k] == 0 ? 0 : U.mat[i][k] / U.mat[k][k];
                            fichier.EcritureFichierEtAffichage("m[" + (i + 1) + "][" + (k + 1) + "] = " + m);
                            // On inscrit cette partie dans la matrice L
                            L.mat[i][k] = m;
                            for (int j = k; j < n && m != 0; j++)
                            {
                                // ~> application de Gauss
                                U.mat[i][j] = U.mat[i][j] - (m * U.mat[k][j]);
                            }
                        }
                    }
                    fichier.EcritureFichierEtAffichage("\nMatrice U après l'étape " + (k + 1) + " (" + (n - k - 2) + " étapes restantes) :");
                    fichier.EcritureFichierEtAffichage(U.ToString());
                }
                fichier.afficherEncadre(4, "Fin de Gauss.");
                fichier.EcritureFichierEtAffichage("Affichage des résultats :");
                // Affiche si'l y a eu ou non des permutations
                if (U.pivotage.Count != 0) { fichier.EcritureFichierEtAffichage("-> Nombre de permutations : " + U.pivotage.Count); }
                else { fichier.EcritureFichierEtAffichage("-> Il n'y a pas eu de permutation."); }
                return FusionnerLetU(n, L, U);
            }
            catch (Exception) { throw; }
        }

        public Matrice InversionL()
        {
            Matrice kronecker = InitMatrice(n);
            Matrice m = this;
            Matrice x = new Matrice(n, precision, this.fichier, this.pivotage);
            char lettr = 'x';
            fichier.afficherEncadre(5, "-> Inversion de la matrice L");
            fichier.EcritureFichierEtAffichage("-> Matrice de départ (L):");
            fichier.EcritureFichierEtAffichage(m.ToString());
            fichier.EcritureFichierEtAffichage("\nMultiplié par la matrice " + lettr + " : ");
            fichier.EcritureFichierEtAffichage(DessinerMatriceXY(lettr));
            fichier.EcritureFichierEtAffichage("\nEgal (Kronecker) : ");
            fichier.EcritureFichierEtAffichage(kronecker.ToString());
            fichier.EcritureFichierEtAffichage("\n-> Début de l'inversion");

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    // On inverse chaque élément de L via la matrice kronecker, L et X.
                    x.mat[i][j] = InversionRecursiveL(kronecker, m, x, i, j);
                    fichier.EcritureFichierEtAffichage(lettr + ((i + 1) + "") + ((j + 1) + " = ") + x.mat[i][j] + "\n");
                }
            }

            fichier.afficherEncadre(6, "Matrice " + lettr + " après inversion : ");
            fichier.EcritureFichierEtAffichage(x.ToString());
            return x;
        }

        public double InversionRecursiveL(Matrice kr, Matrice m, Matrice x, int i, int j)
        {
            int k = 0;
            double doublX = kr.mat[i][j];
            // Contraitement à l'inversion U, nous effectuons les calculs dans l'ordre croissant
            while (k < i)
            {
                fichier.EcritureFichierEtAffichage("    ~> Calcul de [" + (i + 1) + "," + (j + 1) + "] =>> " + doublX + " - (" + m.mat[i][k] + " * " + x.mat[k][j] + ")");
                // Application de la formule page 27
                // Seul l'indice k est changeant.
                // tant que k < i on continue de soustraire. 
                // Si nous sommes dans la première ligne (x11, i = 0 et k =0) on entre même pas dans le while.
                doublX -= m.mat[i][k] * x.mat[k][j];
                k++;
            }
            return doublX;
        }

        public Matrice InversionU()
        {
            Matrice kronecker = InitMatrice(n);
            Matrice u = this;
            Matrice y = new Matrice(n, precision, this.Fichier, this.pivotage);
            char lettr = 'y';
            fichier.afficherEncadre(7, "-> Inversion de la matrice U");
            fichier.EcritureFichierEtAffichage("-> Matrice de départ (u):");
            fichier.EcritureFichierEtAffichage(u.ToString());
            fichier.EcritureFichierEtAffichage("\nMultiplié par la matrice " + lettr + " : ");
            fichier.EcritureFichierEtAffichage(DessinerMatriceXY(lettr));
            fichier.EcritureFichierEtAffichage("\nEgal (Kronecker) : ");
            fichier.EcritureFichierEtAffichage(kronecker.ToString());
            fichier.EcritureFichierEtAffichage("\n-> Début de l'inversion");

            for (int i = n - 1; i >= 0; i--)
            {
                for (int j = 0; j < n; j++)
                {
                    // Le dernier élément on ne fait que recopier de la matrice de Kronecker
                    // /!\ on ne peut pas diviser par zero
                    if (i == n - 1) { y.mat[i][j] = u.mat[i][i] == 0 ? 0 : kronecker.mat[i][j] / u.mat[i][i]; }
                    // Les autres il faut appliquer la formule page 28 (explication dans la fonction correspondante)
                    else { y.mat[i][j] = InversionU(kronecker, u, y, i, j); }
                    fichier.EcritureFichierEtAffichage(lettr + ((i + 1) + "") + ((j + 1) + " = ") + y.mat[i][j]+ "\n");
                }
            }

            fichier.afficherEncadre(8, "Matrice " + lettr + " après inversion : ");
            fichier.EcritureFichierEtAffichage(y.ToString());
            return y;
        }

        public double InversionU(Matrice kr, Matrice u, Matrice y, int i, int j)
        {
            // Si le num = 0 alors le résultat vaut zero.
            if (u.mat[i][i] == 0) { return 0; }
            else
            {
                // On commence par la fin, k = la dimension de la matrice -1
                int k = n - 1;
                // Au début le résultat vaut la valeur dans Kronecker en position [i][j] divisé par U[i][i]
                double doublY = kr.mat[i][j] / u.mat[i][i];

                while (k > i)
                {
                    fichier.EcritureFichierEtAffichage("    ~> Calcul de [" + (i + 1) + "," + (j + 1) + "] =>> (" + doublY + " - (" + u.mat[i][k] + " * " + y.mat[k][j] + "))/" + u.mat[i][i]);
                    // Utilisation de l'algorithme donné page 28 du cours.
                    // Ensuite, de cette valeur on soustrait u.mat[i][k] * y.mat[k][j] toujours divisé par u[i][i]
                    // Qu'importe le nombre d'itérations, il n'y a que K qui évolue jusqu'à ce qu'il soit = à i.
                    doublY -= u.mat[i][k] * y.mat[k][j] / u.mat[i][i];
                    k--;
                }
                return doublY;
            }
        }

        public Matrice CalculerInverseA(Matrice UI, Matrice LI)
        {
            try
            {
                fichier.afficherEncadre(9, "Calcul de l'inverse de la matrice A : ");
                Matrice AInverse = UI.MultiplierMatrice(LI.mat);
                fichier.EcritureFichierEtAffichage(AInverse.ToString());

                // S'il n'y a pas eu de pivotages ça ne sert à rien de montrer la matrice "dépivotée"
                if (this.Pivotage.Count > 0)
                {
                    Matrice AInverseDansLordre = new Matrice(n, precision, this.Fichier, this.pivotage);
                    fichier.afficherEncadre(10, "Matrice inverse A remise dans le bon ordre : ");
                    AInverseDansLordre.mat = AInverse.DeswapperMatrice(this.Pivotage);
                    fichier.EcritureFichierEtAffichage(AInverseDansLordre.ToString());
                    return AInverseDansLordre;
                }
                else { return AInverse; }
            }
            catch (Exception) { throw; }

        }
        //Permet de permuter la ligne avec la ligne possèdant le pivot max
        private bool PermuterMax(Matrice U, Matrice L, int ligne)
        {
            try
            {
                // Permet de savoir si la permutation peut avoir lieu.
                bool trouvePlusGrand = false;
                // On ne regarde que la diagonale à chaque fois (pivot)
                double max = Math.Abs(U.mat[ligne][ligne]);
                //La ligne contenant le pivot maximal
                int ligneMaxActuelle = ligne;

                //On commence à la ligne actuelle et on recherche une ligne plus grande pour permuter
                for (int i = ligne; i < U.mat.GetLength(0); i++)
                {
                    // On recherhe en valeur absolue
                    if (Math.Abs(U.mat[i][ligne]) > max)
                    {
                        //Si on trouve, la ligne max devient l'actuelle et la valeur absolue aussi
                        ligneMaxActuelle = i;
                        max = U.mat[i][ligne];
                        trouvePlusGrand = true;
                    }
                }
                if (trouvePlusGrand)
                {
                    //On effectue la permutation pour U et L.
                    U.mat = U.SwapLignes(ligne, ligneMaxActuelle, U.mat, true);
                    L.mat = L.SwapLignes(ligne, ligneMaxActuelle, L.mat, false);
                }

                return trouvePlusGrand;
            }
            catch (Exception) { throw; }
        }

        public double[][] SwapLignes(int a, int b, double[][] matriceASwapper, bool premierPassage)
        {
            try
            {
                // Swap classique
                double[] tmp = matriceASwapper[a];
                matriceASwapper[a] = matriceASwapper[b];
                matriceASwapper[b] = tmp;

                // Cette méthode est appelée pour U et pour L, elle ne peut être effectuée que pour U
                // Car sinon on duplique le nombre de pivotages.
                if (premierPassage)
                {
                    this.Pivotage.Add(a + "|" + b);
                    fichier.EcritureFichierEtAffichage("~> Permutation entre la ligne " + (a + 1) + " et " + (b + 1));
                }
                return matriceASwapper;
            }
            catch (Exception) { throw; }
        }

        public double[][] DeswapperMatrice(ArrayList deswapList)
        {
            try
            {
                //On effectue la permutation DANS LE SENS INVERSE
                for (int i = deswapList.Count - 1; i >= 0; i--)
                {
                    string el = deswapList[i].ToString();
                    // On récupère les deux lignes qui ont étés swappée dans le programme
                    // Ex : 1|2 ~> ces deux lignees ont étés interverties
                    string[] ligneSwappee = el.Split('|');  
                    // On reswap ces lignes.
                    this.SwapColonnes(Convert.ToInt32(ligneSwappee[0]), Convert.ToInt32(ligneSwappee[1]));
                }
                return this.mat;
            }
            catch (Exception) { throw; }
        }

        // Comme on a inversé la matrice c'est la colonne qu'il faut swapper
        public double[][] SwapColonnes(int a, int b)
        {
            try
            {
                for (int i = 0; i < this.mat.Length; i++)
                {
                    // Swap classique tmp = a, a = b, b = a
                    double tmp = this.mat[i][a];
                    this.mat[i][a] = this.mat[i][b];
                    this.mat[i][b] = tmp;
                }
                fichier.EcritureFichierEtAffichage("~> Permutation entre la colonne " + (a + 1) + " et " + (b + 1));
                return this.mat;
            }
            catch (Exception) { throw; }
        }

        // Calcule le déterminant + ajute le signe selon le  nombre de permutations
        public double Determinant()
        {
            try
            {
                double det = 1;
                // Le déterminant se contente de mutliplier les valeurs diagonales
                for (int i = 0; i < this.mat.GetLength(0); i++) { det *= this.mat[i][i]; }
                return det * Math.Pow(-1, this.Pivotage.Count); // Car le signe alterne selon le nombre de permutations.
            }
            catch (Exception) { throw; }
        }

        public Matrice MultiplierMatrice(double[][] mat2)
        {
            Matrice matriceDeBase = new Matrice(n, this.precision, this.fichier, this.pivotage);
            for (int i = 0; i < n; i++)
            {
                for (int k = 0; k < n; k++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        // Multiplication de la ligne i par la colonne j 
                        matriceDeBase.mat[i][k] += this.mat[i][j] * mat2[j][k];
                    }
                }
            }
            return matriceDeBase;
        }

        public bool ComparerMatice(double[][] mat2) {
            bool egal = true;
            for (int i = 0; i < this.n && egal; i++) {
                for (int j = 0; j < this.n && egal; j++) {
                    if (this.mat[i][j] != mat2[i][j]) { egal = false; }
                }
            }
            return egal;
        }
        // On sépare la matrice précédemment fusionnée
        public Matrice SplitMatriceLetU(char type)
        {
            // Pour L on prend le triangle inférieur - la diagonale -> Commence à 1 car la première ligne n'est pas impactée et se finit à i -1
            // Pour U on prend le triangle supérieur + la diagonale ->  Commence à 0 et se fini au max
            Matrice tmp = InitMatrice(n);
            for (int i = type.Equals('U') ? 0 : 1; i < n; i++)
            {
                int max = type.Equals('U') ? n : i;
                for (int j = type.Equals('U') ? i : 0; j < max; j++) { tmp.mat[i][j] = this.mat[i][j]; }
            }
            // On retourne la valeur de pivotage.
            tmp.pivotage = this.Pivotage;
            return tmp;
        }

        // Initialise la matrice avec une matrice identitié. 
        public Matrice InitMatrice(int taille)
        {
            Matrice m = new Matrice(taille, this.precision, this.fichier, this.pivotage);
            for (int i = 0; i < taille; i++)
            {
                for (int j = 0; j < taille; j++)
                {
                    m.Mat[i][j] = j == i ? 1 : 0;
                }
            }
            return m;
        }

        // Initialise la matrice avec des 0 partout
        public Matrice InitMatriceZero(int taille)
        {
            Matrice m = new Matrice(taille, this.precision, this.fichier, this.pivotage);
            for (int i = 0; i < taille; i++)
            {
                for (int j = 0; j < taille; j++)
                {
                    m.Mat[i][j] = 0;
                }
            }
            return m;
        }

        // Car on ne peut renvoyer qu'une matrice on va compacter les deux matrices ensembles
        // Ainsi la fonction "Gauss" ne renvoie qu'une seule matrice qui sera splitée ensuite.
        // C'est possible étant donnée que les matrices ne se "chevauchent" pas.
        public Matrice FusionnerLetU(int taille, Matrice L, Matrice U)
        {
            for (int i = 0; i < taille; i++)
            {
                for (int j = 0; j < taille; j++)
                {
                    if (j < i) { U.Mat[i][j] = L.Mat[i][j]; }
                }
            }
            return U;
        }

        #region Méthodes d'affichage
        //affichee retourne ligne donnée d'une matrice sous forme de string.
        public string AfficherLigneMatrice(double[] line)
        {
            string s = "{";
            for (int i = 0; i < line.GetLength(0); i++)
            {
                if (i < n - 1) { s += String.Format("[{0:F" + precision + "}]", line[i]).PadRight(10 + Convert.ToInt32(precision)); }
                else { s += String.Format("[{0:F" + precision + "}]", line[i]); }
            }
            return s + "}";
        }


        // Récupérer la ligne entière d'un tableau.
        public double[] GetLineOfArray(double[] data, int index, int length)
        {
            double[] result = new double[length];
            System.Array.Copy(data, index, result, 0, length);
            return result;
        }

        public override string ToString()
        {
            string s = "\n";
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                // Récupère la ligne entière d'un array
                s += AfficherLigneMatrice(GetLineOfArray(mat[i], 0, n)) + Environment.NewLine;
            }
            return s;
        }

        public string DessinerMatriceXY(char lettr)
        {
            string s = "";
            for (int i = 0; i < n; i++)
            {
                s += "{";
                for (int j = 0; j < n; j++)
                {
                    if (j < n - 1) { s += String.Format(lettr + ((i + 1) + "") + ((j + 1) + "")).PadRight(10); }
                    // Pas d'espacement pour le dernier élément
                    else { s += String.Format(lettr + ((i + 1) + "") + ((j + 1) + "")); }

                }
                // saut de ligne dans le fichier
                s += "}" + Environment.NewLine;
            }
            return s;
        }
        #endregion
        #endregion

        #region Propriétés
        public double[][] Mat
        {
            get { return mat; }
            set { mat = value; }
        }
        public ArrayList Pivotage
        {
            get { return pivotage; }
            set { pivotage = value; }
        }
        public int N
        {
            get { return n; }
            set { n = value; }
        }
        public FichierMatrice Fichier
        {
            get { return fichier; }
            set { fichier = value; }
        }

        public int Precision
        {
            get { return precision; }
            set { precision = value; }
        }
        #endregion
    }
}
