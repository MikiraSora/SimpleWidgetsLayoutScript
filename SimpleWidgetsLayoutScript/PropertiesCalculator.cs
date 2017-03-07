using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWidgetsLayoutScript
{
	static class PropertiesCalculator
	{
		static BaseCalculator.Calculator _calculator = new BaseCalculator.Calculator();

		public static void calculate(ElementBase element)
		{
			string value;
			List<string> varList;
			KeyValuePair<string, object> property;
			string varValue;

			for (int i=0;i<element.ElementProperties.Count;i++)
			{
				property = element.ElementProperties.ElementAt(i);
				value = (string)property.Value;

				if (value.Contains("\""))
					continue;

				if (ableDirectlyCalculate(value))
				{
					element.ElementProperties[property.Key] = _calculator.Solve(value);
					continue;
				}

				varList = searchAllVariable(value, ref element);

				foreach (var varName in varList)
				{
					varValue = element.getCalculatableVariableValue(varName);
					value = value.Replace(varName, varValue);
				}

				try{
					value=(string)(element.ElementProperties[property.Key] = _calculator.Solve(value));
				}catch { }
				
			}
		}

		static char[] ignoreChar = { '*', '/', '+', '-', '.', '^', '%', '(', ')' };

		static bool ableDirectlyCalculate(string expression)
		{
			foreach (var ch in expression)
			{
				if (Char.IsDigit(ch))
					continue;
				foreach (var _ch in ignoreChar)
				{
					if (_ch == ch)
						goto access;
				}
				return false;
				access: { }
			}

			return true;
		}

		static List<String> searchAllVariable(string expression,ref ElementBase _element)
		{

			List<string> varList = new List<string>();

			int position = -1;
			int startPosition;
			char ch = (char)0;
			string variableName = "";

			while (true)
			{
				position++;
				if (position >= expression.Length)
					break;

				ch = expression[position];

				if (ch == '$')
				{
					startPosition = position;
					while (true)
					{
						position++;
						if (position >= expression.Length)
							break;

						ch = expression[position];

						if (Char.IsLetterOrDigit(ch))
							variableName += ch;
						else
							break;
					}

					if (variableName.Length == 0)
						throw new SynatxErrorException(-1, -1, string.Format("in ElementBase \"{0}\",property value expresion {1} is miss variable name(At pos:{2})", _element.ElementName, expression, startPosition));

					if (Char.IsDigit(variableName[0]))
						throw new SynatxErrorException(-1, -1, string.Format("in ElementBase \"{0}\",property value expresion {1},variable Name \"{2}\" is invaild (At pos:{3})", _element.ElementName, expression,variableName, startPosition));

					varList.Add("$"+variableName);
					continue;
				}

				if (ch == '#')
				{
					startPosition = position;
					while (true)
					{
						position++;
						if (position >= expression.Length)
							break;

						ch = expression[position];

						if (Char.IsLetterOrDigit(ch))
							variableName += ch;
						else
							break;
					}

					if (variableName.Length == 0)
						throw new SynatxErrorException(-1, -1, string.Format("in ElementBase \"{0}\",property value expresion {1} is miss variable name(At pos:{2})", _element.ElementName, expression, startPosition));

					if (Char.IsDigit(variableName[0]))
						throw new SynatxErrorException(-1, -1, string.Format("in ElementBase \"{0}\",property value expresion {1},variable Name \"{2}\" is invaild (At pos:{3})", _element.ElementName, expression, variableName, startPosition));

					varList.Add("#"+variableName);
					continue;
				}
			}

			return varList;
		}
	}
}
