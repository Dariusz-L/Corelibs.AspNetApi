using Common.Basic.Blocks;
using Microsoft.AspNetCore.Mvc;

namespace Corelibs.AspNetApi.Controllers.Extensions
{
    public static class ValueTaskToObjectResultExtensions
    {
        public static async Task<IActionResult> GetQueryResponse<T>(this ValueTask<T> task)
            where T : class
        {
            var value = await task;
            if (value == null)
                return new NoContentResult();

            return new OkObjectResult(value);
        }

        public static async Task<IActionResult> GetPostCommandResponse(this ValueTask<Result> task)
        {
            var result = await task;
            if (result == null)
                return "Something went wrong".To404Result();

            if (result.ValidateSuccessAndValues())
                return result.To404();

            return new OkObjectResult(result);
        }

        public static async Task<IActionResult> GetPatchCommandResponse(this ValueTask<Result> task)
        {
            var result = await task;
            if (result == null)
                return "Something went wrong".To404Result();

            if (result.ValidateSuccessAndValues())
                return result.To404();

            return new OkObjectResult(result);
        }

        public static async Task<IActionResult> GetDeleteCommandResponse(this ValueTask<Result> task)
        {
            var result = await task;
            if (result == null)
                return "Something went wrong".To404Result();

            if (result.ValidateSuccessAndValues())
                return result.To404();

            return result.To204();
        }

        public static async Task<IActionResult> GetPutCommandResponse(this ValueTask<Result> task)
        {
            var result = await task;
            if (result == null)
                return "Something went wrong".To404Result();

            if (result.ValidateSuccessAndValues())
                return result.To404();

            return result.To200();
        }
    }
}
