using lab02;

// ======= Records =======
Book book1 = new Book("1984", "George Orwell", 2011);
Book book2 = new Book("To Kill a Mockingbird", "Harper Lee", 1960);
Book book3 = new Book("The Great Gatsby", "F. Scott Fitzgerald" , 1925);
Book book4 = new Book("The Catcher in the Rye", "Author", 2020);

List<Book> books1 = new List<Book>() { book1, book2, book3, book4 };
Borrower borrower1 = new Borrower(1, "Alice", books1);

Borrower borrower2 = borrower1 with
{
    BorrowedBooks = new List<Book>(borrower1.BorrowedBooks)
    {
        new Book("Brave New World", "Aldous Huxley", 1932)
    }
};

Console.WriteLine("=== Modeling a record task ===");
Console.WriteLine("Borrower1: " + borrower1);
Console.WriteLine("Borrower2: " + borrower2);
Console.WriteLine("");

// ======= Init-only properties =======
Librarian librarian = new Librarian("John Doe", "JognDoe@gmail.com", "Fiction");

Console.WriteLine("=== Use of init-only properties ===");
Console.WriteLine(librarian);
Console.WriteLine("");

// ======= Top Level statements =======
List<Book> books2 = AddBooksToList();

Console.WriteLine("=== Top Level Statements ===");
Console.WriteLine(string.Join( ", ", books2));
Console.WriteLine("");

// ======= Pattern Matching =======

Console.WriteLine("=== Pattern Matching ===");

PatternMatching(borrower1);
PatternMatching(book1);
PatternMatching("Some string");
PatternMatching(books1);

Console.WriteLine("");

// ======= Static Lambda Filtering =======
static Func<List<Book>, List<Book>> FilterBooksByYear()
{
    return books => books.Where(b => b.YearPublished > 2010).ToList();
}

var filter = FilterBooksByYear();
var filteredBooks = filter(books1);

Console.WriteLine("=== Static Lambda Function ===");
Console.WriteLine("Filtered Books: " + string.Join(", ", filteredBooks));
Console.WriteLine("");

// ======= Local Function to add books =======

List<Book> AddBooksToList()
{
    List<Book> books = new List<Book>();
    Console.WriteLine("Write books title: ");
    while (true)
    {
        Console.Write(">");
        string input = Console.ReadLine();
        if (input.ToLower() == "exit")
            break;
        if (!string.IsNullOrWhiteSpace(input))
        {
            books.Add(new Book(input));
        }
    }
    
    return books;
}

// ======= Pattern Matching Function =======

void PatternMatching(object obj)
{
    if (obj is Book)
    {
        Book book = (Book)obj;
        Console.WriteLine($"Book(Title: {book.Title}, Author: {book.Author}, Year: {book.YearPublished})");
    }
    else if (obj is Borrower)
    {
        Borrower borrower = (Borrower)obj;
        Console.WriteLine($"Borrower(Name: {borrower.Name}, BorrowedBooks: { borrower.BorrowedBooks.Count})");

    }
    else
    {
        Console.WriteLine("Unknown type");
    }
}
