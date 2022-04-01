using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using FisherYatesWebApp.Services;

namespace FisherYatesWebApp
{
    [ApiController]
    [Route("[controller]")]
    public class FisherYates : Controller
    {
        protected readonly IFisherYatesService _fisherYatesService;
        public FisherYates(IFisherYatesService fisherYatesService)
        {
            _fisherYatesService = fisherYatesService;
        }

        /// <summary>
        /// todo: Add the skeleton structure for the solution, and implement unit tests (in the FisherYatesTests project)!
        /// </summary>
        /// <param name="input">List of dasherized elements to shuffle (e.g. "D-B-A-C").</param>
        /// <returns>A "200 OK" HTTP response with a content-type of `text/plain; charset=utf-8`. The content should be the dasherized output of the algorithm, e.g. "C-D-A-B".</returns>
        [HttpGet("{input}")]
        public async Task<ContentResult> GetFisherYates(string input)
        {           
          return Content(_fisherYatesService.Shuffle(input));

          
        }



    }
}