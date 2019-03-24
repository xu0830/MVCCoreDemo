using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MVCCoreDemo
{
    [Route("api/[controller]")]
    [EnableCors("localhost")]
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
            List<string> list = new List<string>();
            foreach(var item in _accessor.HttpContext.Request.Headers)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(item.Key);
                sb.Append(": ");
                sb.Append(item.Value);
                list.Add(sb.ToString());
            }
            return list;
                 
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public string Post(string value)
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
