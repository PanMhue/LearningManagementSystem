using Microsoft.EntityFrameworkCore.Query;

namespace OjtProgramApi.Repositories
{
    public class RepositoryBase<T> : IRepositoryBase<T>
        where T : class
    {
        protected readonly AppDB _context;

        public RepositoryBase(AppDB context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RT>> GetAll<RT>(string query, object parameters)
        {
            return await _context.GetAll<RT>(query, parameters);
        }

        public async Task<int> EditData(string query, object parameters)
        {
            return await _context.EditData(query, parameters);
        }

        public async Task<T?> FindByID(int id)
        {
            return await _context.FindAsync<T>(id) ?? null;
        }

        public async Task<T?> FindByCondition(string query, object parameters)
        {
            return await _context.FindAsync<T>(query, parameters) ?? null;
        }

        public void Create(T entity)
        {
            _context.Add(entity);
        }

        public void Update(T entity)
        {
            _context.Update(entity);
        }

        public void Delete(T entity)
        {
            _context.Remove(entity);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
