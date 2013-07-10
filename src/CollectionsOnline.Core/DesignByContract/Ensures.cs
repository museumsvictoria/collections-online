using System;

namespace CollectionsOnline.Core.DesignByContract
{
    public class Ensures
    {
        /// <summary>
        /// Checks a postcondition and throws an exception if this postcondition is false
        /// </summary>
        /// <param name="postcondition">This postcondition should be true, otherwise an exception will be thrown</param>
        /// <param name="conditionDescription">A textual description of the postcondition</param>
        public static void That(bool postcondition, string conditionDescription)
        {
            if (!postcondition)
            {
                throw AssertException.Create("Postcondition", conditionDescription);
            }
        }

        /// <summary>
        /// Checks a postcondition and throws an exception if this postcondition is false. 
        /// This method should be used, if a description must be dynamically created. 
        /// string.Format() will only be called, if the postcondition fails.
        /// </summary>
        /// <param name="postcondition">This postcondition should be true, otherwise an exception will be thrown</param>
        /// <param name="descriptionFormat">Will be passed to string.Format(), to create a textual description of the postcondition</param>
        /// <param name="descriptionParameters">Will be passed to string.Format(), to create a textual description of the postcondition</param>
        public static void That(bool postcondition, string descriptionFormat, params object[] descriptionParameters)
        {
            if (!postcondition)
            {
                throw AssertException.Create("Postcondition",
                                             descriptionFormat,
                                             descriptionParameters);
            }
        }


        public static void That(Func<bool> function)
        {
            if (!function())
            {
                throw AssertException.Create("Postcondition", function);
            }
        }

        /// <summary>
        /// Checks if the toBeTested is not null and throws an exception if this postcondition is false.
        /// </summary>
        /// <param name="toBeTested">The object for test.</param>
        /// <param name="objectName">Name of the object.</param>
        public static void IsNotNull(object toBeTested, string objectName)
        {
            if (toBeTested == null)
            {
                throw AssertException.Create("Postcondition", objectName + " is not null");
            }
        }

    }
}
