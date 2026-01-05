namespace Lpgin2.Models.Entities
{
    public class Enums
    {
        public enum enUserRoles 
        {
            admin=1,
            doctor=2,
            student=3
        };

        public enum enCourseOfferingStatus
        {
            Draft = 1,
            Open = 2,
            Closed = 3,
            Cancelled = 4
        }

        public enum enExamTypes
        {
            First=1,
            Secound=2,
            Final=3
        }
    }
}
