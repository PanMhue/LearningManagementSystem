namespace OjtProgramApi.Repositories
{
    public interface IRepositoryBase<T>
        where T : class
    {
        Task<IEnumerable<RT>> GetAll<RT>(string query, object parameters); // dapper
        Task<int> EditData(string query, object parameters); // dapper
        Task<T?> FindByID(int id);

        Task<T?> FindByCondition(string query, object parameters);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);

        Task Save();
    }
}
