using Common.Basic.Blocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Corelibs.AspNetApi.Controllers.Extensions
{
    public static class ResultToObjectResultExtensions
    {
        public static ObjectResult To404(this Result result) =>
            new ObjectResult(result)
            {
                StatusCode = StatusCodes.Status404NotFound
            };

        public static ObjectResult To404Result(this string errorMessage) =>
            Result.Failure(errorMessage).To404();

        public static ObjectResult To204(this Result result) =>
            new ObjectResult(result)
            {
                StatusCode = StatusCodes.Status204NoContent
            };

        public static ObjectResult To204Result(this string errorMessage) =>
            Result.Failure(errorMessage).To204();

        public static ObjectResult To200(this Result result) =>
            new ObjectResult(result)
            {
                StatusCode = StatusCodes.Status200OK
            };

        public static ObjectResult To200Result(this string errorMessage) =>
            Result.Failure(errorMessage).To200();
    }
}
