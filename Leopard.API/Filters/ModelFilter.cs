using Leopard.API.ResponseConvension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace Leopard.API.Filters
{
	public sealed class MyModelFilter : IActionFilter
	{
		void IActionFilter.OnActionExecuted(ActionExecutedContext context)
		{
		}

		void IActionFilter.OnActionExecuting(ActionExecutingContext context)
		{
			if (!context.ModelState.IsValid)
			{
				var errorList = context.ModelState
					.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray())
					.Where(p => p.Value?.Length > 0)
					.ToDictionary(p => p.Key, p => p.Value);

				var err = new ApiError(MyErrorCode.ModelInvalid, "Model invalid");
				err.Data.Add("info", errorList);
				context.Result = err.Wrap();
				return;
			}
		}
	}

}
