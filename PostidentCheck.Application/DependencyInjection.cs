using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Postident.Application.Common.Validators;
using Postident.Core.Entities;

namespace Postident.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(typeof(DependencyInjection));

            services.AddTransient<IValidator<DataPackReadModel>, DataPackReadModelValidator>();

            return services;
        }
    }
}