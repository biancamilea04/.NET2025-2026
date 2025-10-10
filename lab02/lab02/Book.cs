public record Book(string Title, string Author, int YearPublished)
{
    public Book(string title) : this(title, "Unknown", 0) { }
}