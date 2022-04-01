namespace FisherYatesWebApp.Services
{
    public interface IFisherYatesService
    {
        void Shuffle<T>(T[] array, int? seed = null);       
    }
}
