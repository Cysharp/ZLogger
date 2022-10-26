#nullable disable

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ZLogger.Internal.Unity
{
    internal class DiagnosticServiceProvider : IServiceProvider, IDisposable
    {
        readonly Dictionary<Type, List<ServiceItem>> items;

        public DiagnosticServiceProvider(IServiceCollection services)
        {
            items = new Dictionary<Type, List<ServiceItem>>();
            foreach (var item in services)
            {
                if (!items.TryGetValue(item.ServiceType, out var list))
                {
                    list = new List<ServiceItem>();
                    items.Add(item.ServiceType, list);
                }
                var serviceItem = new ServiceItem(item)
                {
                    Item = item.ImplementationInstance
                };

                list.Add(serviceItem);
            }
        }

        public void Dispose()
        {
            foreach (var item in items)
            {
                foreach (var item2 in item.Value)
                {
                    if (items is IDisposable d)
                    {
                        d.Dispose();
                    }
                }
            }
        }

        static bool IsEnumerable(Type type)
        {
            if (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
                if (type == typeof(IEnumerable<>))
                {
                    return true;
                }
            }
            return false;
        }

        public object GetService(Type serviceType)
        {
            return GetServiceCore1(serviceType, null);
        }

        public object GetServiceCore1(Type serviceType, Type genericsType)
        {
            UnityEngine.Debug.Log("GetService:" + serviceType + ", genericsType?" + genericsType);

            List<ServiceItem> list = default;

            if (genericsType != null && serviceType.IsGenericTypeDefinition)
            {
                if (items.TryGetValue(serviceType.MakeGenericType(genericsType), out list))
                {
                    goto FOUND;
                }
            }

            if (!items.TryGetValue(serviceType, out list))
            {
                if (IsEnumerable(serviceType))
                {
                    var elemType = serviceType.GetGenericArguments()[0];
                    return GetServiceCore2(elemType, genericsType);
                }

                Type useGenericsType = genericsType;
                if (serviceType.IsConstructedGenericType)
                {
                    var gen0 = serviceType.GetGenericArguments()[0];
                    if (gen0.IsGenericParameter && gen0.IsConstructedGenericType)
                    {
                        useGenericsType = gen0;
                    }
                    else if (!gen0.IsGenericType && !gen0.IsGenericParameter)
                    {
                        useGenericsType = gen0;
                    }
                }

                if (serviceType.IsGenericType)
                {
                    var gendef = serviceType.GetGenericTypeDefinition();
                    if (serviceType == gendef)
                    {
                        LogNullReturn(serviceType);
                        return null;
                    }
                    var innerService = GetServiceCore1(serviceType.GetGenericTypeDefinition(), useGenericsType);
                    return innerService;
                }

                LogNullReturn(serviceType);
                return null;
            }

            FOUND:
            return list.Last().Instantiate(this, genericsType);
        }

        public object GetServiceCore2(Type serviceType, Type genericsType)
        {
            UnityEngine.Debug.Log("GetService2:" + serviceType + ", genericsType?" + genericsType);

            List<ServiceItem> list = default;

            if (genericsType != null && serviceType.IsGenericTypeDefinition)
            {
                var elementType = serviceType.MakeGenericType(genericsType);
                if (elementType == null)
                {
                    throw new InvalidOperationException("Can't create type by MakeGenericType:" + serviceType + " <" + genericsType + ">");
                }

                if (items.TryGetValue(elementType, out list))
                {
                    goto FOUND;
                }
            }

            if (!items.TryGetValue(serviceType, out list))
            {
                if (IsEnumerable(serviceType))
                {
                    var elemType = serviceType.GetGenericArguments()[0];
                    return GetServiceCore2(elemType, genericsType);
                }

                Type useGenericsType = genericsType;
                if (serviceType.IsConstructedGenericType)
                {
                    var gen0 = serviceType.GetGenericArguments()[0];
                    if (gen0.IsGenericParameter && gen0.IsConstructedGenericType)
                    {
                        useGenericsType = gen0;
                    }
                    else if (!gen0.IsGenericType && !gen0.IsGenericParameter)
                    {
                        useGenericsType = gen0;
                    }
                }

                if (serviceType.IsGenericType && serviceType == serviceType.GetGenericTypeDefinition())
                {
                    throw new InvalidOperationException("Can not get service from: " + serviceType);
                }

                if (serviceType.IsGenericType)
                {
                    var gendef = serviceType.GetGenericTypeDefinition();
                    if (serviceType == gendef)
                    {
                        throw new InvalidOperationException("Can not get service from: " + serviceType);
                    }
                    var innerService = GetServiceCore1(serviceType.GetGenericTypeDefinition(), useGenericsType);
                    if (innerService == null)
                    {
                        Type elementType;
                        if (genericsType != null && serviceType.IsGenericTypeDefinition)
                        {
                            elementType = serviceType.MakeGenericType(genericsType);
                            if (elementType == null)
                            {
                                throw new InvalidOperationException("Can't create type by MakeGenericType:" + serviceType + " <" + genericsType + ">");
                            }
                        }
                        else
                        {
                            elementType = serviceType;
                        }

                        return Array.CreateInstance(elementType, 0);
                    }

                    var array = Array.CreateInstance(innerService.GetType(), 1);
                    array.SetValue(innerService, 0);
                    return array;
                }

                return Array.CreateInstance(serviceType, 0);
            }
            FOUND:
            {
                Type elementType;
                if (genericsType != null && serviceType.IsGenericTypeDefinition)
                {
                    elementType = serviceType.MakeGenericType(genericsType);
                    if (elementType == null)
                    {
                        throw new InvalidOperationException("Can't create type by MakeGenericType:" + serviceType + " <" + genericsType + ">");
                    }
                }
                else
                {
                    elementType = serviceType;
                }

                var array = Array.CreateInstance(elementType, list.Count);
                if (array == null)
                {
                    throw new InvalidOperationException("Can't create array: " + elementType + "[]");
                }
                for (int i = 0; i < list.Count; i++)
                {
                    array.SetValue(list[i].Instantiate(this, genericsType), i);
                }
                return array;
            }
        }

        static void LogNullReturn(Type serviceType)
        {
            UnityEngine.Debug.Log("DI can't find type:" + serviceType);
        }

        class ServiceItem
        {
            public readonly ServiceDescriptor descriptor;
            public object Item;
            public Dictionary<Type, object> ItemPerGenerics;

            bool isGenericType;

            public ServiceItem(ServiceDescriptor descriptor)
            {
                this.descriptor = descriptor;
                this.isGenericType = descriptor.ServiceType.IsGenericType && !descriptor.ServiceType.IsConstructedGenericType;
                if (isGenericType)
                {
                    ItemPerGenerics = new Dictionary<Type, object>();
                }
            }

            public object Instantiate(DiagnosticServiceProvider provider, Type genericsElementType)
            {
                if (descriptor.Lifetime == ServiceLifetime.Singleton)
                {
                    if (isGenericType && genericsElementType != null)
                    {
                        if (ItemPerGenerics.TryGetValue(genericsElementType, out var value))
                        {
                            return value;
                        }
                    }
                    else
                    {
                        if (Item != null)
                        {
                            return Item;
                        }
                    }
                }

                lock (this)
                {
                    object instance;
                    if (descriptor.ImplementationFactory != null)
                    {
                        instance = descriptor.ImplementationFactory(provider);
                    }
                    else
                    {
                        var implType = descriptor.ImplementationType;
                        if (isGenericType && genericsElementType != null)
                        {
                            implType = implType.MakeGenericType(genericsElementType);
                        }

                        var (ctor, args) = SelectConstructor(implType, genericsElementType, provider);
                        if (ctor == null)
                        {
                            throw new InvalidOperationException("IL2CPP issue, ctor is null. Type:" + implType);
                        }

                        instance = ctor.Invoke(args);
                    }

                    if (descriptor.Lifetime == ServiceLifetime.Singleton)
                    {
                        if (isGenericType && genericsElementType != null)
                        {
                            ItemPerGenerics[genericsElementType] = instance;
                        }
                        else
                        {
                            Item = instance;
                        }
                        return instance;
                    }
                    else
                    {
                        // Transient, Scoped(not supported, same as Transient).
                        return instance;
                    }
                }
            }

            (ConstructorInfo, object[]) SelectConstructor(Type implType, Type genericsElementType, DiagnosticServiceProvider provider)
            {
                var constructors = implType.GetConstructors().OrderByDescending(x => x.GetParameters().Length);
                ConstructorInfo ctor = default;
                object[] args = default;
                foreach (var item in constructors)
                {
                    ctor = item;
                    var parameters = ctor.GetParameters();
                    args = parameters.Select(x => provider.GetServiceCore1(x.ParameterType, genericsElementType)).ToArray();

                    if (args.Any(x => x == null))
                    {
                        continue;
                    }
                    break;
                }
                return (ctor, args);
            }
        }
    }
}
