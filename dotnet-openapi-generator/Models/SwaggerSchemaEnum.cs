using System.Text;

namespace dotnet.openapi.generator;

internal class SwaggerSchemaEnum : List<object>
{
    public string GetBody(string enumName, SwaggerSchemaFlaggedEnum? flaggedEnum, List<string>? enumNames)
    {
        HashSet<string> unique = new(Count);

        StringBuilder builder = new();

        var flagCount = 0;
        for (int i = 0; i < Count; i++)
        {
            string name;
            var value = this[i];

            if (value is null)
            {
                //Some documents that define the enum as nullable, also include the null value.
                //This is handled differently in dotnet and should be skipped here.
                continue;
            }

            if (enumNames is not null)
            {
                name = enumNames[i];

                if (value is not string)
                {
                    name += " = " + value;
                }
            }
            else
            {
                name = value.ToString() ?? "";
            }

            var safeName = name.AsSafeString().AsSafeCSharpName("@", "_");

            if (safeName.TrimStart('@') != name)
            {
                Logger.Break();
                Logger.LogWarning($"Enum \'{enumName}\' has a value that's not supported: \'{name}\' --> \'{safeName}\'.");
                Logger.LogWarning("\tThis has been marked with an EnumMember attribute.");
                Logger.LogWarning("\tPlease manually add the needed serialization support to your ClientOptions.");
                Logger.Break();

                name = $@"[System.Runtime.Serialization.EnumMember(Value = ""{name}"")]{safeName}";
            }
            
            if (!unique.Add(name))
            {
                continue;
            }

            if (flaggedEnum is null || !name.Contains(flaggedEnum.separatingStrings))
            {
                builder.Append('\t').Append(name);

                if (flaggedEnum is not null)
                {
                    builder.Append(" = ");

                    if (i == 0)
                    {
                        builder.Append(0);
                    }
                    else
                    {
                        builder.Append("1 << ").Append(flagCount++);
                    }
                }

                builder.Append(',').AppendLine();
            }
        }

        return builder.ToString().TrimEnd('\n', '\r', ',');
    }
}
