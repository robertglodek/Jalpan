using System.Dynamic;

namespace Jalpan.GatewayApi;

public class PayloadSchema
{
    public ExpandoObject Payload { get; }
    public string Schema { get; }

    public PayloadSchema(ExpandoObject payload, string schema)
    {
        Payload = payload;
        Schema = schema;
    }
}