using System;

namespace Starbucks
{
    public interface ISagaRepository<TSaga> where TSaga : class, ISaga
    {
        TSaga GetById(Guid id);
        void Save(TSaga saga);
    }
}