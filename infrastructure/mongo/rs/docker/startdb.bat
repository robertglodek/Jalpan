@echo off

REM Start the containers in detached mode
docker-compose up -d

REM Wait for 5 seconds to give containers time to initialize
timeout /t 5 /nobreak

REM Run the rs-init.sh script inside mongo1 container
docker exec mongo1 /scripts/rs-init.sh