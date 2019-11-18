using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMS_API.Models
{
    public class CommonModal
    {
        public class paging
        {
            public int Id { get; set; }
            public string AuthKey { get; set; } = null;
            public int PageSize { get; set; }
            public int PageNo { get; set; }
        }
        public class Search : paging
        {
            public string Name { get; set; } = null;
        }
        public class SelectListItem
        {
            public string Value { get; set; } = null;
            public string Text { get; set; } = null;
        }
        public class SelectListItem1
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
        public class IdModal
        {
            public string Id { get; set; }
        }

        public class EmployeeModal
        {
            public string id { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
        }
        public class Details
        {
            public string projectid { get; set; }
            public string CreatedTo { get; set; }
            public string TaskName { get; set; }
            public string Createdon { get; set; }
            public string description { get; set; }
            public string Devicetype { get; set; }

            public string TaskId { get; set; }
        }

        public class PostedFile
        {
            public string fileExt { get; set; }
            public string file { get; set; }
        }

        public class RootObject
        {
            public Details details { get; set; }
            public List<PostedFile> postedFiles { get; set; }
        }

        public class PicVM
        {
            public string ProfilePic { get; set; } = "";
            public string ImagePath { get; set; } = "";
            public string PicExtention { get; set; } = "";
        }
    }
}