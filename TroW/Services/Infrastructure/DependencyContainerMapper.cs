using DatabaseContext.DatabaseAcces;
using Services.CustomerService;
using Services.HistoryDataManagement;
using Services.RouteFinder;
using SimpleInjector;

namespace Services.Infrastructure
{
    public class DependencyContainerMapper
    {
        public static void InitializeContainer(Container container, Lifestyle lifeStyle)
        {
            container.RegisterConditional(typeof(IRepository<>), typeof(Repository<>), lifeStyle, x => !x.Handled);
            container.Register<ICustomerServices, CustomerService.CustomerService>(lifeStyle);
            container.Register<IHistoryDataManagementService,HistoryDataManagementService>(lifeStyle);
            container.Register<IRouteFinderService,RouteFinderService>(lifeStyle);
        }
    }
}
