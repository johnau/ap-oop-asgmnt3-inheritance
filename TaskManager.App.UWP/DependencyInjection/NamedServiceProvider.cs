using System;
using System.Collections.Generic;

namespace TaskManager.App.UWP.DependencyInjection
{
    public class NamedServiceProvider<T> : INamedServiceProvider<T>
    {
        private readonly IServiceProvider _provider;
        private readonly Dictionary<string, T> _services;

        public NamedServiceProvider(IServiceProvider provider, Dictionary<string, T> services)
        {
            _provider = provider;
            _services = services;
        }

        public T GetService(string name)
        {
            if (_services.ContainsKey(name))
            {
                return _services[name];
            }

            throw new ArgumentException($"Service with name {name} not found.");
        }
    }
}
