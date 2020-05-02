using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace vroom.Extensions
{
    public class YaerRangeTillDateAttribute:RangeAttribute
    {
        public YaerRangeTillDateAttribute(int StertYear):base(StertYear , DateTime.Today.Year)
        {

        }
    }
}
