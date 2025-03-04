using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCellarApiCore.Data;
using MyCellarApiCore.Models;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using MyCellarApiCore.Extensions;

namespace MyCellarApiCore.Controllers
{
    public abstract class BaseController<TContext, TModel> : ControllerBase where TContext : BaseDbContext where TModel : BaseModel
    {

        protected readonly TContext _context;

        public BaseController(TContext context)
        {
            _context = context;
        }

        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TModel>>> GetAll([FromQuery] string range = "")
        {
            IQueryable<TModel> query = _context.Set<TModel>().Where(m => !m.Deleted);

            // Apply filters based on query string (exclude range)
            var filter = Request.Query.GetQueryParams<TModel>();
            try
            {
                query = query.ApplyFilter(filter);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error applying filter: {ex.Message}" });
            }

            if (!string.IsNullOrEmpty(range))
            {
                var parts = range.Split('-');
                if (parts.Length != 2)
                {
                    return BadRequest();
                }
                int start;
                int end;
                try
                {
                    start = int.Parse(parts[0]);
                    end = int.Parse(parts[1]);
                }
                catch (Exception)
                {
                    return BadRequest();
                }
                int count = end - start + 1;
                if (count < 1)
                {
                    return BadRequest();
                }

                var totalItems = await query.CountAsync();
                var items = await query.Skip(start).Take(count).ToListAsync();

                Response.Headers["Content-Range"] = $"{start}-{end}/{totalItems}";
                Response.Headers["Accept-Ranges"] = $"items {count}";

                var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";
                var links = new List<string>
            {
                $"<{baseUrl}?range=0-{count - 1}>; rel=\"first\""
            };
                if (start > 0)
                {
                    var prevStart = Math.Max(0, start - count);
                    var prevEnd = start - 1;
                    links.Add($"<{baseUrl}?range={prevStart}-{prevEnd}>; rel=\"prev\"");
                }
                if (end < totalItems - 1)
                {
                    var nextStart = end + 1;
                    var nextEnd = Math.Min(totalItems - 1, end + count);
                    links.Add($"<{baseUrl}?range={nextStart}-{nextEnd}>; rel=\"next\"");
                }
                var lastStart = totalItems - (totalItems % count);
                var lastEnd = totalItems - 1;
                if (lastStart > lastEnd)
                {
                    lastStart = lastEnd;
                }
                links.Add($"<{baseUrl}?range={lastStart}-{lastEnd}>; rel=\"last\"");

                Response.Headers["Link"] = string.Join(", ", links);

                if (end < totalItems - 1)
                {
                    Response.StatusCode = (int)HttpStatusCode.PartialContent;
                }

                return items;
            }
            else
            {
                return await query.ToListAsync();
            }
        }


        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TModel>> GetModel(int id)
        {
            var model = await _context.Set<TModel>().FindAsync(id);

            if (model == null)
            {
                return NotFound();
            }
            else if (model.Deleted)
            {
                return NotFound();
            }
            return model;
        }


        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public virtual async Task<IActionResult> PutModel(int id, TModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            _context.Entry(model).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool ModelExists(int id)
        {
            return _context.Set<TModel>().Any(e => e.Id == id);
        }


        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public virtual async Task<ActionResult<TModel>> PostModel(TModel model)
        {
            _context.Set<TModel>().Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetModel", new { id = model.Id }, model);
        }


        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> DeleteModel(int id)
        {
            var model = await _context.Set<TModel>().FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            else if (model.Deleted)
            {
                return NotFound();
            }

            model.Deleted = true;
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("search")]
        public virtual async Task<ActionResult<IEnumerable<TModel>>> Search()
        {

            Dictionary<string, string> queryParams = Request.Query.GetQueryParams<TModel>();

            var tr = _context.Set<TModel>().Where(m => !m.Deleted).ApplySearch(queryParams);
            
            return await tr.ToListAsync();
        }
    }
}
