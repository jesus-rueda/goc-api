﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Goc.Models
{
    public partial class Character
    {
        public Character()
        {
            //ActionsLog = new HashSet<ActionsLog>();
            //Evidences = new HashSet<Evidences>();
            TeamsCharacters = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Story { get; set; }
        public string Attack { get; set; }
        public string Defense { get; set; }
        public string Bonus { get; set; }

        //public virtual ICollection<ActionsLog> ActionsLog { get; set; }
        //public virtual ICollection<Evidences> Evidences { get; set; }
        [JsonIgnore]
        public virtual ICollection<User> TeamsCharacters { get; set; }
    }
}