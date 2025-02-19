# TFG2024-AntonioPRaulS
## TFG 2024 - Aplicación Móvil

### Gestión de Conexiones Juego-Servidor

Las conexiones del juego con el servidor se gestionan mediante una **API**. La ruta para acceder a esta API es **modificable**, y para ello, solo necesitas acceder al archivo `server_config.json` dentro de la carpeta `./Assets/Resources`.

### Parámetros a configurar en el archivo `server_config.json`:

- **`baseURL`**:  
  Puede ser tanto `"http://localhost"` como una URL de internet. Define la dirección base del servidor.

- **`port`** *(opcional)*:  
  Parámetro opcional. En caso de que la dirección se encuentre en un puerto personalizado dentro de la URL.

- **`apiVersion`**:  
  Define la versión de la API que se está utilizando, lo que permite cambiar fácilmente entre diferentes versiones de la misma.

