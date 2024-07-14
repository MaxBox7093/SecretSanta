using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecretSanta.Contexts;
using SecretSanta.DTOs;
using SecretSanta.Models;
using System.Text.RegularExpressions;
using Group = SecretSanta.Models.Group;

namespace SecretSant.Controllers
{
    [ApiController]
    [Route("api")]
    public class PracticantController : Controller
    {
        private readonly ApplicationContext context;
        private readonly IMapper mapper;

        public PracticantController(ApplicationContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpPost("{id}/participant")]
        public async Task<IActionResult> PostPracticant(int id, [FromBody] ParticipantDto practicantDto)
        {
            var group = await context.Groups.FindAsync(id);

            if (group == null)
            {
                return NotFound("Group not found");
            }

            var practicant = mapper.Map<Participant>(practicantDto);
            practicant.GroupId = id;
            await context.Participants.AddAsync(practicant);
            await context.SaveChangesAsync();
            return Ok(practicant.id);
        }

        [HttpDelete("group/{groupId}/participant/{participantId}")]
        public async Task<IActionResult> DeleteParticipant(int groupId, int participantId)
        {
            try
            {
                var result = await context.Participants
                    .Where(p => p.id == participantId && p.GroupId == groupId)
                    .ExecuteDeleteAsync();

                if (result == 0)
                {
                    return NotFound("Participant not found in the specified group");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "An error occurred while deleting the participant.");
            }
            return Ok("Participant deleted successfully");
        }

        [HttpPost("group/{id}/toss")]
        public async Task<IActionResult> PostTossParticipant(int id)
        {
            var group = await context.Groups
           .Include(g => g.Participants)
           .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
            {
                return NotFound("Group not found");
            }

            var participants = group.Participants!.ToList();

            if (participants.Count < 3)
            {
                return BadRequest("The number of participants in the group must be at least 3.");
            }

            var shuffledParticipants = participants.OrderBy(p => Guid.NewGuid()).ToList();

            for (int i = 0; i < shuffledParticipants.Count; i++)
            {
                var participant = shuffledParticipants[i];
                var recipient = shuffledParticipants[(i + 1) % shuffledParticipants.Count];
                participant.RecipientId = recipient.id;
            }

            await context.SaveChangesAsync();

            var result = participants.Select(p => new
            {
                id = p.id,
                name = p.Name,
                wish = p.Wish,
                recipient = new
                {
                    id = p.Recipient!.id,
                    name = p.Recipient.Name,
                    wish = p.Recipient.Wish
                }
            });

            return Ok(result);
        }

        [HttpGet("group/{groupId}/participant/{participantId}/recipient")]
        public async Task<IActionResult> GetParticipant(int groupId, int participantId)
        {
            // Найти группу по идентификатору
            var group = await context.Groups
                .Include(g => g.Participants)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
            {
                return NotFound("Group not found");
            }

            // Найти участника в этой группе по идентификатору
            var participant = group.Participants!.FirstOrDefault(p => p.id == participantId);

            if (participant == null)
            {
                return NotFound("Participant not found in the specified group");
            }

            // Найти подопечного участника
            var recipient = await context.Participants
                .FirstOrDefaultAsync(p => p.id == participant.RecipientId);

            if (recipient == null)
            {
                return NotFound("Recipient not found");
            }

            // Вернуть информацию о подопечном участнике
            var result = new
            {
                id = recipient.id,
                name = recipient.Name,
                wish = recipient.Wish
            };

            return Ok(result);
        }
    }
}
