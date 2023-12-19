namespace FullStackAuth_WebAPI.DataTransferObjects
{
    public class BookDetailsDto
    {
        public List<ReviewWithUserDto> Reviews { get; set; }
        public double Average { get; set; }
        public bool Favorited { get; set; }
    }
}
