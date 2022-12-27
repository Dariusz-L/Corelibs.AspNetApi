using AutoMapper;
using Common.Basic.Blocks;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Corelibs.AspNetApi.Controllers.Extensions
{
    public static class MapSendAndGetResponseWithMapperExtensions
    {
        public static Task<IActionResult> MapSendAndGetResponse<TAppQuery, TReturnValue>(this IMediator mediator, object query, IMapper mapper)
            where TAppQuery : IQuery<Result<TReturnValue>>
            where TReturnValue : class 
        {
            var appQuery = mapper.Map<TAppQuery>(query);
            return mediator.SendAndGetResponse(appQuery);
        }

        public static Task<IActionResult> MapSendAndGetPostResponse<TAppCommand>(this IMediator mediator, object command, IMapper mapper)
            where TAppCommand : ICommand<Result>
        {
            var appCommand = mapper.Map<TAppCommand>(command);
            return mediator.SendAndGetPostResponse(appCommand);
        }

        public static Task<IActionResult> MapSendAndGetPatchResponse<TAppCommand>(this IMediator mediator, object command, IMapper mapper)
           where TAppCommand : ICommand<Result>
        {
            var appCommand = mapper.Map<TAppCommand>(command);
            return mediator.SendAndGetPatchResponse(appCommand);
        }

        public static Task<IActionResult> MapSendAndGetPutResponse<TAppCommand>(this IMediator mediator, object command, IMapper mapper)
           where TAppCommand : ICommand<Result>
        {
            var appCommand = mapper.Map<TAppCommand>(command);
            return mediator.SendAndGetPutResponse(appCommand);
        }

        public static Task<IActionResult> MapSendAndGetDeleteResponse<TAppCommand>(this IMediator mediator, object command, IMapper mapper)
           where TAppCommand : ICommand<Result>
        {
            var appCommand = mapper.Map<TAppCommand>(command);
            return mediator.SendAndGetDeleteResponse(appCommand);
        }
    }
}
