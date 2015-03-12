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
using System.Data;

namespace TSS.Data.Sql
{
    public interface IColumnReader : IDataReader
    {
        bool GetBoolean(string column);
        byte GetByte(string column);
        long GetBytes(string column, long fieldOffset, byte[] buffer, int bufferOffset, int length);
        char GetChar(string column);
        DateTime GetDateTime(string column);
        decimal GetDecimal(string column);
        double GetDouble(string column);
        float GetFloat(string column);
        Guid GetGuid(string column);
        short GetInt16(string column);
        int GetInt32(string column);
        long GetInt64(string column);
        string GetString(string column);
        object GetValue(string column);
        bool IsDBNull(string column);
        Type GetFieldType(string column);

        bool HasColumn(string name);
        bool HasValue(string name);
        List<string> Columns { get; }

        bool FixNulls { get; set; }
        bool FixMissing { get; set; }
    }

}

