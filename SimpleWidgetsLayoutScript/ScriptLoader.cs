using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SimpleWidgetsLayoutScript
{
    public static class ScriptLoader
    {
        public static WrapperResult LoadFromString(string scriptText)
        {
            ScriptParser parser = new ScriptParser(scriptText);
            return parser.parse();
        }

        public static WrapperResult LoadFromFile(string filePath)
        {
            //try
            //{
                return LoadFromString(File.ReadAllText(filePath));
            //}
            //catch { return null; }
        }
    }
}
