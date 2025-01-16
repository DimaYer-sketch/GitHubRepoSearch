GitHub Repository Search
A full-stack application showcasing GitHub repository search and bookmarking functionality. This project demonstrates the use of Angular for the frontend and .NET 8 for the backend.

Features
Search GitHub repositories by typing a keyword.
View results as gallery items with:
Repository name.
Owner's avatar.
Bookmark button.
Bookmark repositories and store them in the user's session (custom session implementation).
JWT-based authentication between client and server.
(Bonus) Bookmark screen to view and manage all bookmarked repositories.
Technologies Used
Frontend: Angular 15, TypeScript, HTML, SCSS, Bootstrap/Angular Material.
Backend: .NET 8 Web API, C#.
API: GitHub REST API.

How to Run
Prerequisites
Install Node.js (for Angular client).
Install .NET SDK 8 (for the server).

Steps
Clone the repository:
git clone https://github.com/DimaYer-sketch/GitHubRepoSearch.git

Server Setup:
Navigate to the Server/GitHubRepoSearchApi folder.
Open the solution (GitHubRepoSearchApi.sln) in Visual Studio.
Build and run the server.
The server will be available at: https://localhost:7008.

Client Setup:
Navigate to the Client/GitHubRepoSearch folder.
Open the folder in Vsual Code.
Run the following commands:
npm install
npm start

The Angular application will be available at: http://localhost:4200.
Improvements Beyond the Initial Task
Enhanced UI with a user-friendly design.
Bookmarked repositories are visually marked and cannot be added again.
Added functionality to remove bookmarks from the bookmark screen.
Search inputs are retained when navigating between tabs.
Implemented an AuthInterceptor to attach JWT tokens to HTTP requests.

About Me
I am a full-stack developer with extensive experience in modern web development technologies, including Angular, .NET, and RESTful APIs. 
This project demonstrates my ability to build efficient, scalable, and maintainable applications.
