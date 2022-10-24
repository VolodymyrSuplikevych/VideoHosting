﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoHosting.Abstractions.Dto;

namespace VideoHosting.Abstractions.Services
{
    public interface IVideoService
    {
        Task<VideoDto> GetVideoById(Guid id, string userId);

        Task<IEnumerable<VideoDto>> GetVideosOfSubscripters(string userId);

        Task<IEnumerable<VideoDto>> GetLikedVideos(string userId);

        Task<IEnumerable<VideoDto>> GetDisLikedVideos(string userId);

        Task<IEnumerable<VideoDto>> GetVideosByName(string name, string userId);

        Task<IEnumerable<VideoDto>> GetVideosOfUser(string userId);

        Task<Guid> AddVideo(VideoAddDto video);

        Task RemoveVideo(Guid id);

        Task AddView(Guid id);
    }
}
