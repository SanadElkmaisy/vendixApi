using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VendixPos.DTOs;
using VendixPos.Models;
using VendixPos.Services;

[ApiController]
[Route("api/[controller]")]
public class FastGroupWebPosController : ControllerBase
{
    private readonly IItemsRepository _repo;
    private readonly IMapper _mapper;

    public FastGroupWebPosController(IItemsRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FastGroupWebPosDto>>> GetAll()
    {
        var groups = await _repo.GetAllFastGroupsAsync();
        return Ok(_mapper.Map<IEnumerable<FastGroupWebPosDto>>(groups));
    }


    [HttpGet("{GroupName}")]
    public async Task<ActionResult<FastGroupWebPosDto>> Get(string GroupName)
    {
        var group = await _repo.GetFastGroupByNameAsync(GroupName);
        if (group == null) return NotFound();
        return Ok(_mapper.Map<FastGroupWebPosDto>(group));
    }

    [HttpPost]
    public async Task<IActionResult> CreateFastGroup([FromBody] FastGroupWebPosDto groupDto)
    {
        if (groupDto == null || string.IsNullOrEmpty(groupDto.FastItemGroupName))
            return BadRequest("Group name is required");

        try
        {
            await _repo.AddFastGroupAsync(groupDto);
            return Ok(new { message = "Group added successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message }); // 409 Conflict response
        }
    }


    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, FastGroupWebPosDto dto)
    {
        if (id != dto.FastItemGroupID) return BadRequest();
        var group = _mapper.Map<FastGroupWebPos>(dto);
        await _repo.UpdateFastGroupAsync(group);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _repo.DeleteFastGroupAsync(id);
        return NoContent();
    }
}
