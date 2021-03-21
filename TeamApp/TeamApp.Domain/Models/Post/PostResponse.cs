﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Domain.Models.Post
{
    public class PostResponse
    {
        public string PostId { get; set; }
        public string PostUserId { get; set; }
        public string PostTeamId { get; set; }
        public string PostContent { get; set; }
        public DateTime? PostCreatedAt { get; set; }
        public int? PostCommentCount { get; set; }
        public bool? PostIsDeleted { get; set; }
        public bool? PostIsPinned { get; set; }
    }
}