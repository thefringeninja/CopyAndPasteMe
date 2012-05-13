using System;
using System.Collections;
using System.Reflection;
using System.ServiceModel;

namespace Starbucks.Wcf
{
    public static class WindowsCommunicationFoundation
    {
        private static readonly object _syncRoot = new object();
        private static readonly FieldInfo hostingManagerField;
        private static FieldInfo serviceActivationsField;

        static WindowsCommunicationFoundation()
        {
            hostingManagerField = typeof (ServiceHostingEnvironment)
                .GetField("hostingManager", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField);

            typeof (ServiceHostingEnvironment)
                .GetMethod("EnsureInitialized", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod)
                .Invoke(null, new object[] {});
        }

        public static void RegisterServiceLayer<TBoundedContext>(Func<Type, String> endpoint = null)
            where TBoundedContext : IBoundedContext, new()

        {
            endpoint = endpoint ?? (type => "~/" + type.Name + ".svc");
            string endpointAddress = endpoint(typeof (TBoundedContext));
            Type factoryType = typeof (BoundedContextHostFactory<TBoundedContext>);
            Type service = typeof (TBoundedContext);

            lock (_syncRoot)
            {
                object hostingManager = hostingManagerField.GetValue(null);

                if (serviceActivationsField == null)
                {
                    serviceActivationsField = hostingManager
                        .GetType()
                        .GetField("serviceActivations",
                                  BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                }

                var serviceActivations = (Hashtable) serviceActivationsField.GetValue(hostingManager);

                string value = string.Format("{0}|{1}|{2}", endpointAddress, factoryType.AssemblyQualifiedName,
                                             service.AssemblyQualifiedName);

                if (false == serviceActivations.ContainsKey(endpointAddress))
                {
                    serviceActivations.Add(endpointAddress, value);
                }
            }
        }
    }
}