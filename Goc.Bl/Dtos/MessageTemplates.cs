﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Goc.Api.Dtos
{
    public partial class MessageTemplates
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public int ActionTypeId { get; set; }

        public virtual ActionTypesDto ActionType { get; set; }
    }
}