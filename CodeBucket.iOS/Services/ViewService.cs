using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CodeBucket.Core.Services;
using ReactiveUI;

namespace CodeBucket.Services
{
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
                if (type.GetConstructor(Type.EmptyTypes) == null)
                    continue;

                var viewFor = type.GetInterface("IViewFor`1");
                var genericType = viewFor.GetGenericArguments()[0];
                _map.Add(genericType, Expression.Lambda<Func<IViewFor>>(Expression.New(type)).Compile());
            }
        }

        public IViewFor GetView(object viewModel)
        {
            var type = viewModel.GetType();
            if (!_map.ContainsKey(type))
                return null;

            try
            {
                var vc = _map[type].Invoke() as IViewFor;
                vc.ViewModel = viewModel;
                return vc;
            }
            catch
            {
                return null;
            }
        }
    }
}