using System.Xml.Linq;
using TSS.Domain.DataModel;

namespace TSS.Data
{
    public interface ITestImportRepository
    {
        bool SaveDistrictAndSchool(string xmlInputs,string district);
        bool SaveStudent(Student aStudent, string district);
        bool SaveTeacher(Teacher aTeacher, string district);
        bool SaveTest(Test aTest, string district);
        bool BatchProcessAssingmentAndResponse(string xmlInputs, string district);


    }
}
