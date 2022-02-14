using System.Collections.Generic;
using Entities.Models;

namespace Interfaces
{
    public interface IDataShaper<T>
    {
        IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string fieldsString);

        ShapedEntity ShapeData(T entity, string fieldsString);
    }
}