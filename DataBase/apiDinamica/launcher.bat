@echo off
set /p MONGODB_URI="Introduce la URL de la base de datos: "
setx MONGODB_URI "%MONGODB_URI%" /M
echo Variable de entorno establecida.

echo Iniciando la aplicación...
::definir el nombre del archivo aqui
java -jar mi-aplicacion.jar
::para evitar que se cierre y acabe el proceso
pause


::Se puede lanzar manualmente con una ruta dinamica de la siguiente manera:
:: set MONGODB_URI= ruta
:: java -jar mi-aplicacion.jar
