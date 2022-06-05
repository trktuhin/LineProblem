# LineProblem
This is a solution to a problem.
Problem: Build and API service using .NET 5 where a user can send a list of words through a post
request and a page size n using request header. The header name should be page-size. Now
you have to construct line(s) using those words where each line can contain a maximum n
number of characters. Return the constructed line through a get request.

# Steps to Run the Project
Requirement: .NET SDK 6
1. Go to the project directory from your terminal or command prompt
2. Run command "dotnet watch run" or "dotnet run" (this should open your browser with swagger index; most probably http://localhost:5161/swagger/index.html this url)
3. Send POST request using Postman or some other tool containing "page-size" header with numeric value and list of words in the body
4. You should get a string value back which is record id (generated in the server)
5. You can get the constructed line(s) by the record id sending a GET request

# Sample POST Request example
URL: http://localhost:5161/api/line
TYPE: POST
HEADERS: Key = "page-size" value = 20
BODDY: {
  "words": [
    "string", "Robin", "Khan", "Tuhin", "Faisal"
  ]
}

# Sample GET Request example
URL: http://localhost:5161/api/Line/649ee3ba-d9cb-46a8-b22d-18470928115f
TYPE: GET
