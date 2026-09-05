using Microsoft.AspNetCore.Mvc;
// Ajoute ici le 'using' vers ton domaine si 'Result', 'Error' et 'ErrorType' sont dans un autre projet.

namespace HotelListing.API.Results
{
    public static class ResultExtensions
    {
        // 1. Pour les requêtes sans retour de données (ex: Delete, Update -> 204 No Content)
        public static IActionResult ToActionResult(this Result result)
        {
            return result.IsSuccess
                ? new NoContentResult()
                : result.Errors.ToProblemDetails();
        }

        // 2. Pour les requêtes avec retour de données (ex: Get -> 200 OK)
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            return result.IsSuccess
                ? new OkObjectResult(result.Value)
                : result.Errors.ToProblemDetails();
        }

        // 3. Pour les créations (ex: Post -> 201 Created)

        public static IActionResult ToCreatedAtActionResult<T>(
        this Result<T> result,
        string actionName,
        Func<T, object> routeValueFactory)
        {
            if (!result.IsSuccess)
                return result.Errors.ToProblemDetails();

            // Value n'est accédé QUE si IsSuccess est true
            var routeValues = routeValueFactory(result.Value);
            return new CreatedAtActionResult(actionName, null, routeValues, result.Value);
        }

        // 4. Utilitaire privé : Ne traite que le tableau d'erreurs pour générer la RFC 7807
        private static ObjectResult ToProblemDetails(this Error[] errors)
        {
            if (errors == null || errors.Length == 0)
                throw new InvalidOperationException("Impossible de générer un ProblemDetails sans erreurs.");

            var firstError = errors[0];

            var statusCode = firstError.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };

            return new ObjectResult(new ProblemDetails
            {
                Status = statusCode,
                Title = firstError.Code,
                Detail = firstError.Description,
                Extensions = { { "errors", errors } }
            })
            {
                StatusCode = statusCode
            };
        }
    }
}