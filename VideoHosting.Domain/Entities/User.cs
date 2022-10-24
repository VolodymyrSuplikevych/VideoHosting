using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VideoHosting.Domain.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Faculty { get; set; }

        public string Group { get; set; }

        public int? TempPassword { get; set; }

        public DateTime DateOfCreation { get; set; }

        public string PhotoPath { get; set; }

        public virtual List<Video> Videos { get; set; }

        public virtual List<Commentary> Commentaries { get; set; }

        public virtual List<UserUser> Subscribers { get; set; }

        public virtual List<UserUser> Subscriptions { get; set; }

        public virtual List<VideoUser> Reactions { get; set; }

        public User()
        {
            Videos = new List<Video>();
            Commentaries = new List<Commentary>();

            Subscribers = new List<UserUser>();
            Subscriptions = new List<UserUser>();

            Reactions = new List<VideoUser>();
        }

        public void AddLike(Video video)
        {
            if (Reactions.FirstOrDefault(x => x.Video == video && !x.IsPositive) != null)
            {
                Reactions.FirstOrDefault(x => x.Video == video && !x.IsPositive).IsPositive = true;
                return;
            }

            if (Reactions.FirstOrDefault(x => x.Video == video && x.IsPositive) != null)
            {
                Reactions.Remove(Reactions.FirstOrDefault(x => x.Video == video && x.IsPositive));
                return;
            }

            if(Reactions.FirstOrDefault(x => x.Video == video) == null)
            {
                Reactions.Add(new VideoUser { Video = video, User = this, IsPositive = true });
                return;
            }
        }

        public void AddDislike(Video video)
        {
            if (Reactions.FirstOrDefault(x => x.Video == video && x.IsPositive) != null)
            {
                Reactions.FirstOrDefault(x => x.Video == video && x.IsPositive).IsPositive = false;
                return;
            }

            if (Reactions.FirstOrDefault(x => x.Video == video && !x.IsPositive) != null)
            {
                Reactions.Remove(Reactions.FirstOrDefault(x => x.Video == video && !x.IsPositive));
                return;
            }

            if (Reactions.FirstOrDefault(x => x.Video == video) == null)
            {
                Reactions.Add(new VideoUser { Video = video, User = this, IsPositive = true });
                return;
            }
        }

        public void Subscribe(User user)
        {
            if (Subscriptions.FirstOrDefault(x => x.Subscripter == user) == null)
            {
                Subscriptions.Add(new UserUser { Subscriber = this, Subscripter = user });
            }
        }

        public void Unsubscribe(User user)
        {
            if (Subscriptions.FirstOrDefault(x => x.Subscripter == user) != null)
            {
                Subscriptions.Remove(Subscriptions.FirstOrDefault(x => x.Subscripter == user));
            }
        }
    }
}
