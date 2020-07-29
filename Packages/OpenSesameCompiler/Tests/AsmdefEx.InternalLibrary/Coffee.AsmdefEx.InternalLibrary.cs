namespace Coffee.AsmdefEx.InternalLibrary
{
    /// <summary>
    /// Internal Class
    /// </summary>
    internal class InternalClass
    {
        /// <summary>
        /// private String Field
        /// </summary>
        private string privateStringField = "privateStringField";

        /// <summary>
        /// private Static String Field
        /// </summary>
        private static string privateStaticStringField = "privateStaticStringField";

        /// <summary>
        /// Public Static Method
        /// </summary>
        public static string PublicStaticMethod()
        {
            return "PublicStaticMethod";
        }

        /// <summary>
        /// Private Static Method
        /// </summary>
        private static string PrivateStaticMethod()
        {
            return "PrivateStaticMethod";
        }

        /// <summary>
        /// Public Method
        /// </summary>
        public string PublicMethod()
        {
            return "PublicMethod";
        }

        /// <summary>
        /// Private Method
        /// </summary>
        private string PrivateMethod()
        {
            return "PrivateMethod";
        }
    }
}
