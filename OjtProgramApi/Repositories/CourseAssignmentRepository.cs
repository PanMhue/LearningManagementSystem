using System.Diagnostics;
using Dapper;
using OjtProgramApi.Models;
using OjtProgramApi.Repositories;

//[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class CourseAssignmentRepository
    : RepositoryBase<CourseAssignment>,
        ICourseAssignmentRepository
{
    public CourseAssignmentRepository(AppDB context)
        : base(context) { }

    // private string GetDebuggerDisplay()
    // {
    //     return ToString();
    // }
}
