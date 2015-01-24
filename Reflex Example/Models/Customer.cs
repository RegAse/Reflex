using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reflex_Example.Models
{
    class Customer : Reflex.Reflex // Fix
    {
        public int id { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }

        public List<Review> Reviews()
        {
            //return this.HasMany<Review>(id,"id", "customer_id");//Need to change this
            //Or 
            return HasMany<Review>(this);
        }
    }
}
