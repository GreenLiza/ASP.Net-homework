using GoodNewsAggregator.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Data
{
    public class NewsAggregatorContext : DbContext
    {
        public DbSet<Comment> Comments { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<RatingWord> RatingWords { get; set; }
        public DbSet<Source> Sources { get; set; }     
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public NewsAggregatorContext(DbContextOptions<NewsAggregatorContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        
    }
}
