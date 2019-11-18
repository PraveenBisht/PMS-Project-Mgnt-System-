using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace PMS.Models.Tag
{
    public class TagModel : BaseModel
    {
        public int ID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Tag Description")]
        public string TagDescription { get; set; }
    }
}