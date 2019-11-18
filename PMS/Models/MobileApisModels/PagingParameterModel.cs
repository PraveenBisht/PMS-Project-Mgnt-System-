using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMS_API.Models
{
    public class PagingParameterModel
    {
        public string Id { get; set; }

        const int maxPageSize = 20;
        public int pageNumber { get; set; } = 1;
        public int _pageSize { get; set; } = 10;
        public int pageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
    public class leaveList : PagingParameterModel
    {
        public string leave_status { get; set; }
        public string RoleType { get; set; }
        public string RoleType1 { get; set; }
        public string FromDate { get; set; }
        public string Todate { get; set; }
    }
    public class TaskList : PagingParameterModel
    {
        public string FromDate { get; set; }
        public string Todate { get; set; }
    }
}