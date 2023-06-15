namespace TagMyFav_AI.Interfaces
{
    public interface ICategoryHelper
    {
        Task<int> CheckForCategory(int userID);
        Task<int> CreateDefaultCategory(int userID);
    }
}
