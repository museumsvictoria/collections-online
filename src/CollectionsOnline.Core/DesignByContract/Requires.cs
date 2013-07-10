using System;

namespace CollectionsOnline.Core.DesignByContract
{
    public class Requires
    {
        /// <summary>
        /// Checks a precondition and throws an exception if this precondition is false
        /// </summary>
        /// <param name="precondition">This precondition should be true, otherwise an exception will be thrown</param>
        /// <param name="conditionDescription">A textual description of the precondition</param>
        public static void That(bool precondition, string conditionDescription)
        {
            if (!precondition)
            {
                throw AssertException.Create("Precondition", conditionDescription);
            }
        }

        /// <summary>
        /// Checks a precondition and throws an exception if this precondition is false
        /// This method should be used, if a description must be dynamically created. 
        /// string.Format() will only be called, if the precondition fails.
        /// </summary>
        /// <param name="precondition">This precondition should be true, otherwise an exception will be thrown</param>
        /// <param name="descriptionFormat">Will be passed to string.Format(), to create a textual description of the precondition</param>
        /// <param name="descriptionParameters">Will be passed to string.Format(), to create a textual description of the precondition</param>
        public static void That(bool precondition, string descriptionFormat, params object[] descriptionParameters)
        {
            if (!precondition)
            {
                throw AssertException.Create("Precondition", descriptionFormat, descriptionParameters);
            }
        }

        public static void That(Func<bool> function)
        {
            if (!function())
            {
                throw AssertException.Create("Precondition", function);
            }
        }

        /// <summary>
        /// Checks if the toBeTested is not null and throws an exception if this precondition is false.
        /// </summary>
        /// <param name="toBeTested">The object for test.</param>
        /// <param name="objectName">Name of the object.</param>
        public static void IsNotNull(object toBeTested, string objectName)
        {
            if (toBeTested == null)
            {
                throw AssertException.Create("Precondition", objectName + " is not null");
            }
        }

        /// <summary>
        /// Checks if the toBeTested is not null and throws an exception if this precondition is false.
        /// </summary>
        /// <param name="toBeTested">The object for test.</param>
        /// <param name="objectName">Name of the object.</param>
        public static void IsNotNullOrWhitespace(string toBeTested, string objectName)
        {
            if (string.IsNullOrWhiteSpace(toBeTested))
            {
                throw AssertException.Create("Precondition", objectName + " is not null or whitespace");
            }
        }
    }
}
