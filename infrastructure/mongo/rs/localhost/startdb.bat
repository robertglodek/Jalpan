@echo off

mkdir C:\mongo-replica\node1
mkdir C:\mongo-replica\node2
mkdir C:\mongo-replica\node3

echo Starting MongoDb instance on port 27018...
start "" "C:\Program Files\MongoDB\Server\8.0\bin\mongod.exe" --port 27018 --dbpath C:\mongo-replica\node1 --replSet "dbrs" --bind_ip localhost

echo Starting MongoDb instance on port 27019...
start "" "C:\Program Files\MongoDB\Server\8.0\bin\mongod.exe" --port 27019 --dbpath C:\mongo-replica\node2 --replSet "dbrs" --bind_ip localhost

echo Starting MongoDb instance on port 27020...
start "" "C:\Program Files\MongoDB\Server\8.0\bin\mongod.exe" --port 27020 --dbpath C:\mongo-replica\node3 --replSet "dbrs" --bind_ip localhost

timeout /t 5 /nobreak

echo Initiating the replica set...
start "" "C:\Program Files\MongoDB\mongosh\bin\mongosh.exe" --port 27018 ".\scripts\rs-init.js"

pause