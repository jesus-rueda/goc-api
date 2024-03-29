﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Goc.Models
{
    public partial class Team
    {
        public Team()
        {
            ActionsLog = new HashSet<ActionsLog>();
            MessagesRecipientTeamNavigation = new HashSet<Messages>();
            MessagesSenderTeamNavigation = new HashSet<Messages>();
            TeamsCharacters = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Coinks { get; set; }

        //TODO: Avoid load the image in all the queries
        public byte[]? Image { get; set; }

        public virtual ICollection<ActionsLog> ActionsLog { get; set; }
        public virtual ICollection<Messages> MessagesRecipientTeamNavigation { get; set; }
        public virtual ICollection<Messages> MessagesSenderTeamNavigation { get; set; }
        public virtual ICollection<User> TeamsCharacters { get; set; }
    }
}