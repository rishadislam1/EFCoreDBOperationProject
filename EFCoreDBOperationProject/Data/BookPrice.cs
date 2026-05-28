namespace EFCoreDBOperationProject.Data
{
    public class BookPrice
    {
        public int Id { get; set; }
        public int BooksId { get; set; }
        public int CurrencyId { get; set; }
        public int Amount { get; set; }
        public virtual Books Books { get; set; }
        public virtual Currency Currency { get; set; }
    }
}
