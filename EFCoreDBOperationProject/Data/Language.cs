namespace EFCoreDBOperationProject.Data
{
    public class Language
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<Books> Books { get; set; }
    }
}
