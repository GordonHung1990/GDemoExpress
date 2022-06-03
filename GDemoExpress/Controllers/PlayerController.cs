using GDemoExpress.Core.ApplicationServices;
using GDemoExpress.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GDemoExpress.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PlayerController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("{playerId}")]
        public Task<PlayerGetResponse?> AddAsync(Guid playerId)
            => _mediator.Send(new PlayerGetRequest()
            {
                PlayerId = playerId,
            });

        [HttpPost]
        public Task<Guid> AddAsync(PlayerAddViewModel model)
            => _mediator.Send(new PlayerAddRequest()
            {
                Account = model.Account,
                Password = model.Password,
                NickName = model.NickName,
            });
    }
}
