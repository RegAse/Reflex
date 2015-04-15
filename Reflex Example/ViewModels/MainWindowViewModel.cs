using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reflex_Example.Models;
using System.IO;

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
            //reviews = Review.Where("id", "=", "2").Get<Review>();
            reviews = customers[1].Reviews();
            Customer cust = new Customer();
            cust.Message = "This is a message.";
            cust.Name = "Leo";
            //cust.Save<Customer>(cust);
            Review review = new Review();
            review.Content = "THis is a RAD REview";
            review.Save<Review>(review);
            
            /*using(StreamReader reader = new StreamReader("../../countries.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Country country = new Country();
                    country.country = line;
                    country.Save<Country>(country);
                }
            }*/
        }
    }
}
