# **UserPermissionsN5 - Web API para Gestión de Permisos**

Este proyecto implementa una **Web API** utilizando **.NET Core** para gestionar permisos de empleados. La API permite realizar operaciones de **solicitar**, **modificar** y **consultar** permisos. Además, los registros de cada operación se almacenan en **Elasticsearch** y se envían a **Apache Kafka** para su posterior análisis.

## **Características principales**

- **Gestión de permisos**: Permite realizar las operaciones de solicitar, modificar y consultar permisos de empleados.
- **Persistencia**: La aplicación utiliza **SQL Server** para almacenar la información de los permisos.
- **Elasticsearch**: Los registros de cada operación se indexan en **Elasticsearch** para un acceso rápido y eficiente.
- **Apache Kafka**: Los cambios en permisos se envían a un **topic** de **Kafka** para procesamiento en tiempo real.
- **Patrones de diseño**: Se implementan los patrones de **Repositorio** y **Unit of Work** .

## **Tecnologías utilizadas**

- **.NET 8**: Framework de desarrollo.
- **SQL Server**: Base de datos relacional.
- **Entity Framework Core**: ORM para interactuar con SQL Server.
- **Elasticsearch**: Motor de búsqueda para almacenamiento y búsqueda rápida de datos.
- **Apache Kafka**: Sistema de mensajería distribuido.
- **Serilog**: Biblioteca para logging.
- **Docker**: Contenedores para ejecutar Elasticsearch y Kafka localmente.

## **Requisitos previos**

- **.NET 5/6/7 SDK** (o superior)
- **SQL Server** (o Docker para contenedores)
- **Docker** (opcional, si deseas ejecutar Elasticsearch y Kafka localmente)
- **Elasticsearch** (puede ejecutarse en un contenedor de Docker)
- **Apache Kafka** (puede ejecutarse en un contenedor de Docker)

## **Instalación y configuración**

1. **Clonar el repositorio**

   ```bash
   git clone https://github.com/tu-usuario/UserPermissionsN5.git
   cd UserPermissionsN5
   ```

2. **Configuración de Elasticsearch**  
   Si usas Docker para Elasticsearch, puedes usar el siguiente archivo `docker-compose.yml`:

   ```yaml
   version: '3'
   services:
     elasticsearch:
       image: docker.elastic.co/elasticsearch/elasticsearch:7.10.1
       environment:
         - discovery.type=single-node
         - ES_JAVA_OPTS=-Xms512m -Xmx512m
       ports:
         - "9200:9200"
   ```

   Ejecuta el siguiente comando para iniciar Elasticsearch en Docker:

   ```bash
   docker-compose up
   ```

3. **Configuración de Apache Kafka**  
   Si usas Docker para Kafka, asegúrate de tener el siguiente `docker-compose.yml`:

   ```yaml
   version: '3'
   services:
     zookeeper:
       image: wurstmeister/zookeeper
       ports:
         - "2181:2181"
     kafka:
       image: wurstmeister/kafka
       ports:
         - "9092:9092"
       environment:
         KAFKA_ADVERTISED_LISTENERS: INSIDE://kafka:9093
         KAFKA_LISTENER_SECURITY_PROTOCOL: PLAINTEXT
         KAFKA_LISTENER_NAME_EXTERNAL: EXTERNAL
         KAFKA_LISTENER_NAME_INSIDE: INSIDE
         KAFKA_LISTENER_PORT: 9093
         KAFKA_ZOOKEEPER_CONNECT: zookeeper:2181
   ```

   Ejecuta el siguiente comando para iniciar Kafka en Docker:

   ```bash
   docker-compose up
   ```

4. **Configuración de la base de datos SQL Server**

   Asegúrate de tener **SQL Server** en ejecución (ya sea en un contenedor Docker o en una instancia local). Luego, realiza las migraciones necesarias utilizando Entity Framework Core:

   ```bash
   dotnet ef database update
   ```

5. **Ejecutar la API**

   Una vez configurados todos los servicios, puedes ejecutar la API:

   ```bash
   dotnet run
   ```

   La API estará disponible en `http://localhost:5000`.

## **Uso de la API**

### **1. Solicitar Permiso**
- **Método**: `POST`
- **Endpoint**: `/api/permission/request`
- **Body**:
  ```json
  {
    "EmployeeForeName": "John",
    "EmployeeSurName": "Doe",
    "PermissionTypeId": 1,
    "PermissionDate": "2025-02-16T10:00:00"
  }
  ```

### **2. Modificar Permiso**
- **Método**: `PUT`
- **Endpoint**: `/api/permission/modify/{id}`
- **Body**:
  ```json
  {
    "EmployeeForeName": "John",
    "EmployeeSurName": "Doe",
    "PermissionTypeId": 2,
    "PermissionDate": "2025-02-18T10:00:00"
  }
  ```

### **3. Consultar Permisos**
- **Método**: `GET`
- **Endpoint**: `/api/permission/get`
- **Respuesta**:
  ```json
  [
    {
      "Id": 1,
      "EmployeeForeName": "John",
      "EmployeeSurName": "Doe",
      "PermissionType": "Sick Leave",
      "PermissionDate": "2025-02-16T10:00:00"
    }
  ]
  ```

## **Registros en Elasticsearch y Kafka**

Cada operación realizada en la API (solicitar, modificar, obtener) se guarda en un **índice de Elasticsearch** llamado **"permissions"** y se envía a un **topic Kafka** llamado **"permissions"**.

## **Tests**

Se han implementado **pruebas unitarias** y **de integración** para cada uno de los servicios de la API. Puedes ejecutarlas con el siguiente comando:

```bash
dotnet test
```

## **Logs**

El proyecto usa **Serilog** para el registro de eventos. Los logs se almacenan tanto en la consola como en un archivo de texto (`Logs/app.log`).
