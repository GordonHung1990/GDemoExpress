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

        [HttpPost(Name = "Player_Add")]
        public Task<Guid> AddAsync(PlayerAddViewModel model)
            => _mediator.Send(new PlayerAddRequest()
            {
                Account = model.Account,
                Password = model.Password,
                NickName = model.NickName,
            });

        [HttpGet("{playerId}", Name = "Player_Get")]
        public async Task<PlayerViewModel?> GetAsync(Guid playerId)
        {
            var data = await _mediator.Send(new PlayerGetRequest()
            {
                PlayerId = playerId,
            }).ConfigureAwait(false);

            return data is null ? null : new PlayerViewModel(
                PlayerId: data.PlayerId,
                Account: data.Account,
                Status: data.Status,
                LastName: data.LastName,
                FullName: data.FullName,
                NickName: data.NickName,
                PhoneNumber: data.PhoneNumber,
                Mailbox: data.Mailbox,
                CreatedOn: data.CreatedOn,
                UpdatedOn: data.UpdatedOn);
        }

        [HttpGet(Name = "Player_Query")]
        public async Task<IEnumerable<PlayerViewModel>?> QueryAsync()
        {
            var datas = await _mediator.Send(new PlayerQueryRequest()).ConfigureAwait(false);

            return datas is null || !datas.Any() ? null :
                datas.Select(data => new PlayerViewModel(
                PlayerId: data.PlayerId,
                Account: data.Account,
                Status: data.Status,
                LastName: data.LastName,
                FullName: data.FullName,
                NickName: data.NickName,
                PhoneNumber: data.PhoneNumber,
                Mailbox: data.Mailbox,
                CreatedOn: data.CreatedOn,
                UpdatedOn: data.UpdatedOn));
        }
    }
}
