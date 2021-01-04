using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Postident.Application.Common.Models;
using Postident.Application.Common.Validators;

namespace Postident.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(typeof(DependencyInjection));

            services.AddTransient<IValidator<DataPack>, DataPackValidator>();
            services.AddTransient<IValidator<Address>, AddressValidator>();

            return services;
        }
    }
}