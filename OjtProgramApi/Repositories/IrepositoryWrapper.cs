namespace OjtProgramApi.Repositories
{
    public interface IRepositoryWrapper
    {
        IUserRepository User { get; }
        ICourseAssignmentRepository CourseAssignment { get; }
        IRolePermissionRepository RolePermission { get; }
        ICourseRepository Course { get; }

        IExerciseRepository Exercise { get; }

        IExerciseAssignmentRepository ExerciseAssignment { get; }

        IGradeRepository Grade { get; }

        IEventLogRepository EventLog { get; }

        //     Task SaveAsync();
        // }
    }
}
