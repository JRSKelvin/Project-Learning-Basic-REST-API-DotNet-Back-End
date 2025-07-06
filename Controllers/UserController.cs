using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Contracts;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Interfaces;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Models.Common;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Models.Domain;

namespace Project_Learning_Basic_REST_API_DotNet_Back_End.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            var response = new SuccessResponse<object>((int)HttpStatusCode.OK, "Data Retrieved Successfully", users);
            return StatusCode(response.StatusCode, response);
            /*
            return Ok(users);
            */
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user is null)
            {
                var notFoundResponse = new ErrorResponse((int)HttpStatusCode.NotFound, "Not Found", "Data Not Found");
                return StatusCode(notFoundResponse.StatusCode, notFoundResponse);
            }
            var response = new SuccessResponse<object>((int)HttpStatusCode.OK, "Data Retrieved Successfully", user);
            return StatusCode(response.StatusCode, response);
            /*
            return user is null ? NotFound() : Ok(user);
            */
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserRequest request)
        {
            var user = _mapper.Map<User>(request);
            var created = await _userService.CreateAsync(user);
            var response = new SuccessResponse<object>((int)HttpStatusCode.Created, "Data Created Successfully", created);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
            /*
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            */
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateUserRequest request)
        {
            var user = _mapper.Map<User>(request);
            var updatedUser = await _userService.UpdateAsync(id, user);
            if (updatedUser == null)
            {
                var notFoundResponse = new ErrorResponse((int)HttpStatusCode.NotFound, "Not Found", "Data Not Found");
                return StatusCode(notFoundResponse.StatusCode, notFoundResponse);
            }
            var response = new SuccessResponse<object>((int)HttpStatusCode.OK, "Data Updated Successfully", updatedUser);
            return StatusCode(response.StatusCode, response);
            /*
            if (updatedUser == null) return NotFound();
            return Ok(updatedUser);
            */
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deletedUser = await _userService.DeleteAsync(id);
            if (deletedUser == null)
            {
                var notFoundResponse = new ErrorResponse((int)HttpStatusCode.NotFound, "Not Found", "Data Not Found");
                return StatusCode(notFoundResponse.StatusCode, notFoundResponse);
            }
            var response = new SuccessResponse<object>((int)HttpStatusCode.OK, "Data Deleted Successfully", deletedUser);
            return StatusCode(response.StatusCode, response);
            /*
            if (deletedUser == null) return NotFound();
            return Ok(deletedUser);
            */
        }
    }
}
