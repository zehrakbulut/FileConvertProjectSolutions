using FileConvertProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FileConvertProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<FileConvert>
            fileConverts
        { get; set; }
        public DbSet<ApplicationUser> applicationUsers { get; set; }
        public DbSet<FileRange> FileRanges { get; set; }

    }
}