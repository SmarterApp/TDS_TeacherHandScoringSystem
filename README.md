# Teacher Hand Scoring System (THSS) #
The Teacher Hand Scoring System was designed to be used by Teachers and Proctors of interim tests. It allows for the scoring of student responses and the reassigning of student responses to other proctors or teachers. Features of the Teacher Hand Scoring System include: 

* Interface to receive student responses from the Test Integration System. 
* Sorting and filtering of responses in a paged list of responses.
* Ability to reassign the items in the user's queue to other scorers in the same entity.
* Role-granted ability to view items belonging to other users in the user's entity.
* Role-granted ability to reassign other user's items.
* Interface for viewing student responses using the Item Rendering Service, a service that utilizes the same item rendering engine found in Open Source TDS. 
	* This includes the ability to view related items and associated passages.
* Linking to scoring guides and exemplar pdfs.
* An interface for scoring student responses
	* Score items in one or more scoring dimensions
	* Assign a condition code to a response
	* A max-min value for each dimension
* An interface for sending scored items back to the Test Integration system.
* A JSON interface for configuring items.

## License ##
This project is licensed under the [AIR Open Source License v1.0](http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf).

## Getting Involved ##
We would be happy to receive feedback on its capabilities, problems, or future enhancements:

* For general questions or discussions, please use the [Forum](http://forum.opentestsystem.org/viewforum.php?f=21).
* Use the **Issues** link to file bugs or enhancement requests.
* Feel free to **Fork** this project and develop your changes!

## Usage 
### Item Configuration API(/api/item/submit)
This is a REST endpoint that receives item configuraiton in json format. This is a tool used by system administrator to define hand scoring items in the system.  


### Test Report Receiver API(/api/test/submit)
This is a REST endpoint that receives test reports in an XML format from the Test Integration System.  
Each result received is inserted into a database where it is picked up and processed.  

### THSS User Interface(TSS.MVC)      
The Teacher Hand Scoring System was designed to be used by Teachers and Proctors of interim tests. It allows for the scoring of student responses and the reassigning of student responses to other proctors or teachers.  


## Build & Deploy
TIS requires Visual Studio 2012 to build. The Deployment steps are as follows - 

1) Create the following databases [DB Server]:

*  `TSS`

Create an App User account and grant that account dbo access on the above DBs. [DB server]

2) Create these folders on the application server (if they don't already exist): [Web server]

* `/thss_opentestsystem`

4) Deploy the `[Db server].TSS` database objects by running the following scripts in order: [DB server]

* `<root>\Src\DB\TSS\Tables\dbo.ConditionCodes.sql`
* `<root>\Src\DB\TSS\Tables\dbo.Dimensions.sql`
* `<root>\Src\DB\TSS\Tables\dbo.Items.sql`
* `<root>\Src\DB\TSS\Tables\dbo.Districts.sql`
* `<root>\Src\DB\TSS\Tables\dbo.Schools.sql`
* `<root>\Src\DB\TSS\Tables\dbo.Students.sql`
* `<root>\Src\DB\TSS\Tables\dbo.Teachers.sql`
* `<root>\Src\DB\TSS\Tables\dbo.Tests.sql`
* `<root>\Src\DB\TSS\Tables\dbo.Responses.sql`
* `<root>\Src\DB\TSS\Tables\dbo.Assignments.sql`
* `<root>\Src\DB\TSS\Tables\dbo.Logs.sql`
* `<root>\Src\DB\TSS\Tables\dbo._dbLatency.sql`
* `<root>\Src\DB\TSS\Functions\dbo.fn_SplitDelimitedString.sql`
* `<root>\Src\DB\TSS\StoredProcedures\*.sql`


5) Deploy TSS.MVC code at `thss_opentestsystem` [Web server]
	
6) Load the item Package by using domain/api/item/submit/.

## Configuration
Per instance application configuration settings can be found in the following files:

* `<root>\Src\TSS.MVC\App_Data\settings.config`
	
			<?xml version="1.0"?>
			<appSettings>
				<add key="webpages:Version" value="2.0.0.0" />
				<add key="webpages:Enabled" value="false" />
				<add key="PreserveLoginUrl" value="true" />
				<add key="ClientValidationEnabled" value="false" />
				<add key="UnobtrusiveJavaScriptEnabled" value="false" />
			
				<add key="EMAIL_AS_UUID" value="true"/>
				<add key="ART_API_URL" value="https://[SOME ART SERVER URL]/rest/user" />
				<add key="ART_API_CLIENT" value="SBAC"/>
				<add key="ART_OAUTH_URL" value="https://[SOME OPENAM URL]/auth/oauth2/access_token?realm=/[realm name where needed]"/>
				<add key="ART_OAUTH_PASSWORD_GRANTTYPE" value="true" />
				<add key="ART_OAUTH_USERNAME" value="[Valid OpenAM Username]"/>
				<add key="ART_OAUTH_REQUIRED" value="true"/>
				<add key="ART_OAUTH_PASSWORD" value="[Password for Valid User]"/>
				<add key="ART_OAUTH_SECRET" value="[Client Secret]"/>
				<add key="ART_OAUTH_CLIENTID" value="[Client ID as Configured in OpenAM]"/>
		   	
				<add key="IGNORE_TENANCY_CHAINS" value="False"/>
				<add key="SAML_OWNER_PREFIX" value="sbac"/>
				<add key="SAML_SESSIONREFRESH_URL" value="https:/[SOME OPENAM URL]/auth/identity/attributes?refresh=true"/>
				<add key="SAML_REDIRECT" value="https://[The URL of the THSS Service]/"/>
				<add key="PERMISSIONS_SCHEMA_URL" value="https://[SOME PERMISSIONS SERVER URL]/permissions/rest/role?component=[Name of Component as Registered in Permissions Server]"/>
			
				<!-- It is possible to load a pre-configured set of permissions from below instead of using the permissions API. Set LOAD_PERMISSIONS_FROM_LOCAL to true and load a role.json file in the App_Config folder. See Permissions API for JSON structure. -->
				<add key="LOAD_PERMISSIONS_FROM_LOCAL" value="false"/>
				<add key="ART_SCORER_DATA_CACHING_MINS" value="30"/>
				<add key="IRIS_OPEN_SOURCE" value="True"/>
    			<add key="IRIS_VENDOR_ID" value="2B3C34BF-064C-462A-93EA-41E9E3EB8333" />
				<add key="IRIS_ROOT_URL" value="https://[Item Renderer URL]/IRiS/"/>
				<add key="IRISBlackbox_ROOT_URL" value="https://[Item Renderer URL]/IRiS/"/>
			
				<add key="IRIS_PEM_LOCATION" value="~/App_Data/air-iris.ppk" />
				<add key="IRIS_KEY_EXPIRE_MINUTES" value="30" />
			
				<add key="USER_GUIDE_LOCATION" value="/content/UserGuide/TSS_User_Guide.docx"/>
				<add key="SCORE_SUBMITTED_MESSAGE" value="The score has been saved." />
				<add key="ACCESS_DENIED" value="You are not Authorized User"/>
				<add key="SHOW_STATUS" value="true" />

				<!-- Minimum level for which logs should be created -->
				<add key="MinLogLevel" value="Info" />
				<add key="COOKIE_TIMEOUT_MINS" value="30"/>
			</appSettings>
				
* `<root>\Src\TSS.MVC\App_Data\DataDistribution.config`

		<?xml version="1.0"?>
		<TSSDataDistribution>
			<ConnectionStrings>
				<ConnectionString name="DefaultConnection" connectionString="Data Source=[Standard SQL Server Connection String]" default="true">
    				<Districts>
        				<add id="1001"/>
        				<add id="1002"/>
        				<!-- This section is redundant and added for example only as all districts not explicitly mentioned in another connection string are added to the default configuration. -->
        			</Districts>
    			</ConnectionString>
    			<ConnectionString  name="DistrictDatabaseConnection1" connectionString="Data Source=[Standard SQL Server Connection String]" default="true">
    				<Districts>
    					<add id="1003"/>
    					<add id="1004"/>
    					<add id="9999"/>
    				</Districts>
    			</ConnectionString>
    			<!-- Additional connection strings may follow, here. -->
    		</ConnectionStrings>
    	</TSSDataDistribution>
		
* `<root>\Src\TSS.MVC\App_Data\sp.xml`
* `<root>\Src\TSS.MVC\App_Data\sp-extended.xml`
* `<root>\Src\TSS.MVC\App_Data\idp.xml`
* `<root>\Src\TSS.MVC\App_Data\idp-extended.xml`
* `<root>\Src\TSS.MVC\App_Data\fedlet.cot`
* `<root>\Src\TSS.MVC\App_Data\reportxml_os.xsd`

#### Additional Config information
	
#### Item Config JSON
##### A JSON array of items.
	[
	  {
	    "HandScored": "1", //This flag is set to true for items that should be seen in the item list and scored by a proctor
	    "Passage": null, // If an item should be grouped with other items, when viewing - the joining group should be listed here (e.g. 9999)
	    "bankKey": "[item bank number e.g. 9999]", // This is the item bank from which an item came
	    "baseUrl": "/Sample/Pdf/", // This is the URL on which all PDFs can be found. Relative or absolute.
	    "description": "My Item Description", 
	    "dimensions": [ // Scoring dimension array of an item
	      {
	        "conditions": [ //Condition code array that is assigned to a dimension
	          {
	            "code": "B", // Value to be sent to TIS regarding the code
	            "description": "Blank" // Description of the code
	          }, 
	          {
	            "code": "I", 
	            "description": "Insufficient"
	          }, 
	          {
	            "code": "L", 
	            "description": "Non-Scorable Language"
	          }
	        ], 
	        "description": "Correct Answer", 
	        "maxPoints": "1", // Maximum Points Possible
	        "minPoints": "0" // Maximum Points Possible
	      }
	    ], 
	    "exemplar": "[name of exemplar document].pdf", // This document should be found on baseURL 
	    "grade": "4", // Grade of item to be scored
	    "itemId": "9999", // Item ID - paired with bank id to render items.
	    "layout": "[some value]", // Layout style of an item
	    "maxScore": "1", // Maximum Item Level Score (should equal sum of dimensions)
	    "rubriclist": "<rubriclist><rubric scorepoint=\"\"><name>\r\n        Rubric\u00a0</name><val><![CDATA[ <!--[SOME HTML VALUE] --> ]]></val></rubric><samplelist maxval=\"\" minval=\"\" /></rubriclist>", //Hand Scored Rubric information  
	    "scoreType": "R", 
	    "subject": "MATH", // Subject of Item 
	    "trainingGuide": "[name of exemplar document].pdf", // This document should be found on baseURL 
	  }, 
	  {
	    "HandScored": "0", // An item can be part of a scoring group and not scored. Set this to 0 if it not supposed to be scored by the user and it will not be displayed in the the item list.
	    "Passage": 9999, // If an item should be grouped with other items, when viewing - the joining group should be listed here (e.g. 9999)
	    "bankKey": "9999", 
	    "baseUrl": "/Sample/Pdf/", 
	    "description": "My Item Description 2", 
	    "dimensions": [
	      {
	        "conditions": [
	          {
	            "code": "B", 
	            "description": "Blank"
	          }, 
	          {
	            "code": "I", 
	            "description": "Insufficient"
	          }, 
	          {
	            "code": "L", 
	            "description": "Non-Scorable Language"
	          }
	        ], 
	        "description": "Correct Answer", 
	        "maxPoints": "2", 
	        "minPoints": "0"
	      }
	      //Additional Items follow
	 ]


## Dependencies
Test Hand Scoring System has the following dependencies that are necessary for it to compile and run. 
please find dependencies in `<root>\package`

###Compile/Runtime Dependencies
* .Net Framework 4.5
* Castle Windsor
* Newtonsoft JSON.net
* Sharp Architecture
* .NET MVC 4
* Json Web Token
* CommonServiceLocator.WindsorAdapter
* Iesi.Collections

###Javascript/UI Dependencies
* jQuery
* jQuery UI
* Bootstrap
* Chosen
* Handlebars
* jQuery Colorbox
* jQuery Cookie
* jQuery DataTables
* jQuery StorageAPI
* jQuery Validate
* Modernizr


## Future Enhancements 
The following features and tasks are not included in the 2/6/2015 release:

###1) System and Integration Testing - 
The Teacher Hand Scoring System has not undergone a complete system testing or integration testing with the Test Integration System, System and integration testing will be complete (with the features identified above) as of the 02/23/2015 release.