using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Starbucks.Actors;

namespace Starbucks.Wcf
{
    public class BoundedContextHost<TBoundedContext> : ServiceHost
        where TBoundedContext : IBoundedContext, new()

    {
        private const string DefaultNamespace = "http://tempuri.org/contract/";


        public BoundedContextHost(Type serviceHost, params Uri[] baseAddresses)
            : base(serviceHost, baseAddresses)
        {
        }

        protected override ServiceDescription CreateDescription(
            out IDictionary<string, ContractDescription> implementedContracts)
        {
            ServiceDescription service = base.CreateDescription(out implementedContracts);

            ContractDescription contract = CreateContractDescription();

            implementedContracts.Add(contract.Name, contract);

            return service;
        }

        private ContractDescription CreateContractDescription()
        {
            var context = new TBoundedContext();
            var contract = new ContractDescription(context.GetName(), DefaultNamespace + context.GetName());

            context.GetUseCases()
                .Select(useCase => CreateOperationDescription(contract, useCase))
                .ForEach(contract.Operations.Add);

            return contract;
        }

        private OperationDescription CreateOperationDescription(ContractDescription contract,
                                                                Type useCase)
        {
            var result = new OperationDescription(useCase.Name, contract);
            string ns = DefaultNamespace + contract.Name;

            // add input parameter
            result.Messages.Add(new MessageDescription(ns + "/" + useCase.Name, MessageDirection.Input)
                                    {
                                        Body =
                                            {
                                                WrapperNamespace = ns,
                                                WrapperName = useCase.Name,
                                                Parts =
                                                    {
                                                        new MessagePartDescription(useCase.Name, ns)
                                                            {Type = useCase, Index = 0}
                                                    }
                                            }
                                    });

            //commands always return void
            result.Messages.Add(new MessageDescription(ns + "/" + useCase.Name, MessageDirection.Output)
                                    {
                                        Body =
                                            {
                                                WrapperNamespace = ns,
                                                WrapperName = useCase.Name + "Response",
                                                ReturnValue = new MessagePartDescription(useCase.Name + "Result", ns)
                                                                  {
                                                                      Type = typeof (void)
                                                                  }
                                            }
                                    });

            result.Behaviors.Add(new OperationInvoker(ServiceLocator.Dispatch));
            result.Behaviors.Add(new OperationBehaviorAttribute());
            result.Behaviors.Add(new DataContractSerializerOperationBehavior(result));

            return result;
        }


        public override ReadOnlyCollection<ServiceEndpoint> AddDefaultEndpoints()
        {
            ReadOnlyCollection<ServiceEndpoint> defaultEndpoints = base.AddDefaultEndpoints();
            IEnumerable<ServiceEndpoint> endpoints = from baseAddress in BaseAddresses
                                                     from contract in ImplementedContracts.Values
                                                     where contract.ContractType != typeof (IBoundedContext)
                                                     // skip bs contract
                                                     select AddServiceEndpoint(contract.Name, defaultEndpoints
                                                                                                  .Where(
                                                                                                      e =>
                                                                                                      e.ListenUri.Scheme
                                                                                                      ==
                                                                                                      baseAddress.Scheme)
                                                                                                  .Select(e => e.Binding)
                                                                                                  .First(), string.Empty);
            return new List<ServiceEndpoint>(endpoints.Union(defaultEndpoints)).AsReadOnly();
        }

        // Simple invoker, simply delegates to the underlying BusinessModel object

        #region Nested type: OperationInvoker

        private class OperationInvoker : IOperationBehavior, IOperationInvoker
        {
            private readonly Action<Object> dispatch;

            public OperationInvoker(Action<object> dispatch)
            {
                this.dispatch = dispatch;
            }

            #region IOperationBehavior Members

            public void AddBindingParameters(OperationDescription operationDescription,
                                             BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
            {
            }

            public void ApplyDispatchBehavior(OperationDescription operationDescription,
                                              DispatchOperation dispatchOperation)
            {
                dispatchOperation.Invoker = this;
            }

            public void Validate(OperationDescription operationDescription)
            {
            }

            #endregion

            #region IOperationInvoker Members

            public object[] AllocateInputs()
            {
                return new object[1];
            }

            public object Invoke(object instance, object[] inputs, out object[] outputs)
            {
                outputs = new object[0];

                dispatch(inputs[0]);
                return null;
            }

            public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
            {
                throw new NotSupportedException();
            }

            public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
            {
                throw new NotSupportedException();
            }

            public bool IsSynchronous
            {
                get { return true; }
            }

            #endregion
        }

        #endregion
    }
}