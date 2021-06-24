using MediatR;

namespace SearchEngine.Autocomplete.Application.Commands
{
    public class IndexRealEstateEntitiesCommand : IRequest<bool>
    {
        public int MaxItems { get; set; }
    }
}