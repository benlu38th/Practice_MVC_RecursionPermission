using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_RecursionPermission.Models
{
    public class NewsCatalog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "編號")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "分類")]
        [MaxLength(20)]
        public string Catalos { get; set; }

        public virtual ICollection<News> News { get; set; }
    }
}