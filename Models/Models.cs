using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class ContentModel
    {
        public string Html { get; set; }
        public string Title { get; set; }
        public EditHistory History { get; set; }

        public ContentModel()
        {
            History = new EditHistory();
        }
    }

    public class EditHistory
    {
        public IReadOnlyCollection<UserDetail> Authors { get; set; }

        public EditHistory()
        {
            Authors = new List<UserDetail>();
        }
    }

    public class UserDetail
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
    }
}