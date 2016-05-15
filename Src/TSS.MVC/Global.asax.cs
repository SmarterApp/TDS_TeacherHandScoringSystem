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
using System.Web;
using System.Web.Http;
using System;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using SharpArch.Domain.Events;
using Microsoft.Practices.ServiceLocation;
using SharpArch.Web.Mvc.Castle;
using SharpArch.Web.Mvc.ModelBinder;
using TSS.Data.DataDistribution;
using TSS.MVC.Controllers;
using TSS.Services;
using System.Configuration;
using System.Net;


namespace TSS.MVC
{

    /// <summary>
    /// Represents the MVC Application
    /// </summary>
    /// <remarks>
    /// For instructions on enabling IIS6 or IIS7 classic mode, 
    /// visit http://go.microsoft.com/?LinkId=9394801
    /// </remarks>
    public class MvcApplication : HttpApplication
    {
        
        public override void Init()
        {
            base.Init();
        
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Start()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            ModelBinders.Binders.DefaultBinder = new SharpModelBinder();
            ModelValidatorProviders.Providers.Add(new ClientDataTypeModelValidatorProvider());

            InitializeServiceLocator();

            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            RouteRegistrar.RegisterRoutesTo(RouteTable.Routes);

            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            

            SAML.Common.Config.Initialize(
            ConfigurationManager.AppSettings["SAML_SESSIONREFRESH_URL"],
            ConfigurationManager.AppSettings["SAML_REDIRECT"],
             ConfigurationManager.AppSettings["SAML_REDIRECT"],
            "6000",
            "iPlanetDirectoryPro-drcdev",
            "Username", this.Context.Server.MapPath("~/App_Data"), "openam_keepalive.aspx");

            ServicePointManager.ServerCertificateValidationCallback += (sender, c, chain, sslPolicyErrors) => true;

        }

        /// <summary>
        /// Instantiate the container and add all Controllers that derive from
        /// WindsorController to the container.  Also associate the Controller
        /// with the WindsorContainer ControllerFactory.
        /// </summary>
        protected virtual void InitializeServiceLocator()
        {
            var container = new WindsorContainer();

            // Initialize MVC application
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));
            container.RegisterControllers(typeof(HomeController).Assembly);
            ComponentRegistrar.Initialize(container);

            // Initialize WebApi
            container.Register(AllTypes.FromAssembly(typeof(HomeController).Assembly)
                                        .BasedOn<ApiController>()
                                        .If(r => r.Name.EndsWith("Controller"))
                                        .LifestyleTransient());

            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator),
                new WindsorCompositionRoot(container));

            GlobalConfiguration.Configuration.DependencyResolver = new WindsorDependencyResolver(container);

            // Setup service locator
            var windsorServiceLocator = new WindsorServiceLocator(container);
            DomainEvents.ServiceLocator = windsorServiceLocator;
            ServiceLocator.SetLocatorProvider(() => windsorServiceLocator);
        }

    }
}
