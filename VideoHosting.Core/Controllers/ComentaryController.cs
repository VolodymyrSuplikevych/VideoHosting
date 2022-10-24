using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VideoHosting.Abstractions.Dto;
using VideoHosting.Abstractions.Services;

namespace VideoHosting.Core.Controllers
{

    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class CommentaryController : ControllerBase
    {
        private readonly ICommentaryService _commentaryService;

        public CommentaryController(ICommentaryService service)
        {
            _commentaryService = service;
        }

        [HttpGet]
        [Route("CommentaryByVideo/{id}")]
        public async Task<ActionResult> GetCommentariesByVideoId(Guid id)
        {
            IEnumerable<CommentaryDto> commentaryDto = await _commentaryService.GetCommentariesByVideoId(id);
            return Ok(commentaryDto);
        }

        [HttpPost]
        [Route("AddCommentary")]
        public async Task<ActionResult> CreateCommentary(CommentaryDto model)
        {
            if (ModelState.IsValid)
            {
                await _commentaryService.AddCommentary(model);
                return Ok(new { message = "You added commentary." });
            }

            return BadRequest();
        }

        [HttpDelete]
        [Route("DeleteCommentary/{id}")]
        public async Task<ActionResult> DeleteCommentary(Guid id)
        {
            CommentaryDto commentary = await _commentaryService.GetCommentaryById(id);
            if (commentary.UserId == User.Identity.Name || User.IsInRole("Admin"))
            {
                await _commentaryService.RemoveCommentary(id);
                return Ok(new { message = "This commentary was deleted." });
            }

            return BadRequest("You do not have rules to do it.");
        }

        [HttpPut]
        [Route("PutLike/{id}")]
        public async Task<ActionResult> PutLike(Guid id)
        {
            await _commentaryService.PutLike(id, User.Identity.Name);
            return Ok();
        }

        [HttpPut]
        [Route("PutDislike/{id}")]
        public async Task<ActionResult> PutDislike(Guid id)
        {
            await _commentaryService.PutDislike(id, User.Identity.Name);
            return Ok();
        }
    }
}
