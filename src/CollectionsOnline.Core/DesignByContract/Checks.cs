using System;

namespace CollectionsOnline.Core.DesignByContract
{
    public class Checks
    {
        /// <summary>
        /// Checks a condition and throws an exception if this condition is false
        /// </summary>
        /// <param name="condition">This condition should be true, otherwise an exception will be thrown</param>
        /// <param name="conditionDescription">A textual description of the condition</param>
        public static void That(bool condition, string conditionDescription)
        {
            if (!condition)
            {
                throw AssertException.Create("Condition", conditionDescription);
            }
        }

        /// <summary>
        /// Checks a condition and throws an exception if this condition is false.
        /// This method should be used, if a description must be dynamically created. 
        /// string.Format() will only be called, if the condition fails.
        /// </summary>
        /// <param name="condition">This condition should be true, otherwise an exception will be thrown</param>
        /// <param name="descriptionFormat">Will be passed to string.Format(), to create a textual description of the condition</param>
        /// <param name="descriptionParameters">Will be passed to string.Format(), to create a textual description of the condition</param>
        public static void That(bool condition, string descriptionFormat, params object[] descriptionParameters)
        {
            if (!condition)
            {
                throw AssertException.Create("Condition",
                                             descriptionFormat,
                                             descriptionParameters);
            }
        }

        public static void That(Func<bool> function)
        {
            if (!function())
            {
                throw AssertException.Create("Condition", function);
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
                throw AssertException.Create("Condition", objectName + " is not null");
            }
        }
    }
}
