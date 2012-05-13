using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Starbucks.Wcf
{
    [ServiceContract]
    public interface IBoundedContext
    {
        IEnumerable<Type> GetUseCases();

        [OperationContract]
        string GetName();
    }
}