using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_RecursionPermission.Models
{
    public class News
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "編號")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "消息名稱")]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "消息內容")]
        [MaxLength(100)]
        public string Description { get; set; }

        [Display(Name = "消息類別")]
        public int? NewsCatalogId { get; set; }
        [ForeignKey("NewsCatalogId")]
        public virtual NewsCatalog MyNewsCatalog { get; set; }
    }
}