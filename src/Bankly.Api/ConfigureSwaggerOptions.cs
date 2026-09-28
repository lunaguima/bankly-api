using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bankly.Api;

/// <summary>
/// Cria um documento do Swagger para cada versão da API.
/// </summary>
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfo(description));
        }
    }

    private static OpenApiInfo CreateInfo(ApiVersionDescription description)
    {
        var text = "API REST do Bankly, sistema bancário desenvolvido para a FIAP. " +
                   "Expõe operações de usuários, endereços, contas, tipos de conta, cartões e transações, " +
                   "seguindo Clean Architecture e persistindo os dados em banco Oracle.";

        if (description.IsDeprecated)
        {
            text += " ATENÇÃO: esta versão da API está DEPRECADA. Ela continua funcionando, " +
                    "mas novos clientes devem usar a versão mais recente (2.0).";
        }

        return new OpenApiInfo
        {
            Title = "Bankly API",
            Version = description.ApiVersion.ToString(),
            Description = text
        };
    }
}