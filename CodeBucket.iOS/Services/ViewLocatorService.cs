using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ReactiveUI;

namespace CodeBucket.Services
{
    public interface IViewLocatorService
    {
        IViewFor GetView<T>(T viewModel) where T : class;
    }

    public class ViewLocatorService : IViewLocatorService
    {
        private readonly IDictionary<Type, Func<IViewFor>> _map 
            = new Dictionary<Type, Func<IViewFor>>();

        public ViewLocatorService()
        {
            var types = Assembly.GetAssembly(typeof(ViewLocatorService))
                                .GetTypes()
                                .Where(x => typeof(IViewFor).IsAssignableFrom(x) && !x.IsAbstract);

            foreach (var type in types)
            {
                var viewFor = type.GetInterface("IViewFor`1");
                var genericType = viewFor.GetGenericArguments()[0];
                _map.Add(genericType, Expression.Lambda<Func<IViewFor>>(Expression.New(type)).Compile());
            }
        }

        public IViewFor GetView<T>(T viewModel) where T : class
        {
            var vc = _map[viewModel.GetType()].Invoke() as IViewFor;
            vc.ViewModel = viewModel;
            return vc;
        }
    }
}

