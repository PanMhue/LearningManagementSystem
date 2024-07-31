using Dapper;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using OjtProgramApi.Models;

public class AppDB : DbContext
{
    readonly string connectionString = "";

    public AppDB(DbContextOptions<AppDB> options)
        : base(options)
    {
        var appsettingbuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        var Configuration = appsettingbuilder.Build();
        connectionString = Configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<T?> GetAsync<T>(string command, object parms)
    {
        T? result;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            result = (
                await connection.QueryAsync<T>(command, parms).ConfigureAwait(false)
            ).FirstOrDefault();
        }
        return result;
    }

    public async Task<List<T>> GetAll<T>(string command, object parms)
    {
        List<T> result = new List<T>();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            result = (await connection.QueryAsync<T>(command, parms)).ToList();
        }
        return result;
    }

    public async Task<int> EditData(string command, object parms)
    {
        int result;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            result = await connection.ExecuteAsync(command, parms);
        }
        return result;
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<RolePermission> RolePermissions { get; set; }

    public DbSet<CourseAssignment> CourseAssignments { get; set; }

    public DbSet<Course> Courses { get; set; }

    public DbSet<Exercise> Exercises { get; set; }

    public DbSet<Grade> Grades { get; set; }

    public DbSet<EventLog> EventLog { get; set; }

    public DbSet<ExerciseAssignment> ExerciseAssignments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(u => u.userID);

        modelBuilder.Entity<Role>().HasKey(r => r.RoleID);

        modelBuilder.Entity<RolePermission>().HasKey(rp => rp.RolePermissionID);

        modelBuilder.Entity<CourseAssignment>().HasKey(ca => ca.AssignmentID);
        modelBuilder.Entity<ExerciseAssignment>().HasKey(ea => ea.ExerciseAssignmentID);

        modelBuilder.Entity<Course>().HasKey(c => c.CourseID);

        modelBuilder.Entity<Exercise>().HasKey(e => e.ExerciseID);

        modelBuilder.Entity<Grade>().HasKey(g => g.GradeID);

        modelBuilder.Entity<EventLog>().HasKey(c => new { c.log_type, c.log_datetime });
    }
}
