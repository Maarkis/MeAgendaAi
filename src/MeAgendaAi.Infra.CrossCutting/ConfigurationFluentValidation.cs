using FluentValidation.AspNetCore;
using MeAgendaAi.Domains.RequestAndResponse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace MeAgendaAi.Infra.CrossCutting
{
    public static class ConfigurationFluentValidation
    {
        public static IMvcBuilder AddValidation<TStartup>(this IMvcBuilder services)
        {
            services.AddFluentValidation(fluentValidationOptions =>
            {
                fluentValidationOptions.RegisterValidatorsFromAssemblyContaining<TStartup>();
                fluentValidationOptions.ImplicitlyValidateChildProperties = true;
                fluentValidationOptions.DisableDataAnnotationsValidation = true;
            }).ConfigureApiBehaviorOptions(apiOptions =>
            {
                apiOptions.InvalidModelStateResponseFactory =
                    context => new BadRequestObjectResult(
                        new ErrorMessage<string>(
                            error: MappingError(context.ModelState),
                            message: "Invalid requisition"));
            });
            return services;
        }

        private static string MappingError(ModelStateDictionary modelState)
        {
            var errors = modelState.Keys
                .SelectMany(key =>
                    modelState[key]?.Errors
                    .Select(error =>
                        $"{GetValidationCause(key, error.ErrorMessage)}") ??
                        Enumerable.Empty<string>());
            return $"Request: {string.Join("; ", errors)}";
        }

        private static string GetValidationCause(string propertyName, string message) =>
            string.IsNullOrWhiteSpace(propertyName) ? message : $"{propertyName}: {message}";
    }
}