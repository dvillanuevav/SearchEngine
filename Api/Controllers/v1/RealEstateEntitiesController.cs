using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SearchEngine.Autocomplete.Api.Models.v1;
using SearchEngine.Autocomplete.Application.Commands;
using SearchEngine.Autocomplete.Application.Models;
using SearchEngine.Autocomplete.Application.Queries;
using SearchEngine.Autocomplete.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SearchEngine.Autocomplete.Api.Controllers.v1
{
    [Route("api/v1/real-estate-entities")]
    [ApiController]
    public class RealEstateEntitiesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RealEstateEntitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Search real estate entities coincidences by market(s) and keyword.
        /// </summary>
        /// <param name="model">Market(s), keyqord and pagination parameters.</param>
        /// <returns>List of suggestions.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<SearchResult<RealEstateEntity>>> SearchByMarketAsync([FromQuery] SearchByMarketModel model)
        {
            try
            {
                return await _mediator.Send(new SearchRealEstateEntitiesByMarketQuery
                {
                    Keyword = model.Keyword,
                    Markets = model.Markets.ToList(),
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Index seed data into Elasticsearch.
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("indices/{maxItems}")]
        [HttpPost]
        public async Task<ActionResult> BulkIndexAsync(int maxItems = 20000)
        {
            try
            {
                await _mediator.Send(new IndexRealEstateEntitiesCommand
                {
                    MaxItems = maxItems
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete index in Elasticsearch.
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("indices")]
        [HttpDelete]
        public async Task<ActionResult> DeleteIndexAsync()
        {
            try
            {
                await _mediator.Send(new DeleteRealEstateEntitiesIndexCommand());

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}