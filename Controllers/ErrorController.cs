
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class ErrorController : ApiController
{
    [Route("/error")]
    public IActionResult HandleError()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = context?.Error;

        return Problem(
            detail: exception?.Message,
            title: "Error",
            statusCode: 500
        );
    }
}
