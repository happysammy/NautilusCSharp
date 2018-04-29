namespace Nautilus.BlackBox
{
    using System.Linq;
    using Nautilus.BlackBox.Core.Interfaces;

    public class DummyDatabase : IDatabaseAdapter
    {
        public void OpenConnection()
        {
        }

        public IQueryable<T> Query<T>()
        {
            return null;
        }

        public void Delete(object entity)
        {
        }

        public void Store(object entity)
        {
        }

        public void SaveChanges()
        {
        }

        public void Dispose()
        {
        }
    }
}
