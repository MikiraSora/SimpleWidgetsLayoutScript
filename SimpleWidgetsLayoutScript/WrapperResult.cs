using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWidgetsLayoutScript
{
    public class WrapperResult
    {
        public List<ElementBase> Element { get; set; }

        public WrapperResult()
        {
            Element = new List<ElementBase>();
        }
    }
}
