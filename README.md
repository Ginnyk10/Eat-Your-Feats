# EatYourFeats

## [Click This To Access Our Game!](https://eatyourfeats1.azurewebsites.net/)

## Setting Up MongoDB Locally

1. Open the solution in Visual Studio. Then, open the terminal and navigate to the folder holding the solution (cd EatYourFeats)

2. In the terminal, install the MongoDB driver for .NET by running the following command: dotnet add package MongoDB.Driver

3. Whitelist Your Current IP Address
The connection string is currently hardcoded. However, to connect to the MongoDB database remotely, you still must whitelist your current IP address. Follow these steps to whitelist your IP:
    - Go to the MongoDB website.
    - Navigate to Ginny's Org - 2024-10-17.
    - Select Project 0.
    - From the sidebar, select Network Access.
    - If your IP is not whitelisted, you should be automatically prompted to add it

Once you’ve completed the above steps, you can run the application and connect to MongoDB with the provided connection string.
