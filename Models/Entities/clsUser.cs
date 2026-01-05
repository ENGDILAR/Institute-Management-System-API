
namespace Lpgin2.Models.Entities
{
    public class clsUser
    {
       
        public int id { set; get; }
        public required string Email { set; get; }
        public required string Password { set; get; }
        public Enums.enUserRoles role { set; get; }

        // this is how we define Relations
        public clsDoctor? doctor { get; set; }
        public clsAdmin? admin { get; set; }

        public clsStudent? student { get; set; }

    }
}
