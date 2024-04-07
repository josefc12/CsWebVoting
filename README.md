# cs_web_voting
A web app for voting over cs maps
Check license.

version 1.0.0:
The code base needs revision. It's in a very crap state :/.

Depends on:
nodejs (sudo apt install nodejs)
npm (sudo apt install npm)
React (already taken care of by project dependencies)
MySQL
SignalR (aleary taken care of by project dependencies)
dotnet 7.0.0 (download the script at https://learn.microsoft.com/en-us/dotnet/core/install/linux-scripted-manual#scripted-install and then do ./dotnet-install.sh --channel 7.0)
dotnet-ef tool (dotnet tool install --global dotnet-ef)

How to setup:
Setup the database (needs MySQL):
    Create the database:
    sudo mysql
    CREATE DATABASE cs_web_voting;
    Look into 'appsettings.json' and customize the line at:
    "DefaultConnection":"Server=localhost;Database=cs_web_voting;Uid=testuser;Pwd=12345;"
    If you installed MySQL exclusively for this project and have no mysql setup, you can create the testuser and the password by:
        sudo mysql
        CREATE USER 'testuser'@'localhost' IDENTIFIED BY '12345';
        GRANT ALL PRIVILEGES ON cs_web_voting.* TO 'testuser'@'localhost';
    Do a migration (install dotnet-ef tools 'dotnet tool install --global dotnet-ef'):
        dotnet ef migrations add InitialCreate
        dotnet ef database update
    After that's done, populate the maps table with map names. It's best to use the file maps.csv included in the Files directory. The app expects the maps to use the schema. DESCRIBE maps to check the generated maps table schema in mysql;

Do 'dotnet build'
Do 'dotnet run'
Go to https://localhost:7292/
Enjoy.

How to:
-> Input nickname, input room name
    Others can join your room by inputting the same room name. If you input a room name of a non-existing room, a new room with such name will be created and will remain created until the session ends or when the last person leaves.
-> During the first stage, wait for an admin to start the nominating process.
-> During the nominating stage, each person gets to nominate 3 maps. 
-> During the voting stage, each person gets to vote on the nominated maps.
-> At last, the map with the most votes is displayed.

If you're running a dedicated server, you can run a script that would change the map or start a new server with the map.

DISCLAIMER: I'm actually not sure how many concurrent session this app can handle.

Customization:
You can find the frontend in the ClientApp dir
The style is original and can be found at ClientApp/src/tac-custom.css
