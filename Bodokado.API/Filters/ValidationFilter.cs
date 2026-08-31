using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Bodokado.Application.Common.Localization;

namespace Bodokado.API.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IResponseLocalizer _responseLocalizer;

    public ValidationFilter(IServiceProvider serviceProvider, IResponseLocalizer responseLocalizer)
    {
        _serviceProvider = serviceProvider;
        _responseLocalizer = responseLocalizer;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null) continue;
            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            var validator = _serviceProvider.GetService(validatorType) as IValidator;
            if (validator is null) continue;
            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(validationContext);
            if (!result.IsValid)
            {
                var localizedErrors = new Dictionary<string, string[]>();
                foreach (var group in result.Errors.GroupBy(e => e.PropertyName))
                {
                    var messages = new List<string>();
                    foreach (var error in group)
                        messages.Add(await _responseLocalizer.LocalizeAsync(error.ErrorMessage));
                    localizedErrors[group.Key] = messages.ToArray();
                }
                var message = await _responseLocalizer.LocalizeAsync(MessageKeys.ValidationFailed);
                context.Result = new BadRequestObjectResult(new { success = false, message, errors = localizedErrors });
                return;
            }
        }
        await next();
    }
}
