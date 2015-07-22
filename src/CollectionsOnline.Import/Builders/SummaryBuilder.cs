using System.Text;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;

namespace CollectionsOnline.Import.Builders
{
    public class SummaryBuilder
    {
        private int _charactersLeft = Constants.SummaryMaxChars;

        private StringBuilder _summary;

        public SummaryBuilder()
        {
            _summary = new StringBuilder();
        }

        public SummaryBuilder AddHeading(string value)
        {
            if (_charactersLeft <= 0 || string.IsNullOrWhiteSpace(value)) return this;

            var text = _charactersLeft < value.Length ? value.Truncate(_charactersLeft) : value;

            _summary.Append(string.Format("<h5>{0}</h5>", text));

            _charactersLeft -= text.Length;

            return this;
        }

        public SummaryBuilder AddText(string value)
        {
            if (_charactersLeft <= 0 || string.IsNullOrWhiteSpace(value)) return this;

            var text = _charactersLeft < value.Length ? value.Truncate(_charactersLeft) : value;

            _summary.Append(text);

            _charactersLeft -= text.Length;

            return this;
        }

        public SummaryBuilder AddField(string field, string value)
        {
            if (_charactersLeft <= 0 || string.IsNullOrWhiteSpace(value) || _charactersLeft <= field.Length) return this;

            var text = _charactersLeft < (field.Length + value.Length) ? value.Truncate(_charactersLeft - field.Length) : value;

            _summary.Append(string.Format("<span><strong>{0}</strong>{1}</span>", field, value));

            _charactersLeft -= text.Length;

            return this;
        }

        public override string ToString()
        {
            return _summary.ToString();
        }
    }
}
