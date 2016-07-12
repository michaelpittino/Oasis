using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Util
{

    public static class DynamicConvert
    {

        public static T To<T>(dynamic value) => Convert.ChangeType(value, typeof(T));

    }

}
