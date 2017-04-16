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
        /// <summary>
        /// 解析某个文本
        /// </summary>
        /// <param name="scriptText"></param>
        /// <returns></returns>
        public static WrapperResult LoadFromString(string scriptText)
        {
            ScriptParser parser = new ScriptParser(scriptText);
            return parser.parse();
        }

        /// <summary>
        /// 解析某个文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
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
