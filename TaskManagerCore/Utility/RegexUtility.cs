using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TaskManagerCore.Utility
{
    internal partial class RegexUtility
    {
        [GeneratedRegex(@"^due[_\.]?date$")]
        public static partial Regex Regex_DueDatePropertyNameLowerCase();
    }
}
