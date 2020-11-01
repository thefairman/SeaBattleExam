using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattleServer
{
    public class MyAppDbContext : DbContext
    {

        public MyAppDbContext(DbContextOptions<MyAppDbContext> options)
       : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
