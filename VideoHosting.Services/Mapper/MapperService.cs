﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using VideoHosting.Abstractions.Dto;
using VideoHosting.Domain.Entities;

namespace VideoHosting.Services.Mapper
{
    public class MapperService : Profile
    {
        public MapperService(IConfiguration configuration)
        {
            CreateMap<User, UserDto>()
               .ForMember(c => c.Id, x => x.MapFrom(c => c.Id))
               .ForMember(c => c.Name, x => x.MapFrom(c => c.Name))
               .ForMember(c => c.Surname, x => x.MapFrom(c => c.Surname))
               .ForMember(c => c.Group, x => x.MapFrom(c => c.Group))
               .ForMember(c => c.DateOfCreation, x => x.MapFrom(c => c.DateOfCreation))
               .ForMember(c => c.Subscribers, x => x.MapFrom(c => c.Subscribers.Count))
               .ForMember(c => c.Subscriptions, x => x.MapFrom(c => c.Subscriptions.Count))
               .ForMember(c => c.PhotoPath, x => x.MapFrom(c => c.PhotoPath != null ? configuration.GetSection("Settings:UserPhoto").Value + c.PhotoPath : null))
               .ForMember(c => c.PhotoName, x => x.MapFrom(c => c.PhotoPath));

            CreateMap<UserDto, User>()
                .ForMember(c => c.Name, x => x.MapFrom(c => c.Name.Trim()))
                .ForMember(c => c.UserName, x => x.MapFrom(c => Guid.NewGuid().ToString()))
                .ForMember(c => c.Surname, x => x.MapFrom(c => c.Surname.Trim()))
                .ForMember(c => c.Group, x => x.MapFrom(c => c.Group))
                .ForMember(c => c.Email, x => x.MapFrom(c => c.Email))
                .ForMember(c => c.DateOfCreation, x => x.MapFrom(c => DateTime.Now))
                .ForMember(c => c.Subscribers, x => x.MapFrom(c => new List<UserUser>()))
                .ForMember(c => c.Subscriptions, x => x.MapFrom(c => new List<UserUser>()));

            CreateMap<User, UserLoginDto>()
                .ForMember(c => c.Id, x => x.MapFrom(c => c.Id))
                .ForMember(c => c.Email, x => x.MapFrom(c => c.Email));


            CreateMap<Commentary, CommentaryDto>()
                .ForMember(c => c.Id, x => x.MapFrom(p => p.Id))
                .ForMember(c => c.Content, x => x.MapFrom(p => p.Content))
                .ForMember(c => c.DayOfCreation, x => x.MapFrom(p => p.DayOfCreation))
                .ForMember(c => c.VideoId, x => x.MapFrom(p => p.Video.Id))
                .ForMember(c => c.UserId, x => x.MapFrom(p => p.User.Id))
                .ForMember(c => c.UserName, x => x.MapFrom(p => p.User.Name))
                .ForMember(c => c.UserSurname, x => x.MapFrom(p => p.User.Surname))
                .ForMember(c => c.UserPhotoPath, x => x.MapFrom(p => p.User.PhotoPath != null ? configuration.GetSection("Settings:UserPhoto").Value + p.User.PhotoPath : null))
                .ReverseMap();

            CreateMap<Video, VideoDto>()
                .ForMember(v => v.Id, x => x.MapFrom(p => p.Id))
                .ForMember(v => v.Name, x => x.MapFrom(p => p.Name))
                .ForMember(v => v.PhotoPath, x => x.MapFrom(p => p.PhotoPath != null ? configuration.GetSection("Settings:VideoPhoto").Value + p.PhotoPath : null))
                .ForMember(v => v.VideoPath, x => x.MapFrom(p => p.VideoPath != null ? configuration.GetSection("Settings:Video").Value + p.VideoPath : null))
                .ForMember(v => v.PhotoName, x => x.MapFrom(p => p.PhotoPath))
                .ForMember(v => v.VideoName, x => x.MapFrom(p => p.VideoPath))
                .ForMember(v => v.Views, x => x.MapFrom(p => p.Views))
                .ForMember(v => v.Likes, x => x.MapFrom(p => p.Reactions.Where(y => y.IsPositive).Count()))
                .ForMember(v => v.Dislikes, x => x.MapFrom(p => p.Reactions.Where(y => !y.IsPositive).Count()))
                .ForMember(v => v.DayOfCreation, x => x.MapFrom(p => p.DayOfCreation))
                .ForMember(v => v.UserId, x => x.MapFrom(p => p.User.Id))
                .ForMember(v => v.UserName, x => x.MapFrom(c => c.User.Name))
                .ForMember(v => v.UserSurname, x => x.MapFrom(c => c.User.Surname))
                .ForMember(v => v.UserPhoto, x => x.MapFrom(c => c.User.PhotoPath != null ? configuration.GetSection("Settings:UserPhoto").Value + c.User.PhotoPath : null))
                .ReverseMap(); ;

        }
    }
}
