namespace backend.Dtos
{
    public class UpdateBookDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Pages { get; set; }
        public IFormFile Image { get; set; }
    }
}
