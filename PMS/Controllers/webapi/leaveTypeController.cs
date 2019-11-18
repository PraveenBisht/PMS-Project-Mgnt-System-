using PMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;

namespace PMS.Controllers.webapi
{
    public class leaveTypeController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<object> Get()
        {
            PMSEntities DbLMS = new PMSEntities();
            var LeaveType = DbLMS.Leave_Management_Leave_Type;
            List<Leave_Management_Leave_Type> objLeaveType = new List<Leave_Management_Leave_Type>();
            foreach (var x in LeaveType)
            {
                Leave_Management_Leave_Type ObjLeave_Management_Leave_Type = new Leave_Management_Leave_Type();
                ObjLeave_Management_Leave_Type.pk_leave_type_id = x.pk_leave_type_id;
                ObjLeave_Management_Leave_Type.leave_type = x.leave_type;
                objLeaveType.Add(ObjLeave_Management_Leave_Type);
            }
            return objLeaveType;
        }
       

        // POST api/<controller>
        public bool Post(Leave_Management_Leave_Type newType)
        {
            try
            {
                PMSEntities DbLMS = new PMSEntities();

                DbLMS.Leave_Management_Leave_Type.Add(newType);
                DbLMS.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return true;
            }
            
        }

        // PUT api/<controller>/5
        public bool Post(int id, Leave_Management_Leave_Type newType)
        {
            try
            {
                PMSEntities DbLMS = new PMSEntities();
                var SelectedRow = DbLMS.Leave_Management_Leave_Type.FirstOrDefault(x => x.pk_leave_type_id == id);
                SelectedRow.leave_type = newType.leave_type;
                DbLMS.Entry(SelectedRow).State = EntityState.Modified;
                DbLMS.SaveChanges();
               
                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}