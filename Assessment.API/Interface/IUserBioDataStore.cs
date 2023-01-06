namespace Assessment.API.Interface
{
    public interface IUserBioDataStore<T>
    {
        public Task<IEnumerable<T>> GetAllUsers();
    }
}
