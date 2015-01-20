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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TSS.Domain.DataModel
{
    public class StudentResponseAssignment 
    {
        public Guid AssignmentId { get; set; }
        public Test Test { get; set; }
        public Student Student { get; set; }
        public School School { get; set; }
        public Teacher Teacher { get; set; }
        public string ScoreData { get; set; }
        public ScoreStatusCode ScoreStatus { get; set; }
        public string SessionId { get; set; }
        public long OpportunityId { get; set; }
        public Guid OpportunityKey { get; set; }
        public StudentResponse StudentResponse { get; set; }
        public string CallbackUrl { get; set; }
        public string ClientName { get; set; }
        public enum ScoreStatusCode
        {
            NotScored = 0,
            TentativeScore = 1,
            Scored = 2
        }
        public StudentResponseAssignment()
        {
            StudentResponse = new StudentResponse();
            Teacher = new Teacher();
            Student = new Student();
            Test = new Test();
        }
    }

    public class StudentResponse 
    {
        public long ResponseId { get; set; }
        public string SegmentId { get; set; }
        public int ItemKey { get; set; }
        public int BankKey { get; set; }
        public string Format { get; set; }
        public string ContentLevel { get; set; }
        public string Response { get; set; }
        public DateTime? ResponseDate { get; set; }
        //public ItemType ItemType { get; set; }
        public int ScoreStatus { get; set; }

        public StudentResponse()
        {
//            ItemType = new ItemType();
        }
    }
    
    public class Test
    {
        public string TestId { get; set; } 
        public string Name { get; set; }
        public string Subject { get; set; }
        public int Bank { get; set; }
        public string Contract { get; set; }
        public string Mode { get; set; }
        public string Grade { get; set; }
        public string AssessmentType { get; set; }
        public int AcademicYear { get; set; }
        public string Version { get; set; }
        
    }


    public class Student 
    {
        public string SSID { get; set; }
        public string TdsLoginId { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Dob { get; set; }
        public string Grade { get; set; }
        public long StudentId { get; set; }
    }

    public class Teacher
    {
        public string Name { get; set; }
        public string TeacherID { get; set; }
    }

    public class School
    {
        public string SchoolID { get; set; }
        public string DistrictID { get; set; }
        public string SchoolName { get; set; }
        public string StateName { get; set; }
    }

    public class District
    {
        public string DistrictID { get; set; }
        public string DistrictName { get; set; }
    }

    public class Log 
    {
        public DateTime LogDate { get; set; }
        public  LogCategory Category { get; set; }
        public  LogLevel Level { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public string IpAddress { get; set; }

        public Log()
        {
            LogDate = DateTime.UtcNow;
        }
    }
    public enum LogCategory
    {
        /// <summary>
        /// Hardware or IIS level logs.
        /// </summary>
        [Description("System")]
        System = 1,

        /// <summary>
        /// Application logs (most common).
        /// </summary>
        [Description("Application")]
        Application,

        /// <summary>
        /// Security adits
        /// </summary>
        [Description("Security")]
        Security,

        /// <summary>
        /// Emails being sent
        /// </summary>
        [Description("Email")]
        Email
    }
    public enum LogLevel
    {
        [Description("Debug")]
        Debug = 1,
        [Description("Info")]
        Info,
        [Description("Warning")]
        Warning,
        [Description("Error")]
        Error
    }

    public class ItemType
    {
        public int ItemKey { get; set; }
        public int BankKey { get; set; }
        public int Passage { get; set; }
        public bool HandScored { get; set; }
        public string Subject { get; set; }
        public string Grade { get; set; }
        public string Description { get; set; }
        public string ExemplarURL { get; set; }
        public string TrainingGuideURL { get; set; }

        public string RubricListXML { get; set; }

        public List<Dimension> Dimensions { get; set; }
        public string Layout { get; set; }

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public bool Modified {get;set;}

        public ItemType()
        {
            Modified = false;
            Dimensions = new List<Dimension>();
        }
    }

    public class ConditionCode 
    {
        public int ConditionCodeId { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public int DimensionId { get; set; }
        public ConditionCode()
        {
            
        }
    }

    // This is a hack to get around the poor way the data is modelled.
    // We store condition codes to reference dimension, but in reality
    // dimensions reference a condition code.  So when we are import new ones,
    // store the dimension information so that we can update the condition table
    // correctly.
    public class ConditionCodeSql
    {
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public string DimensionName { get; set; }
        public int ItemKey { get; set; }
        public int BankKey { get; set; }
        public ConditionCodeSql()
        {

        }
        public ConditionCodeSql(ConditionCode code, Dimension dim, ItemType item)
        {
            BankKey = item.BankKey;
            ItemKey = item.ItemKey;
            DimensionName = dim.Name;
            FullName = code.FullName;
            ShortName = code.ShortName;
        }
    }

    public class Dimension 
    {
        public int DimensionId { get; set; }
        public string Name { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public List<ConditionCode> ConditionCodes { get; set; }
        public int ItemKey { get; set; }
        public int BankId { get; set; }
        public Dimension()
        {
            ConditionCodes = new List<ConditionCode>();
        }
    }
    public class ItemGroupEntry
    {
        public virtual int BankKey { get; set; }
        public virtual int ItemKey { get; set; }
        public virtual string Response{ get; set; }
    }
}
