# PowerPlatformSolutionImport

This repo contains source code of a sample project which I created as a part of my blog series 
There are two projects: 
1. PowerPlatformSolutionImport is an ASP.NET MVC project. After signing in, the user can import a Power Platform solution into the target environment. The solution will be imported from the logged in user's context. 
Enter clientID and clientSecret in Web.Config, and replace destEnvUrl in ImportController.cs with your environment's url. For more details, check out this blog which I have written for the same: https://rohannevrikar.wordpress.com/2020/08/09/importing-power-platform-solution-using-delegated-permission/


2. PowerPlatformSolutionImport-ConsoleApp is a C# console project. Make sure clientId, secret and url are configured before running the program. This console app will import solution outside user's context i.e no signed in user is required in this case. For more details, check out this blog which I have written for the same: https://rohannevrikar.wordpress.com/2020/08/10/importing-power-platform-solution-using-application-user/  
