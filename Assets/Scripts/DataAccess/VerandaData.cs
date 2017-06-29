[System.Serializable]
public class VerandaData {
    public string Error { get; private set; }
    public bool Valid { get; private set; }
    public Product Veranda { get; set; }    

    public VerandaData()
    {

    }

    public VerandaData(string error, bool valid, Product veranda)
    {
        Error = error;
        Valid = valid;
        Veranda = veranda;
    }
}
