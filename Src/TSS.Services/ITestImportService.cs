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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSS.Domain;
using TSS.Domain.DataModel;

namespace TSS.Services
{
    public interface ITestImportService
    {
        Teacher PopulateTeacherFromTdsReport(TDSReport tdsReport);
        School PopulateSchoolFromTdsReport(TDSReport tdsReport);
        Student PopulateStudentFromTdsReport(TDSReport tdsReport);
        Test PopulateTestFromTdsReport(TDSReport tdsReport);
        District PopulateDistrictFromTdsReport(TDSReport tdsReport);
        List<StudentResponse> PopulateItemsFromTdsReport(TDSReport tdsReport);
        void UpdateTeacherDistrictRelationship(string teacherId, string districtId);
    }
}
