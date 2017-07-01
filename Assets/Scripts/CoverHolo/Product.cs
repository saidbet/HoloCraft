[System.Serializable]
public class Product {
    public int Id;
    public string Name;
    public string Description;
    public string Document;
    public string Thumbnail;

    public Product(int id, string name, string description, string document, string thumbnail)
    {
        Id = id;
        Name = name;
        Description = description;
        Document = document;
        Thumbnail = thumbnail;
    }

    public Product()
    {
        Id = 0;
        Name = "";
        Description = "";
        Document = "";
        Thumbnail = "";
    }
}
