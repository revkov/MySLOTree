using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace MySLOTree.Models
{
    public class NewsContext : DbContext
    {
        public NewsContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<News> News { get; set; }
    }

    [Table("News")]
    public class News
    {
        public News()
        {
            IsDeleted = false;
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Title { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class NewsModel
    {
        internal NewsModel(News news)
        {
            this.Id = news.Id;
            this.ParentId = news.ParentId;
            this.Title = news.Title;
        }

        public int Id {get; set; }
        public int? ParentId {get; set; }
        public string Title {get;set;}
    }

    public class NewsListModel
    {
        public int? Seed { get; set; }
        public IEnumerable<NewsModel> News { get; set; }
    }

    
}
