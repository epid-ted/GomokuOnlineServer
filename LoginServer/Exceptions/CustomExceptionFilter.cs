using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LoginServer.Exceptions
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is DuplicateUsernameException)
            {
                object error = new { Message = "Duplicate Username!" };
                context.Result = new ConflictObjectResult(error);
            }
            else
            {
                object error = new { Message = "Error" };
                context.Result = new ConflictObjectResult(error);
            }
        }
    }
}
