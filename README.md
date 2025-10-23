# Indicadores Chile

Esta aplicación web API entrega datos sobre cuanto valen ciertas divisas en días determinados.

# Imagen Docker
Para que GitHub cree una imagen de Docker, debes configurar una GitHub Action que use un Dockerfile en tu repositorio. La acción automática incluirá pasos para clonar tu código, iniciar sesión en el registro de contenedores (como GitHub Packages o Docker Hub), construir la imagen y luego enviarla al registro.


1. Prepara tu repositorio
  - Crea o asegúrate de tener un Dockerfile: Este archivo contendrá todas las instrucciones para construir tu imagen.
  - Sube el Dockerfile y tu código: Si aún no lo has hecho, sube el Dockerfile y los archivos de tu aplicación a tu repositorio de GitHub.
2.Configura las credenciales de seguridad
  - Crea un Personal Access Token (PAT): Ve a la configuración de desarrollador de tu cuenta de GitHub, crea un nuevo token con permisos para write:packages y read:packages.
  - Guarda el token como un secreto: En tu repositorio, ve a "Settings" > "Secrets and variables" > "Actions" y crea una nueva variable de repositorio (por ejemplo, DOCKER_TOKEN) donde almacenes el valor del token. 
3. Crea un flujo de GitHub Actions
  - Crea un archivo YAML en tu repositorio: Dentro de la carpeta .github/workflows, crea un archivo (por ejemplo, docker-image.yml).
  - Define el evento de activación: Configura el flujo para que se ejecute automáticamente cada vez que se empuje o se envíe un pull request a la rama principal (por ejemplo, on: push: branches: main).
  - Añade un trabajo (job): Este trabajo contendrá los pasos para construir y enviar la imagen.
4. Escribe el código del flujo YAML
Código

```
name: Build and Push Docker Image

on:
  push:
    branches: [ main ]

jobs:
  build-and-push-image:
    runs-on: ubuntu-latest
    steps:
    - name: Checkout code
      uses: actions/checkout@v3 # Obtiene el código de tu repositorio

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
        tags: ghcr.io/${{ github.repository }}:${{ github.sha }} ghcr.io/${{ github.repository }}:latest # Asigna etiquetas a la imagen
```

Este ejemplo utiliza la acción docker/login-action para iniciar sesión y la acción docker/build-push-action para construir y publicar. 
Las etiquetas ghcr.io/${{ github.repository }} se refieren a la URL del registro de GitHub, tu nombre de usuario y el nombre del repositorio. 

## Confirma y ejecuta el flujo
Guarda el archivo YAML en .github/workflows/ y confirma los cambios en tu repositorio. 
GitHub Actions ejecutará automáticamente este flujo en la próxima inserción en la rama main, construyendo y publicando tu imagen de Docker en GitHub Packages. 
