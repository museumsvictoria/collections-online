using System;
using System.Globalization;
using System.Text;
using Nancy.Cookies;

namespace CollectionsOnline.WebSite.Infrastructure
{
    public enum SameSite
    {
        Lax,
        Strict,
        None
    }
    
    public class Cookie : NancyCookie
    {
        public Cookie(string name, string value, bool httpOnly, bool secure, DateTime? expires, SameSite sameSite = SameSite.Lax) : base(name, value, httpOnly, sameSite == SameSite.None || secure, expires)
        {
            this.SameSite = sameSite;
        }

        private SameSite SameSite { get; }

        public override string ToString()
        {
            var baseString = base.ToString();

            return $"{baseString}; SameSite={this.SameSite}";
        }
    }
}