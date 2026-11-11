# Indicadores Chile API

## `launchSettings.json`
Ejemplo de lo que debería tener el documento `launchSettings.json`
```json
{
  "profiles": {
    "Container (Dockerfile)": {
      "commandName": "Docker",
      "launchBrowser": true,
      "launchUrl": "{Scheme}://{ServiceHost}:{ServicePort}/scalar/v1",
      "environmentVariables": {
        "ASPNETCORE_HTTPS_PORTS": "8081",
        "ASPNETCORE_HTTP_PORTS": "8080"
      },
      "publishAllPorts": true,
      "useSSL": true
    }
  }
}
```
