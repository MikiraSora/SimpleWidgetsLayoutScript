using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWidgetsLayoutScript
{
    internal class ScriptParser
    {
        string _scriptText=null;

        Stack<ElementBase> elementParentStack = new Stack<ElementBase>();

        ElementBase getCurrentParantElement()
        {
            return elementParentStack.Count == 0 ? null : elementParentStack.Peek();
        }

        public ScriptParser(string scriptText)
        {
            _scriptText = scriptText;
        }

        public  WrapperResult parse()
        {
            if (_scriptText == null)
                return null;
#if DEBUG
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Raw:>\n{0}", _scriptText);
            Console.ResetColor();
#endif
            _scriptText = deleteCommentText();

            //fix special char
            _scriptText = _scriptText.Replace("\r\n", "\n").Replace("\t", string.Empty).Replace("\n\n", "\n");

            int position = -1;
            char ch = (char)0;

            WrapperResult elementResult = new WrapperResult();

            ElementBase currentElement = null,element;

#if DEBUG
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("DeleteComments:>\n{0}", _scriptText);
            Console.ResetColor();
            string AccessedString = "";
#endif

            while (true)
            {
                position++;
                if (position >= _scriptText.Length)
                    break;

                ch = _scriptText[position];

#if DEBUG
                AccessedString += ch;
#endif

                if (ch == '[')
                {
                    Stack<int> balanceBracket = new Stack<int>();
                    balanceBracket.Push(position);
                    string buffer = selectElement(ref position);
                    element = parseSelectElementText(ref buffer);

                    //set parantElement up
                    element.ParentElement = getCurrentParantElement();
                    if(getCurrentParantElement()!=null)
                        element.ParentElement.ChildrenElement.Add(element);

                    currentElement = element;

                    if (element.ParentElement == null)
                        elementResult.Element.Add(element);
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

			calculateElementRecursion(elementResult);

			fixWidgetProperties(elementResult);

			return elementResult;
        }

		void fixWidgetProperties( ElementBase element)
		{
			/*
			KeyValuePair<string, object> pair;
			for (int i = 0; i < element.ElementProperties.Count; i++)
			{
				pair = element.ElementProperties.ElementAt(i);
				element.ElementProperties[pair.Key] = ((string)pair.Value).Trim('"').Trim();
			}
			element.ElementName = element.ElementName.Trim('"').Trim();
			*/

			element.ElementProperties.Add("widgetName", element.ElementName);
			element.ElementProperties.Add("widgetType", element.Type);
			foreach (var child in element.ChildrenElement)
			{
				fixWidgetProperties(child);
			}
		}

		void calculateElementRecursion(ElementBase element)
		{
			PropertiesCalculator.calculate(element);
			foreach (var child in element.ChildrenElement)
			{
				calculateElementRecursion(child);
			}
		}

		void calculateElementRecursion(WrapperResult result)
		{
			foreach (var element in result.Element)
				calculateElementRecursion(element);
		}

		void fixWidgetProperties(WrapperResult result)
		{
			foreach (var element in result.Element)
			{
				fixWidgetProperties(element);
			}
		}

        string deleteCommentText()
        {
            int position = -1;
            char ch = (char)0;
            char prev_ch = (char)0;

            string result = "";

            bool isStringSelecting = false;

            while (true)
            {
                position++;
                if (position >= _scriptText.Length)
                    break;

                prev_ch = ch;
                ch = _scriptText[position];

                if(ch=='"')
                {
                    if (prev_ch != '/')
                        isStringSelecting = !isStringSelecting;
                }

                if (ch == '/' && prev_ch =='/'&&(!isStringSelecting))
                {
                    skipSingleLineComment(ref position);
                    if(result.Length!=0)
                        result=result.Remove(result.Length-1);
                    continue;
                }

                if(ch == '*' && prev_ch == '/' && (!isStringSelecting))
                {
                    skipRangeComment(ref position);
                    if (result.Length != 0)
                        result =result.Remove(result.Length - 1);
                    continue;
                }

                result += ch;
            }

            return result;
        }

        ElementBase parseSelectElementText(ref string text)
        {
            ElementBase element = new ElementBase();
            Stack<int> balanceBracket = new Stack<int>();

            string paramText = "",elementName="";

            int position = -1;

            int tmpStartPosition = 0;

            char ch;

            while (true)
            {
                position++;
                if (position >= text.Length)
                    break;
                ch = text[position];

                //start to select name
                if (ch == ':')
                {
                    tmpStartPosition = position;

                    while (true)
                    {
                        position++;
                        if (position >= text.Length)
                            throw new SynatxErrorException(0, position, "Miss \"(...)\" param list or \"]\"");
                        ch = text[position];

                        //start to parse param list
                        if (ch == '(')
                        {
                            //sub element name 
                            element.ElementName = /*text.Substring(tmpStartPosition + 1,position);*/elementName.Trim();

                            while (true)
                            {
                                position++;
                                if (position >= text.Length)
                                    throw new SynatxErrorException(balanceBracket.Peek(), position, "Miss \")\"");
                                ch = text[position];

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

                                if (ch == '(')
                                {
                                    balanceBracket.Push(position);
                                }

                                paramText += ch;
                            }
                            break;
                        }

                        elementName += ch;

                    }
                    break;
                }

                element.Type += ch;
            }
			element.Type=element.Type.Trim();
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
                    balanceBracket.Pop();
                    if (balanceBracket.Count == 0)
                        return stringSelectElementText;
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

                if (ch == '/' && prev_char == '*')
                {
                    break;
                }
            }
        }
    }
}
