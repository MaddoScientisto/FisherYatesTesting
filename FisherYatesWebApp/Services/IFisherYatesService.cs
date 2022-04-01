namespace FisherYatesWebApp.Services
{
    public interface IFisherYatesService
    {
        string Shuffle(string input, int? seed = null);
        void Shuffle<T>(T[] array, int? seed = null);       
    }
}
