/*Madhan KAMALAKANNAN 29/August/2021 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
 
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace onlinesupermartSQLElasticDB.Models
{
    public class AsnetidentityContext : IdentityDbContext<IdentityUser,IdentityRole, string> //DbContext
    {

        protected IConfiguration configuration;

 
        public AsnetidentityContext(DbContextOptions<AsnetidentityContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
        }
         

        //------------------------------------
    }
}
