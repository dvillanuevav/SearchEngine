using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine.Autocomplete.Application.Commands
{
    public class IndexRealEstateEntitiesCommand : IRequest<bool>
    {
        public int MaxItems { get; set; }
    }
}
