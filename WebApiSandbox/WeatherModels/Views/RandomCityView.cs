namespace Models.Views;

public class RandomCityView
{
    public int     CityId        { get; set; }
    public decimal Latitude      { get; set; }
    public decimal Longitude     { get; set; }
    public long    ForecastCount { get; set; }
}