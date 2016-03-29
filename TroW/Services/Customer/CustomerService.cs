using System.Collections.Generic;
using System.Linq;
using DatabaseContext.DatabaseAcces;

namespace Services.Customer
{
    public class CustomerService : ICustomerServices
    {
        private readonly IRepository<DatabaseContext.Domain.Customer> _customerRepository;
        public CustomerService(IRepository<DatabaseContext.Domain.Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }
        public List<string> GetEmail()
        {
            return (from x in _customerRepository.GetAll() select x.Email).ToList();
        }
    }

}
