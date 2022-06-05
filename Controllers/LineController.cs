using API.Data;
using API.Entities;
using API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LineController : ControllerBase
    {
        private readonly AppDbContext _context;
        public LineController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult> AddLines([FromBody] LineVM vm)
        {
            int pageSize = 999;// initial page size if not provided in the header
            var pageHeader = Request.Headers["page-size"].ToString();
            if (!string.IsNullOrWhiteSpace(pageHeader))
            {
                try
                {
                    pageSize = Convert.ToInt32(pageHeader);
                }
                catch (Exception)
                {
                    pageSize = 999;
                }
            }

            var lines = GetLinesFromWords(vm.Words, pageSize);

            /// save lines in DB
            var recordId = Guid.NewGuid().ToString();
            var lineList = new List<Line>();
            Line lineItem;
            foreach (var line in lines)
            {
                lineItem = new Line
                {
                    RecordId = recordId,
                    Words = line
                };
                lineList.Add(lineItem);
            }

            _context.Lines.AddRange(lineList);
            var result = await _context.SaveChangesAsync() > 0;
            if (result) return CreatedAtRoute("GetLines", new { recordId = recordId }, recordId);
            return BadRequest("Problem Constructing Lines");
        }

        [HttpGet("{recordId}", Name = "GetLines")]
        public async Task<ActionResult<List<string>>> GetLines(string recordId)
        {
            try
            {
                var lines = await _context.Lines.Where(l => l.RecordId == recordId).Select(x => x.Words).ToListAsync();
                return Ok(lines);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private List<string> GetLinesFromWords(List<string> words, int pageSize)
        {
            //// Removing words containing more characters than page-size
            for (int i = 0; i < words.Count; i++)
            {
                if (words[i].Length > pageSize)
                {
                    words.Remove(words[i]);
                    i--;
                }
            }


            List<string> lines = new List<string>();
            string line = "";
            while (words.Count > 0)
            {
                string word = words[0];
                //// For first word (no space character count) (+1 for space character)
                int lengthWithNextWord = line.Length == 0 ? line.Length + word.Length : line.Length + word.Length + 1;
                if (lengthWithNextWord <= pageSize)
                {
                    ////adding space before those words which are not first word of line;
                    line += line.Length == 0 ? word : " " + word;
                    words.Remove(word);
                }
                else
                {
                    lines.Add(line);
                    line = "";
                }
            }
            //// Adding the last line to the list
            if (!string.IsNullOrWhiteSpace(line))
            {
                lines.Add(line);
            }
            return lines;
        }
    }
}