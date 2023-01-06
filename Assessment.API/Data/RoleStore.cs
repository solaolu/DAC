using Microsoft.AspNetCore.Identity;
using Assessment.API.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace Assessment.API.Data
{
    public class RoleStore : IRoleStore<UserRoles>
    {
        private readonly string _connectionString;
        private bool disposedValue;

        public RoleStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Connection");
        }

        public async Task<IdentityResult> CreateAsync(UserRoles role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                var queryString = $@"INSERT INTO [ApiUserRoles] ([Name], [NormalizedName])
                    VALUES (@Name, @NormalizedName));
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                SqlCommand cmd = new SqlCommand(queryString, connection);

                cmd.Parameters.Add("@Name", SqlDbType.VarChar);
                cmd.Parameters.Add("@NormalizedName", SqlDbType.VarChar);
                cmd.Parameters["@Name"].Value = role.Name;
                cmd.Parameters["@NormalizedName"].Value = role.NormalizedName;
                try
                {
                    role.Id = (Int32)cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(UserRoles role, CancellationToken cancellationToken)
        {

            cancellationToken.ThrowIfCancellationRequested();
            var queryString = $"DELETE FROM [ApiUserRoles] WHERE [Id] = {role.Id}";
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

        public async Task<UserRoles?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var queryString = $"SELECT * FROM [ApiUserRoles] WHERE [Id] = {roleId}";
            using (SqlConnection connection = new SqlConnection(
                       _connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                SqlCommand cmd = new SqlCommand(queryString, connection);

                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = reader.GetSchemaTable();

                if (reader.HasRows)
                {
                    UserRoles role = new UserRoles();
                    while (reader.Read())
                    {
                        role.Id = reader.GetInt32(0);
                        role.Name = reader.GetString(1);
                        role.NormalizedName = reader.GetString(2);
                    }

                    return role;

                }
            }

            return null;
        }

        public async Task<UserRoles?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var queryString = $"SELECT * FROM [ApiUserRoles] WHERE [Id] = '{normalizedRoleName}'";
            using (SqlConnection connection = new SqlConnection(
                       _connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                SqlCommand cmd = new SqlCommand(queryString, connection);

                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = reader.GetSchemaTable();

                if (reader.HasRows)
                {
                    UserRoles role = new UserRoles();
                    while (reader.Read())
                    {
                        role.Id = reader.GetInt32(0);
                        role.Name = reader.GetString(1);
                        role.NormalizedName = reader.GetString(2);
                    }

                    return role;

                }
            }
            return null;
        }

        public Task<string?> GetNormalizedRoleNameAsync(UserRoles role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(UserRoles role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string?> GetRoleNameAsync(UserRoles role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(UserRoles role, string? normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetRoleNameAsync(UserRoles role, string? roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.FromResult(0);
        }

        public Task<IdentityResult> UpdateAsync(UserRoles role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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
        // ~RoleStore()
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
    }
}
