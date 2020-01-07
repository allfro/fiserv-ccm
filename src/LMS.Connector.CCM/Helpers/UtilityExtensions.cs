using System.Collections.Generic;
using System.Linq;

namespace LMS.Connector.CCM.Helpers
{
    public static class UtilityExtensions
    {
        /// <summary>
        /// Returns a U.S.-based area code from a raw phone number that is 10-characters long.
        /// </summary>
        /// <param name="phoneNumberRaw">U.S.-based phone number with no symbols or spaces. The length MUST be 10-characters long.</param>
        /// <returns></returns>
        public static string GetPhoneNumberAreaCode(this string phoneNumberRaw)
        {
            string area = string.Empty;

            if (phoneNumberRaw.Length != 10)
            {
                area = phoneNumberRaw.Substring(0, 3);
            }

            return area;
        }

        /// <summary>
        /// Returns a U.S.-based major number from a raw phone number that is 10-characters long.
        /// </summary>
        /// <param name="phoneNumberRaw">U.S.-based phone number with no symbols or spaces. The length MUST be 10-characters long.</param>
        /// <returns></returns>
        public static string GetPhoneNumberMajor(this string phoneNumberRaw)
        {
            string major = string.Empty;

            if (phoneNumberRaw.Length != 10)
            {
                major = phoneNumberRaw.Substring(3, 3);
            }

            return major;
        }

        /// <summary>
        /// Returns a U.S.-based minor number from a raw phone number that is 10-characters long.
        /// </summary>
        /// <param name="phoneNumberRaw">U.S.-based phone number with no symbols or spaces. The length MUST be 10-characters long.</param>
        /// <returns></returns>
        public static string GetPhoneNumberMinor(this string phoneNumberRaw)
        {
            string minor = string.Empty;

            if (phoneNumberRaw.Length != 10)
            {
                minor = phoneNumberRaw.Substring(3, 3);
            }

            return minor;
        }

        ///// <summary>
        ///// This projects a new IEnumerable<T, int> that will hold the inputted IEnumerable<T>
        ///// as well as the index of each element in the inputted IEnumerable<T>.
        ///// If collection is null, it creates an new List<T, int> that has no elements.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="collection"></param>
        ///// <example>
        ///// foreach (var (item, index) in collection.WithIndex())
        ///// {
        /////    Debug.WriteLine($"{index}: {item}");
        ///// }
        ///// </example>
        ///// <remarks>Requires System.ValueTuple from Nuget for environments lower than .NET 4.7</remarks>
        ///// <returns></returns>
        //public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> collection)
        //{
        //    var result = collection?.Select((item, index) => (item, index)) ?? new List<(T, int)>();

        //    return result;
        //}
    }
}
