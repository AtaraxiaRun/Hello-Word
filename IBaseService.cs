using SqlSugar;
using System.Linq.Expressions;

namespace SqlSugar2
{
    public interface IBaseService<T> where T : class, new()
    {
        T GetById(dynamic id);
        int Count(Expression<Func<T, bool>> whereExpression);
        List<T> GetList();
        T GetFirst(Expression<Func<T, bool>> whereExpression);
        T GetSingle(Expression<Func<T, bool>> whereExpression);
        List<T> GetList(Expression<Func<T, bool>> whereExpression);
        List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel page);
        List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc);
        List<T> GetPageList(List<IConditionalModel> conditionalList, PageModel page);
        List<T> GetPageList(List<IConditionalModel> conditionalList, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc);
        bool IsAny(Expression<Func<T, bool>> whereExpression);
        bool Insert(T insertObj);
        int InsertReturnIdentity(T insertObj);
        long InsertReturnBigIdentity(T insertObj);
        bool InsertRange(T[] insertObjs);
        bool InsertRange(List<T> insertObjs);
        bool Update(T updateObj);
        bool UpdateRange(T[] updateObjs);
        bool UpdateRange(List<T> updateObjs);
        bool Update(Expression<Func<T, T>> columns, Expression<Func<T, bool>> whereExpression);
        bool Delete(T deleteObj);
        bool Delete(Expression<Func<T, bool>> whereExpression);
        bool DeleteById(dynamic id);
        bool DeleteByIds(dynamic[] ids);
    }
}
