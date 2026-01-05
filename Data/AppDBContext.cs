using Lpgin2.Models.Entities;
using Lpgin2.Models.MTM;
using Microsoft.EntityFrameworkCore;

namespace Lpgin2.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<clsDoctor>()
                .HasOne(d => d.User)
                .WithOne(u => u.doctor)
                .HasForeignKey<clsDoctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<clsStudent>()
                .HasOne(s => s.User)
                .WithOne(u => u.student)
                .HasForeignKey<clsStudent>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<clsAdmin>()
                .HasOne(a => a.User)
                .WithOne(u => u.admin)
                .HasForeignKey<clsAdmin>(a => a.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<clsCourseOffering>()
                .HasOne(o => o.Course)
                .WithMany(c => c.Offerings)
                .HasForeignKey(o => o.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<clsCourseOffering>()
                .HasOne(o => o.doctor)
                .WithMany(d => d.Offerings)
                .HasForeignKey(o => o.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

           
            modelBuilder.Entity<clsEnrollments>()
                .HasOne(e => e.CourseOffering)
                .WithMany(o => o.Enrollments)
                .HasForeignKey(e => e.CourseOfferingId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<clsEnrollments>()
                .HasOne(e => e.Student) 
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<clsStudentMark>()
                .HasOne(sm => sm.Enrollment)
                .WithMany(e => e.StudentMarks)
                .HasForeignKey(sm => sm.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<clsStudentMark>()
                .HasOne(sm => sm.ExamType)
                .WithMany(et => et.StudentMarks)
                .HasForeignKey(sm => sm.ExamTypeId);

            modelBuilder.Entity<clsStudentMark>()
               .Property(sm => sm.Mark)
               .HasPrecision(5, 2);

  



            modelBuilder.Entity<clsExamType>().HasData(
               new clsExamType { Id = 1, Name = "First Exam", MaxMark = 20 },
               new clsExamType { Id = 2, Name = "Second Exam", MaxMark = 20 },
               new clsExamType { Id = 3, Name = "Final Exam", MaxMark = 60 }
);
        }

        public DbSet<clsAdmin> admins { get; set; }
        public DbSet<clsCourse> courses { get; set; }
        public DbSet<clsStudent> students { get; set; }
        public DbSet<clsDoctor> doctors { get; set; }
        public DbSet<clsUser> users { get; set; }
        public DbSet<clsCourseOffering> courseOfferings { get; set; }
        public DbSet<clsEnrollments> enrollments { get; set; }
        public DbSet<clsStudentMark> StudentMarks { get; set; }
        public DbSet<clsExamType> ExamTypes { get; set; }
    }
}
