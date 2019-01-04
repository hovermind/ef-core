using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbFirst.Repo
{
[Table("Post")]
    public partial class Post
    {
        public int PostId { get; set; }
        public int BlogId { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }

        [ForeignKey("BlogId")]
        [InverseProperty("Posts")]
        public Blog Blog { get; set; }
    }
}
