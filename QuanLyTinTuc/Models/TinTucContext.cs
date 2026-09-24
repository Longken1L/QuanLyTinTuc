using System.Data.Entity;

namespace QuanLyTinTuc.Models
{
    public class TinTucContext : DbContext
    {
        public TinTucContext() : base("name=TinTucContext")
        {
        }

        public DbSet<TinTuc> TinTucs { get; set; }
    }
}