using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reflex_Example.Models
{
    class Review : Reflex.Reflex
    {
        public int id { get; set; }
        public string Content { get; set; }
    }
}
