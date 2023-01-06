using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Assessment.API.Models;
using System.Data;
using Assessment.API.Interface;

namespace Assessment.API.Data
{
    public class UserStore: IUserStore<User>, IUserPasswordStore<User>, IUserRoleStore<User>
    //, IUserEmailStore<User>, IUserPhoneNumberStore<User>, IUserTwoFactorStore<User>, IUserRoleStore<User>
    {
        private readonly string _connectionString;
        private bool disposedValue;

        public UserStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Connection");
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var passwordHash = new PasswordHasher<User>().HashPassword(user, user.Password);

            var queryString = "INSERT INTO [ApiUser] ([FirstName],[LastName],[UserName], [NormalizedUserName], [Email],[NormalizedEmail], [EmailConfirmed], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled]) " +
                "VALUES (@FirstName,@LastName,@UserName, @NormalizedUserName, @Email,@NormalizedEmail, @EmailConfirmed, @PasswordHash, @PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled);SELECT CAST(scope_identity() AS int)";
            using (SqlConnection connection = new SqlConnection(
                       _connectionString))
            {
                SqlCommand cmd = new SqlCommand(queryString, connection);
                cmd.Parameters.Add("@UserName", SqlDbType.VarChar);
                cmd.Parameters.Add("@NormalizedUserName", SqlDbType.VarChar);
                cmd.Parameters.Add("@Email", SqlDbType.VarChar);
                cmd.Parameters.Add("@NormalizedEmail", SqlDbType.VarChar);
                cmd.Parameters.Add("@PasswordHash", SqlDbType.VarChar);
                cmd.Parameters.Add("@EmailConfirmed", SqlDbType.Bit);
                cmd.Parameters.Add("@PhoneNumber", SqlDbType.VarChar);
                cmd.Parameters.Add("@PhoneNumberConfirmed", SqlDbType.Bit);
                cmd.Parameters.Add("@TwoFactorEnabled", SqlDbType.VarChar);
                cmd.Parameters.Add("@FirstName", SqlDbType.VarChar);
                cmd.Parameters.Add("@LastName", SqlDbType.VarChar);

                cmd.Parameters["@UserName"].Value = user.UserName;
                cmd.Parameters["@NormalizedUserName"].Value = user.NormalizedUserName;
                cmd.Parameters["@Email"].Value = user.Email;
                cmd.Parameters["@NormalizedEmail"].Value = user.NormalizedEmail;
                cmd.Parameters["@EmailConfirmed"].Value = user.EmailConfirmed;
                cmd.Parameters["@PasswordHash"].Value = passwordHash;
                cmd.Parameters["@PhoneNumber"].Value = user.PhoneNumber;
                cmd.Parameters["@PhoneNumberConfirmed"].Value = user.PhoneNumberConfirmed;
                cmd.Parameters["@TwoFactorEnabled"].Value = user.TwoFactorEnabled;
                cmd.Parameters["@FirstName"].Value = user.UserName;
                cmd.Parameters["@LastName"].Value = user.UserName;

                try
                {
                    await connection.OpenAsync(cancellationToken);
                    user.Id = (Int32)cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var queryString = $"DELETE FROM [ApiUser] WHERE [Id] = {user.Id}";
            using (SqlConnection connection = new SqlConnection(
                       _connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                SqlCommand cmd = new SqlCommand(queryString, connection);
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }

            return IdentityResult.Success;
        }

        public async Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var queryString = $"SELECT * FROM [ApiUser] WHERE [Id] = {userId}";
            using (SqlConnection connection = new SqlConnection(
                       _connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                SqlCommand cmd = new SqlCommand(queryString, connection);

                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = reader.GetSchemaTable();

                if (reader.HasRows)
                {
                    User user = new User();
                    while (reader.Read())
                    {
                        user.Id = reader.GetInt32(0);
                        user.UserName = reader.GetString(1);
                        user.NormalizedUserName = reader.GetString(2);
                        user.Email = reader.GetString(3);
                        user.NormalizedEmail = reader.GetString(4);
                        user.EmailConfirmed = reader.GetBoolean(5);
                        user.PasswordHash = reader.GetString(6);
                        user.PhoneNumber = reader.GetString(7);
                        user.PhoneNumberConfirmed = reader.GetBoolean(8);
                        user.TwoFactorEnabled = reader.GetBoolean(9);
                        user.FirstName = reader.GetString(10);
                        user.LastName = reader.GetString(11);
                    }

                    return user;

                }
            }
            
            return null;
        }

        public async Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var queryString = $"SELECT * FROM [ApiUser] WHERE [NormalizedUserName] = @normalizedUserName";
            using (SqlConnection connection = new SqlConnection(
            _connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                SqlCommand cmd = new SqlCommand(queryString, connection);

                cmd.Parameters.Add("@normalizedUserName", SqlDbType.VarChar);
                cmd.Parameters["@normalizedUserName"].Value = normalizedUserName;
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = reader.GetSchemaTable();

                if (reader.HasRows)
                {
                    User user = new User();
                    while (reader.Read())
                    {
                        user.Id = reader.GetInt32(0);
                        user.UserName = reader.GetString(1);
                        user.NormalizedUserName = reader.GetString(2);
                        user.Email = reader.GetString(3);
                        user.NormalizedEmail = reader.GetString(4);
                        user.EmailConfirmed = reader.GetBoolean(5);
                        user.PasswordHash = reader.GetString(6);
                        user.PhoneNumber = reader.GetString(7);
                        user.PhoneNumberConfirmed = reader.GetBoolean(8);
                        user.TwoFactorEnabled = reader.GetBoolean(9);
                        user.FirstName = reader.GetString(10);
                        user.LastName = reader.GetString(11);
                    }

                    return user;
                }
            }
            return null;
        }

        public Task<string?> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string?> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(User user, string? normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(User user, string? userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var passwordHash = new PasswordHasher<User>().HashPassword(user, user.Password);
            var queryString = $@"UPDATE [ApiUser] SET
                    [FirstName] = @FirstName,
                    [LastName] = @LastName,
                    [UserName] = @UserName,
                    [NormalizedUserName] = @NormalizedUserName,
                    [Email] = @Email,
                    [NormalizedEmail] = @NormalizedEmail,
                    [EmailConfirmed] = @EmailConfirmed,
                    [PasswordHash] = @PasswordHash,
                    [PhoneNumber] = @PhoneNumber,
                    [PhoneNumberConfirmed] = @PhoneNumberConfirmed,
                    [TwoFactorEnabled] = @TwoFactorEnabled
                    WHERE [Id] = @Id";
            using (SqlConnection connection = new SqlConnection(
            _connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                SqlCommand cmd = new SqlCommand(queryString, connection);
                cmd.Parameters.Add("@UserName", SqlDbType.VarChar);
                cmd.Parameters.Add("@NormalizedUserName", SqlDbType.VarChar);
                cmd.Parameters.Add("@Email", SqlDbType.VarChar);
                cmd.Parameters.Add("@NormalizedEmail", SqlDbType.VarChar);
                cmd.Parameters.Add("@PasswordHash", SqlDbType.VarChar);
                cmd.Parameters.Add("@EmailConfirmed", SqlDbType.Bit);
                cmd.Parameters.Add("@PhoneNumber", SqlDbType.VarChar);
                cmd.Parameters.Add("@PhoneNumberConfirmed", SqlDbType.Bit);
                cmd.Parameters.Add("@TwoFactorEnabled", SqlDbType.VarChar);
                cmd.Parameters.Add("@Id", SqlDbType.Int);
                cmd.Parameters.Add("@FirstName", SqlDbType.VarChar);
                cmd.Parameters.Add("@LastName", SqlDbType.VarChar);

                cmd.Parameters["@Id"].Value = user.Id;
                cmd.Parameters["@UserName"].Value = user.UserName;
                cmd.Parameters["@NormalizedUserName"].Value = user.NormalizedUserName;
                cmd.Parameters["@Email"].Value = user.Email;
                cmd.Parameters["@NormalizedEmail"].Value = user.NormalizedEmail;
                cmd.Parameters["@EmailConfirmed"].Value = user.EmailConfirmed;
                cmd.Parameters["@PasswordHash"].Value = passwordHash;
                cmd.Parameters["@PhoneNumber"].Value = user.PhoneNumber;
                cmd.Parameters["@PhoneNumberConfirmed"].Value = user.PhoneNumberConfirmed;
                cmd.Parameters["@TwoFactorEnabled"].Value = user.TwoFactorEnabled;
                cmd.Parameters["@FirstName"].Value = user.UserName;
                cmd.Parameters["@LastName"].Value = user.UserName;
                cmd.ExecuteNonQuery();
            }

            return IdentityResult.Success;
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~UserData()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

        }

        public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

        }

        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return true;
        }

        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }
}
