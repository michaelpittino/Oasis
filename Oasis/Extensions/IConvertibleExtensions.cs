using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oasis.Extensions
{

    public static class IConvertibleExtensions
    {

        public static T To<T>(this IConvertible value) => (T) Convert.ChangeType(value, typeof(T));

    }

}
