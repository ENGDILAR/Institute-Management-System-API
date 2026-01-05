namespace Lpgin2.DTOs.Response
{
    public class DoctorResponseDTO
    {
        public int doctorID { set; get; }
        public required string FirstName { set; get; }
        public required string LastName { set; get; }
        public required string Address { set; get; }
        public required string Phone { set; get; }
        public required string Email { set; get; }

        public string? EPhone { set; get; }
        public int UserID { set; get; }
    }
}
