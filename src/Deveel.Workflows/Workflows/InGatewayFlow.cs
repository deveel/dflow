using System;

namespace Deveel.Workflows
{
    public sealed class InGatewayFlow : IGatewayFlow
    {
        public InGatewayFlow(string objectRef)
        {
            ObjectRef = objectRef ?? throw new ArgumentNullException(nameof(objectRef));
        }

        public  string ObjectRef { get; }

        string IGatewayFlow.NodeRef => ObjectRef;
    }
}
