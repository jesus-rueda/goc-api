﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Goc.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    [Table("Users")]
    public partial class User : IUser
    {
        public User()
        {
            ActionsLog = new HashSet<ActionsLog>();
            Evidences = new HashSet<Evidences>();
        }

        public int Id { get; set; }
        public int? TeamId { get; set; }
        public int? CharacterId { get; set; }
        public string Upn { get; set; }
        public bool IsLeader { get; set; }
        public bool IsAdmin { get; set; }

        public bool PendingAproval { get; set; }

        [JsonIgnore]
        public virtual ICollection<ActionsLog> ActionsLog { get; set; }
        [JsonIgnore]
        public virtual ICollection<Evidences> Evidences { get; set; }
        [JsonIgnore]
        public virtual Character? Character { get; set; }
        [JsonIgnore]
        public virtual Team? Team { get; set; }
    }

    public interface IUser
    {
        
        public int Id { get; set; }
        
        public int? TeamId { get; set; }
        
        public int? CharacterId { get; set; }
        
        public string Upn { get; set; }
        
        public bool IsLeader { get; set; }
        
        public bool IsAdmin { get; set; }
    }
}