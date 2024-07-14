using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SecretSanta.Contexts;
using SecretSanta.DTOs;
using SecretSanta.Models;

namespace SecretSant.Controllers
{
    [ApiController]
    [Route("api")]
    public class GroupsController : ControllerBase
    {
        private readonly ApplicationContext context;
        private readonly IMapper mapper;

        public GroupsController(ApplicationContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpPost("group")]
        public async Task<IActionResult> CreateGroup([FromBody] GroupDto groupDto)
        {
            if (string.IsNullOrEmpty(groupDto.Name))
            {
                return BadRequest("Name is required.");
            }

            var group = mapper.Map<Group>(groupDto);

            await context.Groups.AddAsync(group);
            await context.SaveChangesAsync();

            return Ok(group.Id);
        }

        [HttpGet("groups")]
        public async Task<IActionResult> GetGroups()
        {
            var groups = await context.Groups.Select(g => new
            {
                id = g.Id,
                name = g.Name,
                description = g.Description
            }).ToListAsync();

            if (groups == null || !groups.Any())
            {
                return NotFound("No groups found.");
            }

            return Ok(groups);
        }

        [HttpGet("group/{id}")]
        public async Task<IActionResult> GetGroup(int id)
        {
            var group = await context.Groups
                .Where(g => g.Id == id)
                .Select(g => new
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    Practicants = g.Participants!.Select(p => new
                    {
                        id = p.id,
                        Name = p.Name,
                        Wish = p.Wish
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (group == null)
            {
                return NotFound();
            }

            return Ok(group);
        }

        [HttpPut("group/{id}")]
        public async Task<IActionResult> PutGroup(int id, [FromBody] GroupDto groupDto)
        {
            if (id == 0 || string.IsNullOrEmpty(groupDto.Name))
            {
                return BadRequest("Name is required OR id not valid");
            }

            var group = await context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound("Group not found");
            }

            mapper.Map(groupDto, group);

            await context.SaveChangesAsync();

            return Ok("Group updated successfully");
        }

        [HttpDelete("group/{id}")]
        public async Task<IActionResult> PutGroup(int id) 
        {
            var group = await context.Groups.FindAsync(id);

            if (group == null) 
            {
                return NotFound("Group not found"); 
            }

            context.Groups.Remove(group!);
            await context.SaveChangesAsync();
            return Ok("Group delete successfully");
        }
    }
}
