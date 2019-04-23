# microgaming
## Take home assignment for Microgaming
* .Net Framework 4.6 with MVC 5 code-first build
* SQL Server Express 2017 for the data store
* Bootstrap 4
* jQuery 3.3.1 (I needed to upgrade to take advantage of some functionality)
* ASP.Net Identity

You'll note that the registration process deliberately does not cater for admin so I've added a bit to the startup file - hard-coded me as admin user.  You can always change that if you need to.
The web config file contains info such as permissible file extensions.  Very important is the info I added to allow bigger attachments as .Net by default only allows 1024kb if memory serves.

* Users can register and log in.
* The default page is the request list but the user cannot access it without logging in
* Once logged in there is a toggle of top nav items.  The request list view of the admin differs to that of the regular user.  
* Regular users only have view links in their list whilst the admins have View | Edit | Delete.  The reason for this is that admins can perform CRUD for everything whilst the regular user is restricted.  They see edit/delete buttons on the actual request details page and only when the status is not "Approved"
* There is no separate Admin capability in terms of managing requests - whilst regular users, when creating/editing a request always have the status set to 1 (Submitted), the admin sees a status drop-down to change the request status.
