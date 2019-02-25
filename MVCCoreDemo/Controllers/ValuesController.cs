using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MVCCoreDemo
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private IHttpContextAccessor _accessor;
        public ValuesController(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }


        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2",
                 _accessor.HttpContext.Connection.LocalIpAddress.ToString() 
                 + _accessor.HttpContext.Connection.LocalPort.ToString(),
                _accessor.HttpContext.Connection.RemoteIpAddress.ToString() + ":"
                + _accessor.HttpContext.Connection.RemotePort.ToString() };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public string Post([FromBody]string value)
        {
            return value;
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
