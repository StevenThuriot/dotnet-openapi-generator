using System.Text;
using System.Text.Json.Serialization;

namespace dotnet.openapi.generator;

internal class SwaggerComponentSchemas : Dictionary<string, SwaggerSchema>
{
    [JsonConstructor] public SwaggerComponentSchemas() { }

    public SwaggerComponentSchemas(IDictionary<string, SwaggerSchema> values) : base(values) { }

    public string? GenerateFastEnumToString(string type, string property)
    {
        if (type is null || !TryGetValue(type, out var schema))
        {
            return null;
        }

        if (schema.@enum is null)
        {
            return null;
        }

        StringBuilder builder = new(property);

        builder.AppendLine(" switch")
               .AppendLine("\t\t{");

        foreach (var (name, safeName) in schema.@enum.IterateValues(schema.FlaggedEnum, schema.EnumNames))
        {
            builder.Append("\t\t\t")
                   .Append(type)
                   .Append('.')
                   .Append(safeName)
                   .Append(" => \"")
                   .Append(name)
                   .AppendLine("\",");
        }

        builder.AppendLine("\t\t\t_ => null");
        builder.Append("\t\t}");

        return builder.ToString();
    }
}
