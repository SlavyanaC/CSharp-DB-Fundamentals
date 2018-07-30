namespace PhotoShare.Services.Contracts
{
    using Models;

    public interface IUserService
    {
        TModel ById<TModel>(int id);

        TModel ByUsername<TModel>(string username);

        TModel ByUsernameAndPassword<TModel>(string username, string password);

        bool Exists(int id);

        bool Exists(string name);

        User Register(string username, string password, string confirmPassword, string email);

        void Delete(string username);

        void Activate(string username);

        Friendship AddFriend(int userId, int friendId);

        Friendship AcceptFriend(int userId, int friendId);

        void ChangePassword(int userId, string password);

        void SetBornTown(int userId, int townId);

        void SetCurrentTown(int userId, int townId);

        void SetFirstName(int userId, string newFirstName);

        void SetLastName(int userId, string newLastName);

        void SetAge(int userId, int newAge);
    }
}
