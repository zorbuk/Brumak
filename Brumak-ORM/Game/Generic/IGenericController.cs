using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_ORM.Game.Generic
{
    public interface IGenericController { }

    public interface IGenericController<TEntity> where TEntity : class
    {
        bool Create(TEntity entity);
        TEntity? GetById(int id);
        List<TEntity> GetAll();
        bool Update(TEntity entity);
        bool Delete(int id);
        void Save();
    }
}
