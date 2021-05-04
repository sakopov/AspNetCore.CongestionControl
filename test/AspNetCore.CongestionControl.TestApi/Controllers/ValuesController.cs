using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.CongestionControl.TestApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            // Simulate work
            Thread.Sleep(2000);

            return new string[] { "value1", "value2" };
        }
    }
}
