using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrystalByRiya.Models;
using CrystalByRiya.StoredProcedure;
using CrystalByRiya.Pages;
using CrystalByRiya.Areas.Admin.Pages.Products;


namespace CrystalByRiya.Models
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }
        public void DetachAllEntities()
        {
            var changedEntriesCopy = this.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductBySkuCode>().HasNoKey().ToView(null);
            modelBuilder.Entity<BestSeller>().HasNoKey().ToView(null);
            modelBuilder.Entity<SPCategoryWiseProduct>().HasNoKey().ToView(null);
            modelBuilder.Entity<SPSubCategoryWiseProduct>().HasNoKey().ToView(null);
           
            

        }
        public virtual DbSet<TblCategory> TblCategory { get; set; }
        public virtual DbSet<Subcategory> TblSubcategory { get; set; }
        public virtual DbSet<Product> TblProducts { get; set; }
        public virtual DbSet<Blogs> TblBlogs { get; set; }
        public virtual DbSet<Banner> TblBanners { get; set; }
        public virtual DbSet<Instagram> TblInstagram { get; set; }
        public virtual DbSet<BlogFaq> TblBlogsFaq { get; set; }
        public virtual DbSet<Register> TblRegisters { get; set; }
        public virtual DbSet<Announcement> TblAnnouncement { get; set; }
        public virtual DbSet<ProductFaq> TblProductFaq { get; set; }
        
        public virtual DbSet<ImageGallery> TblImageGalleries { get; set; }
        public virtual DbSet<ProductSizes> TblProductSizes { get; set; }
        public virtual DbSet<ReviewGallery> TblReviewGallery { get; set; }
        public virtual DbSet<TblReviews> TblReviews { get; set; }
        public virtual DbSet<CategoryWiseProduct> TblCategoryWiseProduct { get; set; }
        public virtual DbSet<TblOrderId> TblOrderIds { get; set; }
        public virtual DbSet<TblBillingDetail> TblBillingDetails { get; set; }

        public virtual DbSet<TblShippingDetail> TblShippingDetails { get; set; }
        public virtual DbSet<CouponCodes> CouponCodes { get; set; }

        public virtual DbSet<TblCustomerOrderDetails> TblCustomerOrderDetails { get; set; }

        public virtual DbSet<AdminLogin> TblAdminLogin { get; set; }
        public virtual DbSet<MailCredentials> TblMailCredentials { get; set; }


        public DbSet<ExceptionLog> ExceptionLogs { get; set; }
        public DbSet<Employee> TblEmployee { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<ContactUs> TblContactUs { get; set; }

        public DbSet<NewsLetter> TblNewsLetters { get; set; }
        public DbSet<CommentReply> TblCommentReply { get; set; }
        public DbSet<AddingWishlist> TblWishlist { get; set; }
        public DbSet<AddOn> TblAddOn { get; set; }
        public DbSet<RelatedProduct> RelatedProducts { get; set; }
        public DbSet<Cart> TblCarts { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        #region StoredProcedure
        public virtual DbSet<ProductBySkuCode> ProductBySkuCodes { get; set; }
        public virtual DbSet<BestSeller> SpBestSeller { get; set; }
        public virtual DbSet<SPCategoryWiseProduct> SPCategoryWiseProduct { get; set; }
        public virtual DbSet<SPSubCategoryWiseProduct> SPSubCategoryWiseProduct { get; set; }

        #endregion




    }

}
