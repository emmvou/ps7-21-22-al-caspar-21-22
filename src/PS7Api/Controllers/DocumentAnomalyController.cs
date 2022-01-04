using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PS7Api.Models;

namespace PS7Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentAnomalyController : ControllerBase
    {
        private readonly ILogger<DocumentAnomalyController> _logger;
        private readonly Ps7Context _context;

        public DocumentAnomalyController(ILogger<DocumentAnomalyController> logger, Ps7Context context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/DocumentAnomaly
        //[Authorize(Roles = UserRoles.Administrator)]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_context.DocumentAnomalies.AsAsyncEnumerable());
        }

        // GET: api/DocumentAnomaly/5
        //[Authorize(Roles = UserRoles.Administrator)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var docAno = await _context.DocumentAnomalies.FindAsync(id);

            if (docAno == null)
            {
                return NotFound();
            }

            return Ok(docAno);
        }

        // DELETE: api/DocumentAnomaly/5
        //[Authorize(Roles = UserRoles.Administrator)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var docAno = await _context.DocumentAnomalies.FindAsync(id);
            
            if (docAno == null)
            {
                return NotFound();
            }
            
            _context.DocumentAnomalies.Remove(docAno);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
