namespace WarehouseManagementSystem.Application.Products.DTOs
{
    public class ImportResultDto
    {
        public int SuccessCount { get; set; }
        public List<string> Errors { get; private set; } = new List<string>();
        public bool HasErrors => Errors.Any();
    }
}
