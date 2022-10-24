using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VideoHosting.Abstractions.Dto;
using VideoHosting.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;

namespace VideoHosting.Core.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;
        private readonly IConfiguration _configuration;

        public VideoController(IVideoService videoService, IConfiguration config)
        {
            _videoService = videoService;
            _configuration = config;
        }
        
        [HttpPost]
        [Route("UploadVideo")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult> AddVideo([FromForm] VideoAddDto model)
        {
            string connectionString = _configuration.GetConnectionString("BlobStorage");
            
            model.UserId = User.Identity.Name;
            var files = HttpContext.Request.Form.Files;

            string photo = Guid.NewGuid().ToString();
            model.PhotoPath = files[0].FileName.Contains(".png") ? photo + ".png" : photo + ".jpg";

            BlobContainerClient photoContainer = new BlobContainerClient(connectionString, "videophotos");
            BlobClient photoBlobClient = photoContainer.GetBlobClient(model.PhotoPath);
            await photoBlobClient.UploadAsync(files[0].OpenReadStream());

            model.VideoPath = Guid.NewGuid() + ".mp4";
            BlobContainerClient videoContainer = new BlobContainerClient(connectionString, "videos");
            BlobClient videoBlobClient = videoContainer.GetBlobClient(model.VideoPath);
            await videoBlobClient.UploadAsync(files[1].OpenReadStream());
            
            Guid videoId = await _videoService.AddVideo(model);
            return Ok(videoId);
        }

        [HttpDelete]
        [Route("DeleteVideo/{id}")]
        public async Task<ActionResult> DeleteVideo(Guid id)
        {
            VideoDto videoDto = await _videoService.GetVideoById(id, User.Identity.Name);
            if (videoDto.UserId == User.Identity.Name || User.IsInRole("Admin"))
            {
                await _videoService.RemoveVideo(id);

                string connectionString = _configuration.GetConnectionString("BlobStorage");
                BlobContainerClient photoContainer = new BlobContainerClient(connectionString, "videophotos");
                BlobContainerClient videoContainer = new BlobContainerClient(connectionString, "videos");

                BlobClient photoRemoveBlobClient = photoContainer.GetBlobClient(videoDto.PhotoName);
                BlobClient videoRemoveBlobClient = videoContainer.GetBlobClient(videoDto.VideoName);

                await photoRemoveBlobClient.DeleteAsync();
                await videoRemoveBlobClient.DeleteAsync();

                return Ok(new { message = "This video was deleted" });
            }

            throw new Exception("You do not have permission");
        }

        [HttpGet]
        [Route("GetVideoById/{id}")]
        public async Task<ActionResult> GetVideoById(Guid id)
        {
            VideoDto videoDto = await _videoService.GetVideoById(id, User.Identity.Name);
            await _videoService.AddView(id);
            return Ok(videoDto);
        }

        [HttpGet]
        [Route("UsersVideos/{id}")]
        public async Task<ActionResult> GetVideosOfUser(string id)
        {
            IEnumerable<VideoDto> videos = await _videoService.GetVideosOfUser(id);
            return Ok(videos);
        }

        [HttpGet]
        [Route("GetVideosSubscripters")]
        public async Task<ActionResult> GetVideosSubscripters()
        {
            IEnumerable<VideoDto> videos = await _videoService.GetVideosOfSubscripters(User.Identity.Name);
            return Ok(videos);
        }

        [HttpGet]
        [Route("LikedVideos/{id}")]
        public async Task<ActionResult> GetLikedVideos(string id)
        {
            IEnumerable<VideoDto> videos = await _videoService.GetLikedVideos(id);
            return Ok(videos);
        }

        [HttpGet]
        [Route("DislikedVideos/{id}")]
        public async Task<ActionResult> GetDislikedVideos(string id)
        {
            IEnumerable<VideoDto> videos = await _videoService.GetDisLikedVideos(id);
            return Ok(videos);
        }

        //[HttpGet]
        //[Route("allVideos")]
        //public async Task<ActionResult> GetVideo()
        //{
        //    IEnumerable<VideoDto> videos = await _videoService.GetVideos(User.Identity.Name);
        //    return Ok(videos);
        //}

        [HttpGet]
        [Route("GetVideosByName/{name}")]
        public async Task<ActionResult> GetVideosByName(string name)
        {
            IEnumerable<VideoDto> videos = await _videoService.GetVideosByName(name, User.Identity.Name);
            return Ok(videos);
        }
    }
}
