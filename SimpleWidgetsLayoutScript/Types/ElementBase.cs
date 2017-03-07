using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWidgetsLayoutScript
{
    public class ElementBase
    {
        public List<ElementBase> ChildrenElement { get; set; }

        public String ElementName { get; set; }

        public String Type { get; set; }

        public Dictionary<string,object> ElementProperties { get; set; }

        public ElementBase ParentElement { get; set; }

        public ElementBase()
        {
            ChildrenElement = new List<ElementBase>();
            ElementName = String.Empty;
            this.Type = String.Empty;
            ElementProperties = new Dictionary<string, object>();
            ParentElement = null;
        }

        public void EnumAllChildrenElement(ref List<ElementBase> refResultList) 
        {
            refResultList.Add(this);
            foreach (var child in ChildrenElement)
                EnumAllChildrenElement(ref refResultList);
        }

        public override string ToString()
        {
            return string.Format("[{0}]\"{1}\" --->{2}",this.Type,ElementName,ParentElement==null?"<root>":ParentElement.ElementName);
        }

		public string getCalculatableVariableValue(string varName)
		{
			string value,variableName=varName.Substring(1);

			if (varName[0] == '$')
			{
				if (ParentElement == null)
					throw new SynatxErrorException(-1,-1,string.Format("In element \"{0}\" ,cant get variable \"{2}\" from null parent",ElementName,varName));
				if (!ParentElement.ElementProperties.ContainsKey(variableName))
					throw new SynatxErrorException(-1,-1,"cant find parent property "+variableName);
				value = (string)ParentElement.ElementProperties[variableName];
				if (value.Contains("\""))
					throw new SynatxErrorException(-1, -1, string.Format("In element \"{0}\" property \"{1}\" cant be calculated",ElementName,varName));
				return value;
			}

			if (varName[0] == '#')
			{
				if (ParentElement == null)
					throw new SynatxErrorException(-1, -1, string.Format("In element \"{0}\" ,cant get variable \"{2}\" from null parent", ElementName, varName));
				if (!ElementProperties.ContainsKey(variableName))
					throw new SynatxErrorException(-1, -1, "cant find parent property " + variableName);
				value = (string)ElementProperties[variableName];
				if (value.Contains("\""))
					throw new SynatxErrorException(-1, -1, string.Format("In element \"{0}\" property \"{1}\" cant be calculated", ElementName, varName));
				return value;
			}

			throw new SynatxErrorException(-1,-1,"unknown synatx error!");
		}
    }
}
