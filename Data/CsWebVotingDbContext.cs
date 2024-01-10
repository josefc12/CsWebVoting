using Microsoft.EntityFrameworkCore;
using cs_web_voting.Models;

namespace cs_web_voting.Data
{
    public class CsWebVotingDbContext : DbContext
    {
        public CsWebVotingDbContext(DbContextOptions<CsWebVotingDbContext> options) : base(options)
        {
        }

        public DbSet<Maps>? maps { get; set;}
        public DbSet<Nominations>? nominations { get; set;}
        public DbSet<Votes>? votes { get; set;}
        public DbSet<Sessions>? sessions { get; set;}
        public DbSet<Voters>? voters { get; set;}
        public DbSet<Passwords>? passwords { get; set;}
        
    }
}