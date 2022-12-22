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
        {
            var appQuery = mapper.Map<TAppQuery>(query);
            return mediator.SendAndGetResponse(appQuery);
        }
    }
}
