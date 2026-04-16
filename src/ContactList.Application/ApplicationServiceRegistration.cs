using System.Reflection;
using ContactList.Application.Common.Behaviours;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ContactList.Application
{
    /// <summary>
    /// Composition root for the Application layer. Wires MediatR, FluentValidation
    /// and AutoMapper to the executing assembly, then registers the pipeline
    /// behaviours in outer-to-inner execution order:
    /// <list type="number">
    /// <item><c>UnhandledExceptionBehaviour</c> — top-level safety net that logs
    ///       anything that escapes the inner steps.</item>
    /// <item><c>LoggingBehaviour</c> — emits a "Handling/Handled" pair around every
    ///       request so timing and outcome are visible in the logs.</item>
    /// <item><c>ValidationBehaviour</c> — runs all matching <c>IValidator</c>s and
    ///       throws our <c>ValidationException</c> if any fail. Sits closest to the
    ///       handler so a validation failure short-circuits before the work starts.</item>
    /// </list>
    /// </summary>
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
            services.AddAutoMapper(assembly);
            services.AddValidatorsFromAssembly(assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            return services;
        }
    }
}
