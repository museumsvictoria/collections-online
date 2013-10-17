using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CollectionsOnline.WebApi.Models
{
    public class BaseInputModel
    {
        public int Offset { get; set; }

        public int Size { get; set; }
    }
}