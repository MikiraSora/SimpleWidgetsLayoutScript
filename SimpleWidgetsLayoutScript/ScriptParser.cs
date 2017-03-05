using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWidgetsLayoutScript
{
    class ScriptParser
    {
        string _scriptText=null;

        Stack<ElementBase> elementParentStack = new Stack<ElementBase>();

        ElementBase getCurrentParantElement()
        {
            return elementParentStack.Count == 0 ? null : elementParentStack.Peek();
        }

        ScriptParser(string scriptText)
        {
            _scriptText = scriptText;
        }

        public  WrapperResult parse()
        {
            if (_scriptText == null)
                return null;

            int position = -1;
            char ch = (char)0;
            char prev_char=(char)0;

            WrapperResult elementResult = new WrapperResult();

            ElementBase currentElement = null,element;

            while (true)
            {
                position++;
                if (position >= _scriptText.Length)
                    break;
                prev_char = ch;
                ch = _scriptText[position];

                if (ch == '/' && prev_char == '/')
                {
                    skipSingleLineComment(ref position);
                    continue;
                }

                if (ch == '/' && prev_char == '*')
                {
                    skipRangeComment(ref position);
                    continue;
                }

                if (ch == '[')
                {
                    Stack<int> balanceBracket = new Stack<int>();
                    balanceBracket.Push(position);
                    string buffer = selectElement(ref position);
                    element = parseSelectElementText(ref buffer);

                    //set parantElement up
                    element.ParentElement = getCurrentParantElement();

                    currentElement = element;
                }

                if (ch == '{')
                {
                    elementParentStack.Push(currentElement);
                }

                if (ch == '}')
                {
                    elementParentStack.Pop();
                }
            }

            return elementResult;
        }

        ElementBase parseSelectElementText(ref string text)
        {
            ElementBase element = new ElementBase();
            Stack<int> balanceBracket = new Stack<int>();
            Dictionary<string, object> param;

            string paramText = "";

            int position = -1;

            int tmpNameStartPosition = 0;

            char ch;

            while (true)
            {
                position++;
                if (position >= _scriptText.Length)
                    break;
                ch = _scriptText[position];

                //start to select name
                if (ch == ':')
                {
                    tmpNameStartPosition = position;

                    while (true)
                    {
                        position++;
                        if (position >= _scriptText.Length)
                            throw new SynatxErrorException(0, position, "Miss \"(...)\" param list or \"]\"");
                        ch = _scriptText[position];

                        //start to parse param list
                        if (ch == '(')
                        {
                            //sub element name 
                            element.ElementName = text.Substring(tmpNameStartPosition+1,position);

                            while (true)
                            {
                                position++;
                                if (position >= _scriptText.Length)
                                    throw new SynatxErrorException(balanceBracket.Peek(), position, "Miss \")\"");
                                ch = _scriptText[position];

                                if (ch == ')')
                                {
                                    if (balanceBracket.Count == 0)
                                    {
                                        parseSelectElementProperties(paramText,ref element);
                                        break;
                                    }
                                    else
                                        balanceBracket.Pop();
                                }

                                if (ch == ')')
                                {
                                    balanceBracket.Push(position);
                                }

                                paramText += ch;
                            }
                            break;
                        }
                    }
                    break;
                }

                element.Type += ch;
            }

            return element;
        }
        
        void parseSelectElementProperties(string name,ref ElementBase element)
        {
            string[] paramArray = name.Split(',');

            string paramName ;

            int position;

            if (paramArray.Length == 0)
                return ;

            char ch;

            foreach(string paramText in paramArray)
            {
                paramName = "";
                position = -1;

                while (true)
                {
                    position++;
                    if (position >= _scriptText.Length)
                        throw new SynatxErrorException(0, 0, "Miss \"]\"");
                    ch = paramText[position];

                    if (ch == '=')
                    {
                        string value = paramText.Substring(position + 1);
                        element.ElementProperties.Add(paramName, value);
                        break;
                    }

                    paramName += ch;
                }
            }
        }

        string selectElement(ref int position)
        {
            Stack<int> balanceBracket = new Stack<int>();
            balanceBracket.Push(position);

            char ch = (char)0;
            int startPosition = position;

            string stringSelectElementText = "";

            while (true)
            {
                position++;
                if (position >= _scriptText.Length)
                    throw new SynatxErrorException(balanceBracket.Peek(), position, "Miss \"]\"");

                ch = _scriptText[position];

                if (ch == ']')
                {
                    if (balanceBracket.Count == 0)
                        return stringSelectElementText;
                    else
                        balanceBracket.Pop();
                }

                if (ch == '[')
                {
                    balanceBracket.Push(position);
                }

                stringSelectElementText += ch;
            }
        }

        void skipSingleLineComment(ref int position)
        {
            char ch;
            while (true)
            {
                position++;
                if (position >= _scriptText.Length)
                    break;
                ch = _scriptText[position];

                if (ch == '\n')
                    break;
            }
        }
        
        void skipRangeComment(ref int position)
        {
            char ch = (char)0;
            char prev_char = (char)0;
            int startPosition = position;
            while (true)
            {
                position++;
                if (position >= _scriptText.Length)
                    throw new SynatxErrorException(startPosition, position, "Endless Range Comment");
                prev_char = ch;
                ch = _scriptText[position];

                if (ch == '*' && prev_char == '/')
                {
                    break;
                }
            }
        }
    }
}
