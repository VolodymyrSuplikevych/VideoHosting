using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoHosting.Abstractions.Dto;
using VideoHosting.Abstractions.Services;
using VideoHosting.Abstractions.UnitOfWork;
using VideoHosting.Domain.Entities;
using VideoHosting.Services.Exceptions;
using VideoHosting.Utilities.Constants;

namespace VideoHosting.Services.Services
{
    public class VideoService : IVideoService
    {
        protected IUnitOfWork _unit;
        protected readonly IMapper _mapper;

        public VideoService(IUnitOfWork unit, IMapper mapper)
        {
            _unit = unit;
            _mapper = mapper;
        }

        public async Task<Guid> AddVideo(VideoAddDto videoDto)
        {
            User user = await _unit.UserRepository.GetUserById(videoDto.UserId);
            Video video = new Video
            {
                Id = Guid.NewGuid(),
                Name = videoDto.Name,
                Description = videoDto.Description,
                Views = 0,
                User = user,
                PhotoPath = videoDto.PhotoPath,
                VideoPath = videoDto.VideoPath,
                DayOfCreation = DateTime.Now,
            };

            await _unit.VideoRepository.AddVideo(video);
            await _unit.SaveAsync();

            return video.Id;
        }

        public async Task RemoveVideo(Guid videoId)
        {
            Video video = await _unit.VideoRepository.GetVideoById(videoId);
            if (video == null)
            {
                throw new NotFoundException($"Video with id = {videoId} does not exist.");
            }

            _unit.VideoRepository.RemoveVideo(video);
            await _unit.SaveAsync();
        }

        public async Task AddView(Guid videoId)
        {
            Video video = await _unit.VideoRepository.GetVideoById(videoId);
            if (video == null)
            {
                throw new NotFoundException($"Video with id = {videoId} does not exist.");
            }

            video.Views++;
            await _unit.SaveAsync();
        }

        public async Task<VideoDto> GetVideoById(Guid videoId, string userId)
        {
            User user = await _unit.UserRepository.GetUserById(userId);
            Video video = await _unit.VideoRepository.GetVideoById(videoId);

            VideoDto videoDto = _mapper.Map<VideoDto>(video);

            videoDto.PhotoPath = _unit.AppSwitchRepository.GetValue(AppSwitchConstants.VideoPhotoKey) + videoDto.PhotoPath;
            videoDto.VideoPath = _unit.AppSwitchRepository.GetValue(AppSwitchConstants.VideoKey) + videoDto.VideoPath;
            videoDto.Liked = video.Reactions.FirstOrDefault(x => x.User == user && x.IsPositive) != null;
            videoDto.Disliked = video.Reactions.FirstOrDefault(x => x.User == user && !x.IsPositive) != null;

            return videoDto;
        }

        public async Task<IEnumerable<VideoDto>> GetDisLikedVideos(string userId)
        {
            User user = await _unit.UserRepository.GetUserById(userId);
            IEnumerable<VideoDto> videoDtos = _mapper.Map<IEnumerable<VideoDto>>(user.Reactions.Where(x => !x.IsPositive).Select(x => x.Video));
            return videoDtos;
        }

        public async Task<IEnumerable<VideoDto>> GetLikedVideos(string userId)
        {
            User user = await _unit.UserRepository.GetUserById(userId);
            IEnumerable<VideoDto> videoDtos = _mapper.Map<IEnumerable<VideoDto>>(user.Reactions.Where(x => x.IsPositive).Select(x => x.Video));
            return videoDtos;
        }

        public async Task<IEnumerable<VideoDto>> GetVideosOfSubscripters(string userId)
        {
            User user = await _unit.UserRepository.GetUserById(userId);

            List<VideoDto> list = new List<VideoDto>();
            foreach (var id in user.Subscriptions.Select(x => x.SubscripterId))
            {
                list.AddRange(await GetVideosOfUser(id));
            }

            return list.OrderBy(x => x.DayOfCreation);
        }

        public async Task<IEnumerable<VideoDto>> GetVideosByName(string name, string userId)
        {
            IEnumerable<Video> videos = await _unit.VideoRepository.GetVideosByName(name);
            videos.OrderByDescending(x => x.DayOfCreation).ToList();
            IEnumerable<VideoDto> videoDtos = _mapper.Map<IEnumerable<VideoDto>>(videos);

            return videoDtos;
        }

        public async Task<IEnumerable<VideoDto>> GetVideosOfUser(string userId)
        {
            User user = await _unit.UserRepository.GetUserById(userId);
            IEnumerable<VideoDto> videoDtos = _mapper.Map<IEnumerable<VideoDto>>(user.Videos);
            return videoDtos;
        }
    }
}
