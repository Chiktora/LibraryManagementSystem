namespace LibraryManagementSystem.Data
{
    /// <summary>
    /// Contains configuration settings for the application.
    /// Provides centralized access to important configuration values.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// The connection string for the application's database.
        /// This string contains all necessary information to connect to the SQL Server database.
        /// </summary>
        public const string ConnectionString = @"Server=LAPTOP-13GAAT86\SQLEXPRESS; Database=LibraryDB; Integrated security = true; TrustServerCertificate=True";
    }
}
