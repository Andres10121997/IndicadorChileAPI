# Indicadores Chile

Esta aplicación web API entrega datos sobre cuanto valen ciertas divisas en días determinados.

# GitHub Actions
Para que GitHub cree una imagen de Docker, debes configurar una GitHub Action que use un `Dockerfile` en tu repositorio. La acción automática incluirá pasos para clonar tu código, iniciar sesión en el registro de contenedores (como GitHub Packages o Docker Hub), construir la imagen y luego enviarla al registro.

## 1. Prepara tu repositorio
  * **<ins>Crea o asegúrate de tener un `Dockerfile`</ins>:** Este archivo contendrá todas las instrucciones para construir tu imagen.
  * **<ins>Sube el `Dockerfile` y tu código</ins>:** Si aún no lo has hecho, sube el `Dockerfile` y los archivos de tu aplicación a tu repositorio de GitHub.

## 2. Configura las credenciales de seguridad
  * **<ins>Crea un Personal Access Token (PAT)</ins>:** Ve a la configuración de desarrollador de tu cuenta de GitHub, crea un nuevo token con permisos para `write:packages` y `read:packages`.
  * **<ins>Guarda el token como un secreto</ins>:** En tu repositorio, ve a "Settings" > "Secrets and variables" > "Actions" y crea una nueva variable de repositorio (por ejemplo, `DOCKER_TOKEN`) donde almacenes el valor del token.

## 3. Crea un flujo de GitHub Actions
  * **<ins>Crea un archivo YAML en tu repositorio</ins>:** Dentro de la carpeta `.github/workflows`, crea un archivo (por ejemplo, `docker-image.yml`).
  * **<ins>Define el evento de activación</ins>:** Configura el flujo para que se ejecute automáticamente cada vez que se empuje o se envíe un pull request a la rama principal (por ejemplo, `on: push: branches: main`).
  * **<ins>Añade un trabajo (`job`)</ins>:** Este trabajo contendrá los pasos para construir y enviar la imagen.

## 4. Escribe el código del flujo YAML
Código

```YAML Código
name: Build and Push Docker Image

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build-and-push-image:
    runs-on: ubuntu-latest
    steps:
    - name: Checkout code
      uses: actions/checkout@v4 # Obtiene el código de tu repositorio

    - name: Log in to GitHub Container Registry
      uses: docker/login-action@v2 # Permite iniciar sesión en el registro de contenedores
      with:
        registry: ghcr.io # Usa el registro de contenedores de GitHub
        username: ${{ github.actor }} # Usa tu nombre de usuario de GitHub
        password: ${{ secrets.DOCKER_TOKEN }} # Usa el token de acceso personal guardado como secreto

    - name: Build and push Docker image
      uses: docker/build-push-action@v4 # Acción para construir y subir imágenes de Docker
      with:
        context: . # Construye desde el directorio raíz del proyecto
        push: true # Sube la imagen al registro después de construirla
        tags: | # Asigna etiquetas a la imagen
          ghcr.io/${{ github.repository }}:${{ github.sha }}
          ghcr.io/${{ github.repository }}:latest
```
 * Este ejemplo utiliza la acción `docker/login-action` para iniciar sesión y la acción `docker/build-push-action` para construir y publicar.
 * Las etiquetas `ghcr.io/${{ github.repository }}` se refieren a la URL del registro de GitHub, tu nombre de usuario y el nombre del repositorio.

## 5. Confirma y ejecuta el flujo
 * Guarda el archivo YAML en `.github/workflows/` y confirma los cambios en tu repositorio.
 * GitHub Actions ejecutará automáticamente este flujo en la próxima inserción en la rama `main`, construyendo y publicando tu imagen de Docker en GitHub Packages.

# Docker Compose
## `LANG`, `LANGUAGE` y `LC_ALL`

Para configurar `LANG`, `LANGUAGE` y `LC_ALL` en `docker-compose.yml`, debes definirlas como variables de entorno dentro de la sección `environment` de tu servicio. Esto asegura que el idioma y la configuración regional se establezcan correctamente dentro del contenedor de la aplicación, por ejemplo, `environment: - LANG=es_ES.UTF-8 - LANGUAGE=es_ES:es - LC_ALL=es_ES.UTF-8`.

### Cómo hacerlo
1. Abre tu archivo `docker-compose.yml`.
2. Busca la sección `services` y el servicio para el que quieres configurar el idioma.
3. Añade la sección `environment` si aún no existe.
4. Define las variables de entorno dentro de la sección `environment` con los valores en español.

### Ejemplo práctico
```YML
version: '3.8'

services:
  mi-aplicacion:
    image: mi-imagen-de-aplicacion
    environment:
      - LANG=es_ES.UTF-8
      - LANGUAGE=es_ES:es
      - LC_ALL=es_ES.UTF-8
    ports:
      - "8080:80"
```

### Explicación de las variables
* **<ins>`LANG`</ins>:** Establece la configuración regional predeterminada para la mayoría de los programas.
* **<ins>`LANGUAGE`</ins>:** Se utiliza principalmente para la traducción de mensajes de interfaz de usuario y puede tener múltiples valores.
* **<ins>`LC_ALL`</ins>:** Anula todas las otras variables `LC_*` y `LANG`. Al establecerla, se aplica a todas las categorías de configuración regional.

### ¿Por qué utilizarlo?
#### `DateTimeFormat.MonthNames`
La matriz `DateTimeFormat.MonthNames` de C# considera el idioma y la configuración regional del sistema porque el objeto `DateTimeFormatInfo` que la contiene está diseñado para proporcionar información específica de la referencia cultural para el formato de fecha y hora. Esto permite que la misma aplicación muestre nombres de meses en español, inglés, o cualquier otro idioma soportado, basándose en la configuración del sistema donde se ejecuta el código, lo que garantiza que los nombres de los meses y el formato de fecha sean apropiados para el usuario final.
* **<ins>Especificidad cultural</ins>:** `DateTimeFormatInfo` es una clase que contiene propiedades para el formato de fecha y hora, como los nombres de los meses, días de la semana, y formatos de números específicos de cada cultura (por ejemplo, `es-ES` para español de España o `en-US` para inglés de Estados Unidos).
* **<ins>Adaptación automática</ins>:** Cuando se utiliza sin especificar una cultura, C# utiliza la cultura actual del sistema operativo como valor predeterminado. Esto significa que `DateTimeFormat.MonthNames` contendrá automáticamente los nombres de los meses correspondientes a esa cultura.
* **<ins>Uso explícito</ins>:** También es posible sobrescribir este comportamiento y especificar manualmente una cultura diferente al formatear una fecha, por ejemplo, utilizando `ToString("MMMM", new CultureInfo("en-US"))` para forzar el nombre del mes en inglés.
