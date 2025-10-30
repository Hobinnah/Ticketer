// ===================================THIS FILE WAS AUTO GENERATED===================================

using Ticketer.Api.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ticketer.Api
{
    public partial class TicketerDbContext : IdentityDbContext<User, Role, long>
    {
         private readonly IConfiguration _configuration;
        public TicketerDbContext(DbContextOptions<TicketerDbContext> options, IConfiguration configuration) : base(options)
        {
             this._configuration = configuration;
        }

        #region ========================================== Database Entities ==========================================
          public virtual DbSet<Ticket> Tickets { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this._configuration.GetConnectionString("DefaultAppConnection"));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // You can seed the tables in your database, after this commented line, 
        }
    }
}
