using MediatR;
using SearchEngine.Autocomplete.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SearchEngine.Autocomplete.Application.Commands
{
    public class DeleteRealEstateEntitiesIndexCommandHandler : IRequestHandler<DeleteRealEstateEntitiesIndexCommand, bool>
    {
        private readonly IRealEstateEntityService _realEstateEntityService;

        public DeleteRealEstateEntitiesIndexCommandHandler(IRealEstateEntityService realEstateEntityService)
        {
            _realEstateEntityService = realEstateEntityService;
        }

        public async Task<bool> Handle(DeleteRealEstateEntitiesIndexCommand request, CancellationToken cancellationToken)
        {
            return await _realEstateEntityService.DeleteIndex();
        }
    }
}