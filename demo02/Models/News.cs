using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace demo02.Models
{
    public class News
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}