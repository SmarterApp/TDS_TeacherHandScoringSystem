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
using System.Configuration;
using Castle.Facilities.Startable;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using SharpArch.Web.Mvc.Castle;
using TSS.Data;
using TSS.Domain.DataModel;


namespace TSS.Services
{
    public class ComponentRegistrar
    {
        /// <summary>
        /// Initialize default container
        /// </summary>
        /// <param name="container"></param>
        public static void Initialize(IWindsorContainer container)
        {
            //AddGenericRepositoriesTo(container);
            AddCustomRepositoriesTo(container);
            AddServicesTo(container);
        }

        /// <summary>
        /// Add an implementation to the registrar
        /// </summary>
        /// <typeparam name="I">Interface</typeparam>
        /// <typeparam name="T">Implementation of interface to use</typeparam>
        /// <param name="container">Registrar container</param>
        public static void Add<I, T>(IWindsorContainer container)
        {
            container.Register(
                    Component.For(typeof(I)).ImplementedBy(typeof(T))
                );
        }

        /// <summary>
        /// Add default services to container
        /// </summary>
        /// <param name="container"></param>
        private static void AddServicesTo(IWindsorContainer container)
        {
            container.Register(
               Component.For<IConfigService>()
                .ImplementedBy<ConfigService>()
                .DependsOn(new
                {
                    appSettings = ConfigurationManager.AppSettings
                }));

            // no longer need the logger service.  Use repo directly
            /*  
             *             var minLogLevel =
                (LogLevel)Enum.Parse(typeof(LogLevel), ConfigurationManager.AppSettings["MinLogLevel"]);


             * container.Register(
               Component.For<ILoggerService>()
                .ImplementedBy<LoggerService>()
                .DependsOn(new
                {
                    minLevel = minLogLevel
                }));  */

            container.Register(
                Component.For<IEmailService>()
                    .ImplementedBy<EmailService>());

           
            container.Register(
                AllTypes
                    .FromAssemblyNamed("TSS.Services")
                    .Pick().If(t => t.Name.EndsWith("Service"))
                    .WithService
                    .FirstNonGenericCoreInterface("TSS.Services"));

           
          
        }
       
        /// <summary>
        /// Add application-specific repositories to container
        /// </summary>
        /// <param name="container"></param>
        private static void AddCustomRepositoriesTo(IWindsorContainer container)
        {
          
            container.Register(Component.For<IUserManagementApi>().ImplementedBy<UserManagementApi>());
            container.Register(Component.For<IExportService>().ImplementedBy<ExportService>());
            container.Register(Component.For<ITestImportRepository>().ImplementedBy<TestImportRepository>());
            container.Register(Component.For<IStudentResponseRepository>().ImplementedBy<StudentResponseRepository>());
            container.Register(Component.For<ILoggerRepository>().ImplementedBy<LoggerRepository>());
            container.AddFacility<StartableFacility>();
            container.Register(Component.For<IItemConfigRepository>().ImplementedBy<ItemConfigRepository>().LifeStyle.Singleton.Start());
        }
    }
}
