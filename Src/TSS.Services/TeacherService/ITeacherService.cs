using System;
using System.Collections.Generic;
using TSS.Domain;
using TSS.Domain.DataModel;

namespace TSS.Services
{
    public interface ITeacherService
    {
        IPagedList<Teacher> GetTeachers();
        ATeacher PopulateTeacherFromTdsReport(TDSReport tdsReport);
        bool SaveTeacher(ATeacher aTeacher);

		TeacherResult GetTeachersFromApi(int pageNumber, int pageSize, string role, string associatedEntityId, string level, string state);
        List<SearchResults> GetListOfPossibleScorers(List<TenancyChain> tens, List<RoleSet> roles);
        Teacher GetTeacherByUUID(Teacher teacher);
        List<Teacher> GetTearchersByUUIDs(IEnumerable<string> emailList);
       
    }
}
