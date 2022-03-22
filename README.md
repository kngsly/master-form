# Master Form 
A light weight and portable form creator where you can create experiments (forms) and configure input fields.



Written in ASP.NET Core 6.0 using the latest Blazor Server + docker container config.

### Get it running
- Clone this repo
- Install Visual Studio 2022 (.NET 6.0)
- Open master-form.sln or master-form-blazor-server.csproj
- Make sure master-form-blazor-server is set to the active project "Right click -> Set as startup project"
- Run it straight into a web browser or deploy a docket container.

![image](https://user-images.githubusercontent.com/33945956/159489914-3d6a011c-d25b-4c3e-94d0-7ecff073c5c9.png)

### Assumptions
- Admin auth is outside of this scope, I have implemented "IsAnAdmin" placeholder variables instead.
- Security to prevent spamming and denial of service is out of scope.
- Using local json for storage instead of a database solution.


### Preview
![image](https://user-images.githubusercontent.com/33945956/159488466-24e93ee9-b1ff-450f-a986-d8bfdb95d050.png)
