using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reflex_Example.Models
{
    class Review : Reflex.Reflex
    {
        public bool timestamps = false;

        public int id { get; set; }
        public string Content { get; set; }

        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
