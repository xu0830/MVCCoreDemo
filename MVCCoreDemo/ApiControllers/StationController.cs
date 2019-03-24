using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CJ.Services.Stations;
using CJ.Services.Stations.Dtos;
using Microsoft.AspNetCore.Mvc;
using MVCCoreDemo.Controllers;
using MVCCoreDemo.Models;
using MVCCoreDemo.Models.Station;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MVCCoreDemo.ApiControllers
{
    [Route("api/[controller]")]
    public class StationController : BaseController
    {
        // GET: api/<controller>
        private readonly IStationService stationService;

        public StationController(IStationService _stationService)
        {
            stationService = _stationService;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
            
        }

        [HttpPost("ticketQuery")]
        public OutputModel TicketQuery([FromBody]TicketQueryInput input)
        {
            OutputModel response = new OutputModel();
            response.Code = 200;
            response.Result = "success";
            response.Data = stationService.TicketQuery(new StationServiceInput());
            return response;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
