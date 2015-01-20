#region License
// /*******************************************************************************                                                                                                                                    
//  * Educational Online Test Delivery System                                                                                                                                                                       
//  * Copyright (c) 2014 American Institutes for Research                                                                                                                                                              
//  *                                                                                                                                                                                                                  
//  * Distributed under the AIR Open Source License, Version 1.0                                                                                                                                                       
//  * See accompanying file AIR-License-1_0.txt or at                                                                                                                                                                  
//  * http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf                                                                                                                                                 
//  ******************************************************************************/ 
#endregion
using TSS.Domain.DataModel;

namespace TSS.Data
{
    public interface ITestImportRepository
    {
        bool SaveDistrictAndSchool(string xmlInputs);
        bool SaveStudent(Student aStudent);
        bool SaveTeacher(Teacher aTeacher);
        bool SaveTest(Test aTest);
        bool BatchProcessAssingmentAndResponse(string xmlInputs);


    }
}
