using MediatR;
using Postident.Infrastructure.Services;
using System.Threading.Tasks;

namespace PostidentCheck
{
    public class MainApp
    {
        private readonly IMediator _mediator;
        private readonly TestService _test;

        public MainApp(IMediator mediator, TestService test)
        {
            _mediator = mediator;
            _test = test;
        }

        public async Task Run()
        {
            //await _mediator.Send(new DhlUpdateParcelStatusesCommand());
            await _test.test();
            System.Console.WriteLine("FINISHED");
        }
    }
}