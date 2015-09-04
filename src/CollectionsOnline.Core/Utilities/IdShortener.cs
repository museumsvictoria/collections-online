using System.Collections.Generic;
using System.Linq;

namespace CollectionsOnline.Core.Utilities
{
    public static class IdShortener
    {
        private const string _alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";
        private static Dictionary<string, string> _objectTypeMap = new Dictionary<string, string>
        {
            {"items", "0"},
            {"specimens", "1"}
        };

        public static string Encode(string id)
        {
            // TODO: check for invalid id
            var parts = id.Split('/');
            int numericId;
            
            if (int.TryParse(parts[1] + _objectTypeMap[parts[0]], out numericId))
            {
                return BaseEncode(numericId);
            }

            return null;
        }

        public static string Decode(string shortId)
        {
            var numericId = BaseDecode(shortId);

            // get Last digit (object type)
            var type = (numericId%10).ToString();

            // remove last digit (object type)
            var id = (numericId/10).ToString();

            return string.Format("{0}/{1}", _objectTypeMap.First(x => x.Value == type.ToString()).Key, id);
        }

        private static string BaseEncode(int i)
        {
            if (i == 0) return _alphabet[0].ToString();

            var s = string.Empty;

            while (i > 0)
            {
                s += _alphabet[i % _alphabet.Length];
                i = i / _alphabet.Length;
            }

            return string.Join(string.Empty, s.Reverse());
        }

        private static int BaseDecode(string s)
        {
            var i = 0;

            foreach (var character in s)
            {
                i = (i * _alphabet.Length) + _alphabet.IndexOf(character);
            }

            return i;
        }
    }
}
