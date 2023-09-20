using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace MVC_RecursionPermission.Models
{
    public partial class Context : DbContext
    {
        public Context()
            : base("name=Context")
        {
        }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<NewsCatalog> NewsCatalogs { get; set; }
        public virtual DbSet<News> News { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
