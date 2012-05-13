using System;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Starbucks.Wcf
{
    public class BoundedContextHostFactory<TBoundedContext>
        : ServiceHostFactory where TBoundedContext : IBoundedContext, new()
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new BoundedContextHost<TBoundedContext>(serviceType,
                                                           baseAddresses);
        }
    }
}