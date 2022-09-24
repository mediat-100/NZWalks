using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalksController : Controller
    {
        private readonly IWalkRepository walkRepository;
        private readonly IMapper mapper;

        public WalksController(IWalkRepository walkRepository, IMapper mapper)
        {
            this.walkRepository = walkRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWalksAsync()
        {
            // Fetch data from database - domain walks
            var walks = await walkRepository.GetAllAsync();

            // Convert domain walks to DTO Walks
            var walksDTO = mapper.Map<List<Models.DTO.Walk>>(walks);

            // Return response
            return Ok(walksDTO);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetWalkAsync")]
        public async Task<IActionResult> GetWalkAsync(Guid id)
        {
            // Get Walk Domain object from database
            var walkDomain = await walkRepository.GetAsync(id);

            if (walkDomain == null)
            {
                return NotFound();
            }

            // Convert domain object to DTO
            var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);

            // Return response
            return Ok(walkDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddWalkAsync([FromBody] Models.DTO.AddWalkRequest addWalkRequest)
        {
            // Convert DTO to domain object
            var walkDomain = new Models.Domain.Walk
            {
                Length = addWalkRequest.Length,
                Name = addWalkRequest.Name,
                RegionId = addWalkRequest.RegionId,
                WalkDifficultyId = addWalkRequest.WalkDifficultyId
            };

            // Pass domain object to Repository to persist this
            walkDomain = await walkRepository.AddAsync(walkDomain);
            
            // Convert the Domain object back to DTO
            var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);

            // Send DTO response back to client
            return CreatedAtAction(nameof(GetWalkAsync), new { id = walkDTO.Id }, walkDTO);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateWalkAsync([FromRoute] Guid id, 
            [FromBody] Models.DTO.UpdateWalkRequest updateWalkRequest)
        {
            // Convert DTO to Domain Object
            var walkDomain = new Models.Domain.Walk
            {
                Length = updateWalkRequest.Length,
                Name = updateWalkRequest.Name,
                RegionId = updateWalkRequest.RegionId,
                WalkDifficultyId = updateWalkRequest.WalkDifficultyId
            };

            // Pass details to repository - Get domain object or null
            walkDomain = await walkRepository.UpdateAsync(id, walkDomain);

            // Handle null(not found)
            if (walkDomain == null)
            {
                return NotFound();
            }

            // Convert back domain to DTO
            var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);

            // Return response
            return Ok(walkDTO);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteWalkAsync([FromRoute] Guid id)
        {
            var existingWalk = await walkRepository.DeleteAsync(id);

            if (existingWalk == null)
            {
                return NotFound();
            }

            var existingWalkDTO = mapper.Map<Models.DTO.Walk>(existingWalk);

            return Ok(existingWalkDTO);
        }
    }
}
