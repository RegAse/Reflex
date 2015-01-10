using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reflex_Example.Models;

namespace Reflex_Example.ViewModels
{
    class MainWindowViewModel
    {
        public List<Customer> customers { get; set; }
        public Customer customer { get; set; }
        public List<Review> reviews { get; set; }

        public MainWindowViewModel()
        {
            customers = Customer.All<Customer>();
            //This works
            reviews = Review.Where("id", "=", "2").Get<Review>();
        }
    }
}
