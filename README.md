# TFG2024-AntonioPRaulS
## TFG 2024 - Aplicación Móvil

### Gestión de Conexiones Juego - Base de Datos

## Conexión cliente - API
Las conexiones del juego con el servidor se gestionan mediante una **API**. La ruta para acceder a esta API es **modificable**, y para ello, solo necesitas acceder al archivo `server_config.json` dentro de la carpeta `./Assets/Resources`del proyecto de Unity.

# Parámetros a configurar en el archivo `server_config.json`:

- **`baseURL`**:  
  Puede ser tanto `"http://localhost"` como una URL de internet. Define la dirección base del servidor.

- **`port`** *(opcional)*:  
  Parámetro opcional. En caso de que la dirección se encuentre en un puerto personalizado dentro de la URL.

- **`apiVersion`**:  
  Define la versión de la API que se está utilizando, lo que permite cambiar fácilmente entre diferentes versiones de la misma.

## Conexión API - Base de Datos
La base de datos a la que ataca la API pueden ser definidas dinámicamente. Para esto tenemos dos opciones, todos los archivos que se nombran aquí se encuentran en `DataBase\apiDinamica`.

# Definición manual:
1. Abrimos una consola como administrador, recomendamos situarnos en el directorio `DataBase\apiDinamica`.
2. Creamos variables de entorno.
     2.1 Si es una base de datos en remoto: definimos _MONGODB_URI_ y le asignamos la url de la base de datos.
     2.2 Si es una base de datos en local: definimos _MONGO_HOST_ y _MONGO_PORT_ y le asignamos el host y el puerto correspondientes.
3. Lanzamos el (nombre-jar-remoto).jar o (nombre-jar-local).jar según si lo hemos configurado para remoto o local respectivamente.

# Definición guiada:
1. Ejecutamos `launcher.bat` desde una consola como administrador ubicada en `DataBase\apiDinamica`.
2. Seleccionamos la opción correspondiente, según queramos remoto o local.
3. Introducimos las variables locales según nos la pida.
4. Automáticamente, `launcher.bat`lanzará el .jar correspondiente a la modalidad que hayamos seleccionado.

# Con estos dos pasos se puede configurar de manera sencilla el flujo de datos.

