public record Borrower(int Id, string Name, List<Book> BorrowedBooks)
{
    public override string ToString()
    {
        return $"Borrower(Id: {Id}, Name: {Name}, BorrowedBooks: [{string.Join(", ", BorrowedBooks)}])";
    }
}