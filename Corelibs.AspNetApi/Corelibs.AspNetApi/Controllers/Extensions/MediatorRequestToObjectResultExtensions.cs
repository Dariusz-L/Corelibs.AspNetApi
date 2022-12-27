using Common.Basic.Blocks;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Corelibs.AspNetApi.Controllers.Extensions
{
    public static class MediatorRequestToObjectResultExtensions
    {
        public static Task<IActionResult> SendAndGetResponse<TReturnValue, TQuery>(this IMediator mediator) 
            where TQuery : IQuery<Result<TReturnValue>>, new()
            where TReturnValue : class =>
            mediator.Send(new TQuery()).GetQueryResponse();

        public static Task<IActionResult> SendAndGetResponse<TReturnValue>(this IMediator mediator, IQuery<Result<TReturnValue>> query)
            where TReturnValue : class =>
            mediator.Send(query).GetQueryResponse();

        public static Task<IActionResult> SendAndGetPostResponse<TCommand>(this IMediator mediator) where TCommand : ICommand<Result>, new() =>
            mediator.Send(new TCommand()).GetPostCommandResponse();

        public static Task<IActionResult> SendAndGetPostResponse(this IMediator mediator, ICommand<Result> command) =>
            mediator.Send(command).GetPostCommandResponse();

        public static Task<IActionResult> SendAndGetPatchResponse(this IMediator mediator, ICommand<Result> command) =>
            mediator.Send(command).GetPatchCommandResponse();

        public static Task<IActionResult> SendAndGetPutResponse(this IMediator mediator, ICommand<Result> command) =>
            mediator.Send(command).GetPutCommandResponse();

        public static Task<IActionResult> SendAndGetDeleteResponse(this IMediator mediator, ICommand<Result> command) =>
            mediator.Send(command).GetDeleteCommandResponse();
    }
}
