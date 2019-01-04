using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbFirst.Repo
{
[Table("Blog")]
    public partial class Blog
    {
        public Blog()
        {
            Posts = new HashSet<Post>();
        }
        [Key]
        public int BlogId { get; set; }
        [Required]
        public string Url { get; set; }

        [InverseProperty("Blog")]
        public ICollection<Post> Posts { get; set; }
    }
}
