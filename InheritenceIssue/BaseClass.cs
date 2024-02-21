namespace InheritenceIssue
{
    internal class BaseClass
    {

        public string Result { get; set; }
        public String Result2 { get; set; }

        public BaseClass()
        {
            Result = "!!! ";
            Result2 = "??? ";
        }

        public virtual void WrapMethod()
        {
            Result += MethodToOverride();
            Result2 += MethodToOverride();
        }

        public virtual string MethodToOverride()
        {
            return "This is the string from the base class";
        }

    }
}
