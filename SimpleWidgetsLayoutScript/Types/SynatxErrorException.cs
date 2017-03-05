using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWidgetsLayoutScript
{
    class SynatxErrorException:Exception
    {
        public SynatxErrorException(int startPosition,int endPosition,string errorMsg):base(string.Format("SynatxError[Pos:{0}~{1}]{2}", startPosition, endPosition, errorMsg))
        {

        }
    }
}
