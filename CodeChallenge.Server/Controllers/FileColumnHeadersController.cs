using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Server.DB;
using CodeChallenge.Server.Models;
using Mono.TextTemplating;
using CodeChallenge.Server.Helpers;

namespace CodeChallenge.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileColumnHeadersController : ControllerBase
    {
        private readonly CollegeFootballContext _context;
        private readonly InitializationState _state;

        public FileColumnHeadersController(CollegeFootballContext context, InitializationState state)
        {
            _context = context;
            _state = state;
        }

        // GET: api/FileColumnHeaders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FileColumnHeader>>> GetFileColumns()
        {
            return await _context.FileColumns.ToListAsync();
        }

        [HttpGet("ready")]
        public IActionResult IsReady()
        {
            return Ok(new { ready = _state.IsReady });
        }
    }
}
