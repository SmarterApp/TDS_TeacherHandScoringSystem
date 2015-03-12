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
using System.Linq;

namespace TSS.Domain
{
    /// <summary>
    /// Paged list of data to include page number, 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T> : List<T>, IPagedList<T>
    {
        private IQueryable<T> sourceQuery;

        public PagedList(List<T> source, int index, int pageSize, int totalRecords)
        {
            sourceQuery = source.AsQueryable();
            this.TotalCount = totalRecords;
            this.PageSize = pageSize;
            this.PageIndex = index;
            this.AddRange(source.ToList());
        }

        public PagedList(IQueryable<T> source, int index, int pageSize)
        {
            sourceQuery = source;
            this.TotalCount = source.Count();
            this.PageSize = pageSize;
            this.PageIndex = index;
            this.AddRange(source.Skip((index * pageSize) - pageSize).Take(pageSize).ToList());
        }

        public PagedList(List<T> source, int index, int pageSize)
        {
            sourceQuery = source.AsQueryable();
            this.TotalCount = source.Count();
            this.PageSize = pageSize;
            this.PageIndex = index;
            this.AddRange(source.Skip((index * pageSize) - pageSize).Take(pageSize).ToList());
        }

        public List<T> AllItems
        {
            get
            {
                return sourceQuery.ToList();
            }
        }

        public List<T> Items
        {
            get
            {
                return this.ToList();
            }
        }

        public int TotalCount
        {
            get;
            set;
        }

        public int PageIndex
        {
            get;
            set;
        }

        public int PageSize
        {
            get;
            set;
        }

        public int TotalPages
        {
            get
            {
                double val = (double)TotalCount / (double)PageSize;
                return (int)Math.Ceiling(val);
            }
        }

        public int CurrentPage
        {
            get
            {
                return PageIndex;
                /*double val = (double)PageIndex / (double)PageSize;
                return (int)Math.Ceiling(val);*/
            }
        }

        public bool IsPreviousPage
        {
            get
            {
                return (PageIndex > 0);
            }
        }

        public bool IsNextPage
        {
            get
            {
                return (PageIndex * PageSize) <= TotalCount;
            }
        }
    }

}
