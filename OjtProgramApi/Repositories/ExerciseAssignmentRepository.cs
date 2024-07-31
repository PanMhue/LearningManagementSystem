using OjtProgramApi.Models;

namespace OjtProgramApi.Repositories;

public class ExerciseAssignmentRepository: RepositoryBase<ExerciseAssignment>,
    IExerciseAssignmentRepository
{
    public ExerciseAssignmentRepository(AppDB context)
         : base(context) { }
}