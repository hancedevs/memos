using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend
{
    public class MemoDbContext: DbContext
{
   public DbSet<Planner> Planners { get; set; }
    public DbSet<WeddingStory> Weddings { get; set; }
    public DbSet<Media> Media { get; set; }
    public DbSet<WQRCode> QRCodes { get; set; }
    public DbSet<GuestMessage> GuestMessages { get; set; }

    public MemoDbContext(DbContextOptions<MemoDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Planner>().HasKey(p => p.Id);
        modelBuilder.Entity<WeddingStory>().HasKey(w => w.Id);
        modelBuilder.Entity<Media>().HasKey(m => m.Id);
        modelBuilder.Entity<WQRCode>().HasKey(q => q.Id);
        modelBuilder.Entity<GuestMessage>().HasKey(q => q.Id);

      

        modelBuilder.Entity<Media>()
            .HasOne(m => m.Wedding)
            .WithMany(w => w.Gallery)
            .HasForeignKey(m => m.WeddingId);

        modelBuilder.Entity<WQRCode>()
            .HasOne(q => q.Wedding)
            .WithOne(w => w.QRCode)
            .HasForeignKey<WQRCode>(q => q.WeddingId);
        modelBuilder.Entity<GuestMessage>()
            .HasOne(q => q.Wedding)
            .WithMany(w => w.GuestMessages)
            .HasForeignKey(q => q.WeddingId);
            
        }
}
}