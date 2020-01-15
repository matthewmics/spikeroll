public class SelectionSingleton
{
    private SelectionSingleton() { }

    private static SelectionSingleton _instance;

    public static SelectionSingleton instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new SelectionSingleton();
            }

            return _instance;
        }
    }

    public VersusType VersusType { get; set; }

    public int NumberOfPlayers { get; set; }

    public CountryModel PlayerCountry { get; set; }

    public CountryModel OpponentCountry { get; set; }
}
