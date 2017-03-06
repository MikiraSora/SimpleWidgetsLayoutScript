using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleWidgetsLayoutScript;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = ScriptLoader.LoadFromFile(@"g:\test.swls");

            foreach (var elementRoot in result.Element)
                dump(elementRoot);

            Console.ReadLine();
        }

        public static void dump(ElementBase element,int level=0)
        {
            for (int i = 0; i < level; i++)
                Console.Write("-");
            Console.Write("{0}\n", element.ToString());
            level++;
            foreach (var child in element.ChildrenElement)
                dump(child, level);
        }
    }
}
