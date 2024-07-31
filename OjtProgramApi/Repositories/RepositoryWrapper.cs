using OjtProgramApi.Models;

namespace OjtProgramApi.Repositories
{
    public class RepositoryWrapper(AppDB context) : IRepositoryWrapper
    {
        readonly AppDB _context = context;

        private IUserRepository? _user;

        private ICourseAssignmentRepository? _courseAssignment;

        private IRolePermissionRepository? _rolePermission;

        private ICourseRepository? _course;

        private IExerciseRepository? _exercise;

         private IExerciseAssignmentRepository? _exerciseAssignment;

        private IGradeRepository? _grade;

        private IEventLogRepository? _eventLog;

        public ICourseAssignmentRepository CourseAssignment
        {
            get
            {
                _courseAssignment ??= new CourseAssignmentRepository(_context);
                return _courseAssignment;
            }
        }

        public IUserRepository User
        {
            get
            {
                _user ??= new UserRepository(_context);
                return _user;
            }
        }

        public IRolePermissionRepository RolePermission
        {
            get
            {
                _rolePermission ??= new RolePermissionRepository(_context);
                return _rolePermission;
            }
        }

        public ICourseRepository Course
        {
            get
            {
                _course ??= new CourseRepository(_context);
                return _course;
            }
        }

        public IExerciseRepository Exercise
        {
            get
            {
                _exercise ??= new ExerciseRepository(_context);
                return _exercise;
            }
        }

        public IGradeRepository Grade
        {
            get
            {
                _grade ??= new GradeRepository(_context);
                return _grade;
            }
        }

        public IEventLogRepository EventLog
        {
            get
            {
                _eventLog ??= new EventLogRepository(_context);
                return _eventLog;
            }
        }
         public IExerciseAssignmentRepository ExerciseAssignment
        {
            get
            {
                _exerciseAssignment ??= new ExerciseAssignmentRepository(_context);
                return _exerciseAssignment;
            }
        }
    }
}
