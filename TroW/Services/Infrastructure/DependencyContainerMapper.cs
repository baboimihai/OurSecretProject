using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DatabaseContext.DatabaseAcces;
using Services.Customer;
using Services.HistoryDataManagement;
using SimpleInjector;
using SimpleInjector.Extensions;
using Container = SimpleInjector.Container;
namespace Services.Infrastructure
{
    public class DependencyContainerMapper
    {
        public static void InitializeContainer(Container container, Lifestyle lifeStyle)
        {
            container.RegisterConditional(typeof(IRepository<>), typeof(Repository<>), lifeStyle, x => !x.Handled);
            container.Register<ICustomerServices, CustomerService>(lifeStyle);
            container.Register<IHistoryDataManagementService,HistoryDataManagementService>(lifeStyle);
        }
    }
}
