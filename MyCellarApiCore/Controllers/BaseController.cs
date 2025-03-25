using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCellarApiCore.Data;
using MyCellarApiCore.Extensions;
using MyCellarApiCore.Models;
using System.Net;

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
        public virtual async Task<ActionResult<IEnumerable<TModel>>> GetAll([FromQuery] string range = "", [FromQuery] string asc = "", [FromQuery] string desc = "")
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


            // sort the items by the specified field in ascending order
            if (!string.IsNullOrEmpty(asc))
            {
                query = query.SortAsc(asc);
            }
            // sort the items by the specified field in descending order
            if (!string.IsNullOrEmpty(desc))
            {
                query = query.SortDesc(desc);
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
                var items = await query.GetRange(start, totalItems).ToListAsync();

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
                    return StatusCode((int)HttpStatusCode.PartialContent, items);
                }

                return items;
            }
            return await query.ToListAsync();
        }


        [HttpGet("{id}")]
        public virtual async Task<ActionResult<dynamic>> GetModel(int id, [FromQuery] string fields = "")
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

            if (!string.IsNullOrEmpty(fields))
            {
                return StatusCode((int)HttpStatusCode.PartialContent, model.SelectFields(fields));
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
        public virtual async Task<ActionResult<IEnumerable<TModel>>> Search([FromQuery] string asc = "", [FromQuery] string desc = "")
        {
            Dictionary<string, string> queryParams = Request.Query.GetQueryParams<TModel>();
            var tr = _context.Set<TModel>().Where(m => !m.Deleted).ApplySearch(queryParams);

            // sort the items by the specified field in ascending order
            if (!string.IsNullOrEmpty(asc))
            {
                tr = tr.SortAsc(asc);
            }
            // sort the items by the specified field in descending order
            if (!string.IsNullOrEmpty(desc))
            {
                tr = tr.SortDesc(desc);
            }

            return await tr.ToListAsync();
        }
    }
}
