using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace CollectionsOnline.Core.DesignByContract
{
    /// <summary>
    /// AssertException is thrown by the Assertion class, if a assertion fails.
    /// </summary>
    [Serializable]
    public class AssertException : Exception
    {
        private static string AnonymousFunctionDescription = "anonymous function delivers true";

        public static AssertException Create(string assertionType, string conditionDescription)
        {
            return new AssertException(assertionType, conditionDescription);
        }

        public static AssertException Create(
            string assertionType,
            string descriptionFormat,
            params object[] descriptionParameters)
        {
            return new AssertException(
                assertionType,
                string.Format(CultureInfo.InvariantCulture,
                              descriptionFormat,
                              descriptionParameters));
        }

        public static AssertException Create(
            string assertionType, Func<bool> function)
        {
            string methodName = function.Method.Name;
            Regex anonymousFunctionNamePattern = new Regex(@"<.*>.*__.*");
            if (!anonymousFunctionNamePattern.IsMatch(methodName))
            {
                return new AssertException(assertionType, methodName);
            }
            else
            {
                throw new AssertException(
                    assertionType, AnonymousFunctionDescription);
            }
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public AssertException()
        {
        }

        /// <summary>
        /// Constructor with message.
        /// </summary>
        /// <param name="message"></param>
        public AssertException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor for message and inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public AssertException(string message,
                               Exception innerException)
            : base(message, innerException)
        {
        }


        public AssertException(string assertionType, string description)
            : base(string.Format(CultureInfo.InvariantCulture,
                                 "{0} failed. The expectation was '{1}', but this is false.",
                                 assertionType, description))
        {
        }



        /// <summary>
        /// Constructor with SerializationInfo.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected AssertException(SerializationInfo info,
                                  StreamingContext context)
            : base(info, context)
        {
        }

    }
}
