using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InheritenceIssue
{
    internal class SubClass : BaseClass
    {
        public SubClass()
            : base()
        {
        }

        public override void WrapMethod()
        {
            Result = "____" + MethodToOverride();
        }

        public override string MethodToOverride()
        {
            return "This is the string from the Subclass";
        }

    }
}
