using System;
using System.Reflection;
using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using TSS.Services;
using NHibernate.Tool.hbm2ddl;
using SharpArch.NHibernate;
using Microsoft.Practices.ServiceLocation;
using Configuration = NHibernate.Cfg.Configuration;
 
namespace TSS.Build
{
    class Program
    {
        #region Fields

        private static Configuration _configuration;
        //private static ILogRepository _logRepository;
        private static IWindsorContainer _container;

        #endregion

        #region Main Routine

        static void Main()
        {
            InitNHibernate();
            InitServiceLocator();

            Console.WriteLine("Connections initialized. What would you like to do?");
            Console.WriteLine("[D]emo Environment Setup");
            Console.WriteLine("[Q]uit");
            Console.WriteLine("Press a key...");
            bool validKey = false;

            while (!validKey)
            {
                string key = Console.ReadKey().Key.ToString().ToLower();
                switch(key)
                {
                    case "d":
                        validKey = true;
                        SetupDemoSite();
                        break;
                    case "q":
                        validKey = true;
                        break;
                    default:
                        Console.Write("Invalid choice. Press a key...");
                        break;
                }
            }

            RunScript("sp_DecryptValue.sql");

            NHibernateSession.CloseAllSessions();
            NHibernateSession.Reset();
        }

        #endregion

        #region Setup Methods

        private static void SetupDemoSite()
        {
            CreateDb();
        }

        #endregion

        #region Helper Methods

        private static void InitNHibernate()
        {
            //_configuration = NHibernateSession.Init(
            //    new SimpleSessionStorage(),
            //    new[] { "TSS.Data.dll" },
            //    new AutoPersistenceModelGenerator().Generate(),
            //    "NHibernate.config");
        }

        private static void InitServiceLocator()
        {
            // Setup Windsor for SharpArchitecture
            //_container = new WindsorContainer();
            //ComponentRegistrar.Initialize(_container);
            //ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(_container));
            //InitServices();
        }

        private static void InitServices()
        {
        }

        private static void CreateDb()
        {
            new SchemaExport(_configuration).Execute(false, true, false);
            //BUILDS VIEW
            RunScript("Create_View_AssignmentsList.sql");
            //CREATE CLUSTERED INDEX ON VIEW
            RunScript("Create_ClusteredIndex_View_AssignmentList.sql");
            //CREATE USER DEFINED FUNCTIONS
            RunScript("UserDefinedFunction4New_sp_get_itemlistFilters.sql");
            //CREATES INDEX
            RunScript("Create_Index_Teacher.UUID.sql");
            //CREATES PAGINATED MAIN LIST SP
            RunScript("CreateGetItemList3.sql");
            //GETS ITEMS FOR FILTER DROPDOWN BOXES
            RunScript("CreateGetFilterItems3.sql");
            //GETS ROW COUNT
            RunScript("CreateGetItemCount.sql");
            //GETS ASSIGNED ITEMS
            RunScript("CreateGetAssignedItems.sql");
            

        }

        private static void RunScript(string filename)
        {
            // Get path to item in Build/Scripts folder. 
            // Relative to executable path of compiled executable.
            var a = Assembly.GetEntryAssembly();
            var baseDir = System.IO.Path.GetDirectoryName(a.Location);
            var fullPath = System.IO.Path.Combine("../../../../Build/Scripts/", filename);
            var script = System.IO.File.ReadAllText(fullPath);
            var stringSeparators = new string[] { "\nGO" };
            var lines = script.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                foreach (var query in lines)
                {
                    var command = NHibernateSession.Current.Connection.CreateCommand();
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion
    }
}
