using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TaskManagerCore.Utility
{

#if NETSTANDARD2_0
    internal class RegexUtility
    {

        private static readonly Regex regexDueDatePropertyNameLowerCase = new Regex(@"^due[_\.]?date$", RegexOptions.Compiled);

        public static Regex Regex_DueDatePropertyNameLowerCase()
        {
            return regexDueDatePropertyNameLowerCase;
        }
    }

#elif NET8_0_OR_GREATER
    internal partial class RegexUtility
    {

        [GeneratedRegex(@"^due[_\.]?date$")]
        public static partial Regex Regex_DueDatePropertyNameLowerCase();
    }
#endif

}
