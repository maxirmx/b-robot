# b-robot
B Robot


## (Installation notes)

...
sudo apt-get install -y gpg
wget -O - https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor -o microsoft.asc.gpg
sudo mv microsoft.asc.gpg /etc/apt/trusted.gpg.d/
wget https://packages.microsoft.com/config/ubuntu/22.04/prod.list
sudo mv prod.list /etc/apt/sources.list.d/microsoft-prod.list
sudo chown root:root /etc/apt/trusted.gpg.d/microsoft.asc.gpg
sudo chown root:root /etc/apt/sources.list.d/microsoft-prod.list
...


sudo apt-get update &&  sudo apt-get install -y dotnet-sdk-7.0

or aspnetcore-runtime-7.0 or dotnet-runtime-7.0  ---- (not tested what is enough to run)

dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL (?)

docker compose build
docker compose up b_robot_app
