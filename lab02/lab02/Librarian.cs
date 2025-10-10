namespace lab02;

public class Librarian
{
    public string Name { get; init; }
    public string Email { get; init; }
    public string LibrarySection { get; init; }
    public Librarian(string name, string email, string librarySection)
    {
        Name = name;
        Email = email;
        LibrarySection = librarySection;
    }
    
    public override string ToString()
    {
        return $"Librarian(Name: {Name}, Email: {Email}, LibrarySection: {LibrarySection})";
    }
}