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
    }
}
