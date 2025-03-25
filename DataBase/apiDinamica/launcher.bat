@echo off
cls
echo ==========================================
echo    Selecciona el modo de ejecucion:
echo ==========================================
echo 1 - Online (Base de datos remota)
echo 2 - Local (Base de datos local)
set /p MODE="Introduce tu eleccion (1/2): "

if "%MODE%"=="1" (
    set /p MONGODB_URI="Introduce la URL de la base de datos online: "
    setx MONGODB_URI "%MONGODB_URI%" /M
    echo Variable de entorno MONGODB_URI establecida.
    echo Iniciando la aplicacion online...
    java -jar tfg-0.0.1-SNAPSHOT.jar
    pause
    exit
)

if "%MODE%"=="2" (
    set /p MONGOHOST="Introduce el host de la base de datos local: "
    set /p MONGOPORT="Introduce el puerto de la base de datos local: "
    setx MONGOHOST "%MONGO_HOST%" /M
    setx MONGOPORT "%MONGO_PORT%" /M
    echo Variables de entorno MONGO_HOST y MONGO_PORT establecidas.
    echo Iniciando la aplicacion local...
    java -jar tfgLocal-0.0.1-SNAPSHOT.jar
    pause
    exit
)

echo Opción no valida. Por favor, ejecuta el script nuevamente.
pause
exit



::Se puede lanzar manualmente con una ruta dinamica de la siguiente manera:
:: set MONGODB_URI= ruta
:: java -jar mi-aplicacion.jar
