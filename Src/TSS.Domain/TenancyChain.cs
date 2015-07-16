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

namespace TSS.Domain
{
    public class TenancyChain
    {
        public string RoleID;
        public string Name;
        public string Level;
        public string ClientID;
        public string Client;
        public string GroupOfStatesID;
        public string GroupOfStates;
        public string StateID;
        public string State;
        public string GroupOfDistrictsID;
        public string GroupOfDistricts;
        public string DistrictID;
        public string District;
        public string GroupOfInstitutionsID;
        public string GroupOfInstitutions;
        public string InstitutionID;
        public string Institution;
        public EntityType Entity
        {
            get
            {
                try
                {
                    return (EntityType)Enum.Parse(typeof(EntityType), Level);
                }
                catch
                {
                    return EntityType.NA;
                }


            }
        }
        public string EntityId
        {
            get
            {
                switch (this.Entity)
                {
                    case EntityType.CLIENT:
                        return this.ClientID;
                    case EntityType.GROUPOFDISTRICTS:
                        return this.GroupOfDistrictsID;
                    case EntityType.DISTRICT:
                        return this.DistrictID;
                    case EntityType.GROUPOFSTATES:
                        return this.GroupOfStatesID;
                    case EntityType.GROUPOFINSTITUTIONS:
                        return this.GroupOfInstitutionsID;
                    case EntityType.INSTITUTION:
                        return this.InstitutionID;
                    default:
                        return "-1";

                }
            }
        }
        public string Role
        {
            get { return RoleName; }
        }

        public string RoleName
        {
            get { return Name; }
        }
        
        public enum EntityType
        {
            CLIENT = 0,
            GROUPOFSTATES = 1,
            STATE = 2,
            GROUPOFDISTRICTS = 3,
            DISTRICT = 4,
            GROUPOFINSTITUTIONS = 5,
            INSTITUTION = 6,
            NA = 7
        }

        public TenancyChain(string s)
        {
            //|6_107255|TE|INSTITUTION|1|||000000|Hawaii Department of Education|||9999|Training Complex Area A|99999|Demo School Group 1|9997|Training School B|
            //|DS1|TSS All Item Scorer|DISTRICT|1000|QA Client|||UT|UTAH|||DS1|District 1|||||
            s = s.Substring(1);

            int pipes = 0;
            foreach (char c in s)
            {
                if (c == '|')
                    pipes++;
            }

            // Make sure the tenancy chain is the proper length for this application.
            if (pipes != 17)
            {
                throw new Exception("invalid response format from OpenAM - missing entries in tenancy chain");
            }
            string[] sa = s.Split(char.Parse("|"));
            
            this.RoleID = sa[0];
            this.Name = sa[1];
            this.Level = sa[2];
            this.ClientID = sa[3];
            this.Client = sa[4];
            this.GroupOfStatesID = sa[5];
            this.GroupOfStates = sa[6];
            this.StateID = sa[7];
            this.State = sa[8];
            this.GroupOfDistrictsID = sa[9];
            this.GroupOfDistricts = sa[10];
            this.DistrictID = sa[11];
            this.District = sa[12];
            this.GroupOfInstitutionsID = sa[13];
            this.GroupOfInstitutions = sa[14];
            this.InstitutionID = sa[15];
            this.Institution = sa[16];

    //[0]: ""
    //[1]: "5_107255"
    //[2]: "TC"
    //[3]: "INSTITUTION"
    //[4]: "1"
    //[5]: ""
    //[6]: ""
    //[7]: "000000"
    //[8]: "Hawaii Department of Education"
    //[9]: ""
    //[10]: ""
    //[11]: "9999"
    //[12]: "Training Complex Area A"
    //[13]: "99999"
    //[14]: "Demo School Group 1"
    //[15]: "9997"
    //[16]: "Training School B"
    //[17]: ""
    
        }



    }
}
