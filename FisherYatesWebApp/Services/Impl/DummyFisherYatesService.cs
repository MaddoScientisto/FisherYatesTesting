namespace FisherYatesWebApp.Services.Impl
{
    public class DummyFisherYatesService : IFisherYatesService
    {
        public void Shuffle<T>(T[] array, int? seed = null)
        {
            throw new System.NotImplementedException();
        }

        public string Shuffle(string input, int? seed = null)
        {
            throw new System.NotImplementedException();
        }
    }
}
