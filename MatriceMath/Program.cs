using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatriceMath
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Menu m = new Menu();
                m.MenuRedirection(args[0]);
                System.Console.ReadKey();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
