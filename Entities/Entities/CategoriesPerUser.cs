using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ReadLater.Entities
{
    public class CategoriesPerUser : EntityBase
    {

        [Key]
        public int ID { get; set; }

        public string UserID { get; set; }

        public int? CategoryID { get; set; }

        public virtual Category Category { get; set; }

    }
}
