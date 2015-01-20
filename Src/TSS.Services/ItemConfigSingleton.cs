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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TSS.Data;
using TSS.Domain.DataModel;

namespace TSS.Services
{
    public sealed class ItemConfigSingleton 
    {
        private static volatile ItemConfigSingleton instance;
        private static object syncRoot = new Object();
        private List<ItemType> ItemTypes { get; set; }
        private ItemConfigRepository repo = null;
        private ItemConfigSingleton() 
        {
            repo = new ItemConfigRepository();
            ItemTypes = repo.GetItemConfiguraitons();
        }
        public static ItemConfigSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ItemConfigSingleton();
                    }
                }
                return instance;
            }
        }

        public List<ItemType> LoadItemTypes()
        {
            return ItemTypes;
        }
        public void UpdateItemTypes()
        {
            repo.UpdateItemTypes(ItemTypes);
        }
    }
}
