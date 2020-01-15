namespace Coffee.OpenSesame
{
    internal class InternalClass : InternalInterface
    {
        internal InternalStruct InternalStruct = new InternalStruct("internal");
        private InternalStruct PrivateStruct = new InternalStruct("private");

        internal virtual InternalStruct InternalProperty
        {
            get { return PrivateStruct; }
        }

        private InternalStruct PrivateGet()
        {
            return PrivateStruct;
        }

        internal virtual InternalStruct InternalGet()
        {
            return InternalStruct;
        }

        public virtual InternalEntityClass PublicGet()
        {
            return new InternalEntityClass();
        }
    }

    internal interface InternalInterface
    {
        InternalEntityClass PublicGet();
    }

    internal class InternalEntityClass
    {
        public string PublicString = "public";
        internal string InternalString = "internal";
        private string PrivateString = "private";

        internal InternalEntityClass()
        {
        }
    }

    internal struct InternalStruct
    {
        public string PublicString;
        internal string InternalString;
        private string PrivateString;

        public InternalStruct(string value)
        {
            PublicString = value;
            InternalString = value;
            PrivateString = value;
        }
    }
}
