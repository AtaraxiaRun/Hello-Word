using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using SqlSugar;
using System.Linq.Expressions;
using PageModel = SqlSugar.PageModel;

namespace SqlSugar2
{
    public class BaseService<T> where T : class, new()
    {
        private readonly IServiceProvider _serviceProvider;

        private IDBContext _dbContext { get; }
        private string _dataSource { get; }
        public BaseService(IServiceProvider serviceProvider)
        {
            //IDBContext
            //_dbContext = AutofacHelper.Resolve<IDBContext>();
            _dbContext = serviceProvider.GetService<IDBContext>();
            //检查是否有配置多数据源，没有则用默认数据源
            var dsAttr = GetType().GetCustomAttributes(typeof(DataSourceAttribute), false);
            if (dsAttr != null && dsAttr.Length > 0)
            {
                var dataSource = dsAttr.First() as DataSourceAttribute;
                _dataSource = dataSource.Name;
            }
            //打印执行sql
            //DBClient.Aop.OnLogExecuting = (sql, pars) =>
            //{
            //    Console.WriteLine("<------------------ sqlSugar start ------------------>");
            //    Console.WriteLine(sql);
            //    Console.WriteLine("<------------------ sqlSugar end ------------------>");

            //};
        }
        private ISqlSugarClient _dbClient;
        /// <summary>
        /// 当前连接
        /// </summary>
        protected ISqlSugarClient DBClient
        {
            get => _dbContext.GetClient(_dataSource);
        }
        /// <summary>
        /// SimpleClient
        /// </summary>
        protected SimpleClient<T> SimpleClient
        {
            get => DBClient.GetSimpleClient<T>();

        }

        public T GetById(dynamic id)
        {
            return DBClient.Queryable<T>().InSingle(id);
        }
        public int Count(Expression<Func<T, bool>> whereExpression)
        {
            return DBClient.Queryable<T>().Where(whereExpression).Count();
        }
        public List<T> GetList()
        {
            return DBClient.Queryable<T>().ToList();
        }
        public T GetFirst(Expression<Func<T, bool>> whereExpression)
        {
            return DBClient.Queryable<T>().First(whereExpression);
        }
        public T GetSingle(Expression<Func<T, bool>> whereExpression)
        {
            return DBClient.Queryable<T>().Single(whereExpression);
        }
        public List<T> GetList(Expression<Func<T, bool>> whereExpression)
        {
            return DBClient.Queryable<T>().Where(whereExpression).ToList();
        }
        public List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel page)
        {
            int count = 0;
            var result = DBClient.Queryable<T>().Where(whereExpression).ToPageList(page.PageIndex, page.PageSize, ref count);
            page.TotalCount = count;
            return result;
        }
        public List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            int count = 0;
            var result = DBClient.Queryable<T>().OrderByIF(orderByExpression != null, orderByExpression, orderByType).Where(whereExpression).ToPageList(page.PageIndex, page.PageSize, ref count);
            page.TotalCount = count;
            return result;
        }
        public List<T> GetPageList(List<IConditionalModel> conditionalList, PageModel page)
        {
            int count = 0;
            var result = DBClient.Queryable<T>().Where(conditionalList).ToPageList(page.PageIndex, page.PageSize, ref count);
            page.TotalCount = count;
            return result;
        }
        public List<T> GetPageList(List<IConditionalModel> conditionalList, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            int count = 0;
            var result = DBClient.Queryable<T>().OrderByIF(orderByExpression != null, orderByExpression, orderByType).Where(conditionalList).ToPageList(page.PageIndex, page.PageSize, ref count);
            page.TotalCount = count;
            return result;
        }
        public bool IsAny(Expression<Func<T, bool>> whereExpression)
        {
            return DBClient.Queryable<T>().Where(whereExpression).Any();
        }
        public bool Insert(T insertObj)
        {
            return DBClient.Insertable(insertObj).ExecuteCommand() > 0;
        }
        public int InsertReturnIdentity(T insertObj)
        {
            return DBClient.Insertable(insertObj).ExecuteReturnIdentity();
        }
        public long InsertReturnBigIdentity(T insertObj)
        {
            return DBClient.Insertable(insertObj).ExecuteReturnBigIdentity();
        }
        public bool InsertRange(T[] insertObjs)
        {
            return DBClient.Insertable(insertObjs).ExecuteCommand() > 0;
        }
        public bool InsertRange(List<T> insertObjs)
        {
            return DBClient.Insertable(insertObjs).ExecuteCommand() > 0;
        }
        public bool Update(T updateObj)
        {
            return DBClient.Updateable(updateObj).ExecuteCommand() > 0;
        }
        public bool UpdateRange(T[] updateObjs)
        {
            return DBClient.Updateable(updateObjs).ExecuteCommand() > 0;
        }
        public bool UpdateRange(List<T> updateObjs)
        {
            return DBClient.Updateable(updateObjs).ExecuteCommand() > 0;
        }
        public bool Update(Expression<Func<T, T>> columns, Expression<Func<T, bool>> whereExpression)
        {
            return DBClient.Updateable(columns).Where(whereExpression).ExecuteCommand() > 0;
        }
        public bool Delete(T deleteObj)
        {
            return DBClient.Deleteable<T>().Where(deleteObj).ExecuteCommand() > 0;
        }
        public bool Delete(Expression<Func<T, bool>> whereExpression)
        {
            return DBClient.Deleteable<T>().Where(whereExpression).ExecuteCommand() > 0;
        }
        public bool DeleteById(dynamic id)
        {
            return DBClient.Deleteable<T>().In(id).ExecuteCommand() > 0;
        }
        public bool DeleteByIds(dynamic[] ids)
        {
            return DBClient.Deleteable<T>().In(ids).ExecuteCommand() > 0;
        }
    }
}
