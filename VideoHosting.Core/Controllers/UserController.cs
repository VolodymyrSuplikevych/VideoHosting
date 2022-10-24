using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using VideoHosting.Abstractions.Dto;
using VideoHosting.Abstractions.Services;
using System;
using System.Linq;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace VideoHosting.Core.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(IUserService service, IConfiguration config)
        {
            _userService = service;
            _configuration = config;
        }

        [HttpPost]
        [Route("UpdateUserPhoto")]
        public async Task<ActionResult> UploadPhoto()
        {
            string connectionString = _configuration.GetConnectionString("BlobStorage");
            var files = HttpContext.Request.Form.Files;
            
            UserDto user = await _userService.GetUserById(User.Identity.Name, User.Identity.Name);
     
            if (files[0].FileName.Contains(".jpg") || files[0].FileName.Contains(".png") || files[0].FileName.Contains(".jpeg"))
            {
                BlobContainerClient photoContainer = new BlobContainerClient(connectionString, "userphotos");               
                if (!string.IsNullOrWhiteSpace(user.PhotoName))
                {
                    BlobClient photoRemoveBlobClient = photoContainer.GetBlobClient(user.PhotoName);
                    await photoRemoveBlobClient.DeleteAsync();
                }

                string photo = files[0].FileName.Contains(".jpg") || files[0].FileName.Contains(".jpeg") ? Guid.NewGuid() + ".jpg" : Guid.NewGuid() + ".png";
                user.PhotoPath = photo;

                BlobClient photoBlobClient = photoContainer.GetBlobClient(photo);
                await photoBlobClient.UploadAsync(files[0].OpenReadStream());
                
                await _userService.UpdateProfile(user);
            }
            else
            {
                return BadRequest("Image should be .jpg or .png");
            }

            return Ok();
        }

        [HttpPut]
        [Route("Subscribe/{Id}")]
        public async Task<ActionResult> Subscribe(string id)
        {
            bool isSubscribed = await _userService.Subscribe(User.Identity.Name, id);
            return Ok(isSubscribed);
        }

        [HttpPut]
        [Route("UpdateUserInfo")]
        public async Task<ActionResult> UpdateUser(UserDto model)
        {
            model.PhotoPath = null;
            model.Id = User.Identity.Name;

            await _userService.UpdateProfile(model);
            return Ok(new { message = "You changed data" });
        }


        [HttpGet]
        [Route("ProfileUser/{Id}")]
        public async Task<ActionResult> GetUser(string id)
        {
            UserDto user = await _userService.GetUserById(id, User.Identity.Name);
            return Ok(user);
        }

        [HttpGet]
        [Route("subscribers")]
        public async Task<ActionResult> GetSubscribers()
        {
            IEnumerable<UserDto> users = await _userService.GetSubscribers(User.Identity.Name);
            return Ok(users);
        }

        [HttpGet]
        [Route("FindSubscriptions")]
        public async Task<ActionResult> GetSubscriptions()
        {
            IEnumerable<UserDto> users = await _userService.GetSubscriptions(User.Identity.Name);
            return Ok(users);
        }

        [HttpGet]
        [Route("FindUserByName/{name}")]
        public async Task<ActionResult> GetUserByName(string name)
        {
            IEnumerable<UserDto> users = await _userService.GetUserBySubName(name, User.Identity.Name);
            return Ok(users);
        }
    }
}
