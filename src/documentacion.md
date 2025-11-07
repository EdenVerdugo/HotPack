# Documentación del Proyecto HotPack

## Índice

1. [Introducción](#introducción)
2. [Clases de Configuración](#clases-de-configuración)
   - [AppConfiguration](#appconfiguration)
   - [AppConfigurationItemModel](#appconfigurationitemmodel)
   - [Globals](#globals)
3. [Clases Base](#clases-base)
   - [Castable](#castable)
   - [Result](#result)
4. [Clases de Datos](#clases-de-datos)
   - [ConnectionString](#connectionstring)
   - [ONotifyPropertyChanged](#onotifypropertychanged)
5. [Extensiones](#extensiones)
   - [StringExtensions](#stringextensions)
   - [IEnumerableExtensions](#ienumerableextensions)
   - [DateTimeExtensions](#datetimeextensions)
6. [Helpers](#helpers)
   - [ExpressionsHelper](#expressionshelper)
   - [FileHelper](#filehelper)
7. [Utilidades](#utilidades)
   - [Encrypter](#encrypter)
   - [Log](#log)

---

## Introducción

HotPack es una librería de utilidades para .NET que proporciona una colección completa de herramientas, extensiones y helpers para facilitar el desarrollo de aplicaciones. Incluye funcionalidades para configuración de aplicaciones, manejo de datos, logging, encriptación y muchas extensiones útiles.

---

## Clases de Configuración

### AppConfiguration

**Namespace:** `HotPack.App`

**Propósito:** Clase para manejar la configuración de la aplicación a través de archivos XML encriptados. Permite almacenar parámetros y cadenas de conexión de forma segura.

#### Propiedades
- `Path`: Ruta del archivo de configuración

#### Constructores
- `AppConfiguration()`: Crea un archivo de configuración en la ruta por defecto
- `AppConfiguration(string path)`: Crea un archivo de configuración en la ruta especificada
- `AppConfiguration(string path, Encrypter encrypter)`: Crea un archivo con encriptador personalizado
- `AppConfiguration(Encrypter encrypter)`: Usa encriptador personalizado con ruta por defecto

#### Métodos Principales

**Parámetros:**
- `ConnectionString(string name)`: Obtiene una cadena de conexión
- `ConnectionString(string name, bool decrypt, bool createIfNotExists, string defaultValue)`: Obtiene/crea cadena de conexión con opciones
- `Create(AppConfigurationType type, string name, string value, bool encrypt, string comment)`: Crea un nuevo parámetro
- `Update(AppConfigurationType type, string name, string value, bool encrypt, string comment)`: Actualiza un parámetro existente
- `ListAll(AppConfigurationType type, bool decrypt)`: Lista todos los elementos del tipo especificado
- `TryParameter(string name, bool decrypt, Func<Castable?> onError, string comment)`: Intenta obtener un parámetro con callback de error

#### Ejemplo de Uso

```csharp
// Crear configuración con encriptación personalizada
var encrypter = new Encrypter("MiClaveSecreta");
var config = new AppConfiguration(@"C:\MiApp\config.xml", encrypter);

// Crear parámetro encriptado
config.Create(AppConfigurationType.Parameter, "ApiKey", "mi-api-key-secreta", true);

// Obtener parámetro con valor por defecto si no existe
var timeout = config.TryParameter("Timeout", false, () => new Castable("30"));

// Crear cadena de conexión
config.Create(AppConfigurationType.ConexionString, "MainDB", 
    "Server=localhost;Database=MyApp;Trusted_Connection=true;", false);

// Obtener cadena de conexión desencriptada
string connStr = config.ConnectionString("MainDB", true);
```

---

### AppConfigurationItemModel

**Namespace:** `HotPack.App`

**Propósito:** Modelo que representa un elemento de configuración (parámetro o cadena de conexión).

#### Propiedades
- `Type`: Tipo de configuración (Parameter o ConexionString)
- `Name`: Nombre del elemento
- `Value`: Valor como objeto Castable

#### Constructores
- `AppConfigurationItemModel()`: Constructor vacío
- `AppConfigurationItemModel(AppConfigurationType type, string name, string value)`: Constructor completo

#### Ejemplo de Uso

```csharp
var item = new AppConfigurationItemModel(
    AppConfigurationType.Parameter, 
    "MaxRetries", 
    "5"
);

int maxRetries = item.Value.ToInt32();
```

---

### Globals

**Namespace:** `HotPack.App`

**Propósito:** Clase singleton que proporciona información global de la aplicación y métodos para detectar el tipo de aplicación.

#### Propiedades
- `ApplicationPath`: Ruta base de la aplicación
- `ApplicationName`: Nombre de la aplicación
- `ApplicationVersion`: Versión de la aplicación

#### Métodos Estáticos
- `IsAspNetCoreApp()`: Verifica si es una aplicación ASP.NET Core
- `IsDesktopApp()`: Verifica si es una aplicación de escritorio (WinForms/WPF)
- `IsConsoleApp()`: Verifica si es una aplicación de consola

#### Ejemplo de Uso

```csharp
// Obtener información de la aplicación
string appName = Globals.Instance.ApplicationName;
string version = Globals.Instance.ApplicationVersion;

// Detectar tipo de aplicación
if (Globals.IsAspNetCoreApp())
{
    Console.WriteLine("Esta es una aplicación web ASP.NET Core");
}
else if (Globals.IsDesktopApp())
{
    Console.WriteLine("Esta es una aplicación de escritorio");
}
else if (Globals.IsConsoleApp())
{
    Console.WriteLine("Esta es una aplicación de consola");
}
```

---

## Clases Base

### Castable

**Namespace:** `HotPack.Classes`

**Propósito:** Clase que encapsula un objeto y proporciona métodos seguros de conversión de tipos con manejo de errores.

#### Métodos de Conversión

**Conversiones Seguras (lanzan excepción si fallan):**
- `ToInt32()`: Convierte a entero de 32 bits
- `ToUint32()`: Convierte a entero sin signo de 32 bits
- `ToDecimal()`: Convierte a decimal
- `ToSingle()`: Convierte a float
- `ToDouble()`: Convierte a double
- `ToInt64()`: Convierte a long
- `ToUInt64()`: Convierte a ulong
- `ToBoolean()`: Convierte a boolean
- `ToByte()`: Convierte a byte
- `ToBytes()`: Convierte a array de bytes
- `ToDateTime()`: Convierte a DateTime

**Conversiones con Callback de Error:**
- `TryInt32(Func<Castable, int> onError)`: Intenta conversión con callback
- `TryDecimal(Func<Castable, decimal> onError)`: Intenta conversión con callback
- (Similar para todos los tipos)

#### Ejemplo de Uso

```csharp
// Conversión segura
var castable = new Castable("123");
int numero = castable.ToInt32(); // 123

// Conversión con manejo de error
var castableTexto = new Castable("abc");
int numeroConError = castableTexto.TryInt32(c => 0); // Retorna 0 si falla

// Conversión de fecha
var castableFecha = new Castable("2023-12-01");
DateTime fecha = castableFecha.ToDateTime();

// Conversión con callback personalizado
var resultado = castable.TryDecimal(c => {
    Console.WriteLine($"Error convirtiendo: {c}");
    return -1m;
});
```

---

### Result

**Namespace:** `HotPack.Classes`

**Propósito:** Clases para manejar resultados de operaciones con estado de éxito/error, mensajes y datos adicionales.

#### Clases Disponibles
- `Result`: Resultado básico sin tipado específico
- `Result<T>`: Resultado genérico con datos tipados
- `ResultList<T>`: Resultado especializado para listas

#### Propiedades
- `Value`: Indica si la operación fue exitosa
- `Message`: Mensaje descriptivo del resultado
- `Data`: Datos del resultado (tipado en Result<T>)
- `Code`: Código numérico del resultado
- `Bag`: Diccionario para datos adicionales

#### Métodos Principales
- `Create(bool value, string msg)`: Crea un resultado
- `Create(bool value, string msg, T data)`: Crea resultado con datos
- `Create(Exception ex)`: Crea resultado desde excepción
- `Cast<M>()`: Convierte a otro tipo
- `Map<M>(Func<T?, M> func)`: Mapea los datos a otro tipo

#### Ejemplo de Uso

```csharp
// Resultado básico
public Result ValidateUser(string username)
{
    if (string.IsNullOrEmpty(username))
        return Result.Create(false, "Username es requerido");
    
    return Result.Create(true, "Usuario válido");
}

// Resultado genérico
public Result<User> GetUser(int id)
{
    try
    {
        var user = database.GetUser(id);
        if (user == null)
            return Result<User>.Create(false, "Usuario no encontrado");
        
        return Result<User>.Create(true, "Usuario encontrado", user);
    }
    catch (Exception ex)
    {
        return Result<User>.Create(ex);
    }
}

// Uso de resultados
var userResult = GetUser(123);
if (userResult.Value)
{
    Console.WriteLine($"Usuario: {userResult.Data.Name}");
}
else
{
    Console.WriteLine($"Error: {userResult.Message}");
}

// Mapeo de resultados
var userDtoResult = userResult.Map(user => new UserDto 
{ 
    Id = user.Id, 
    Name = user.Name 
});
```

---

## Clases de Datos

### ConnectionString

**Namespace:** `HotPack.Data`

**Propósito:** Clase para construir y manejar cadenas de conexión de base de datos de forma tipo-segura.

#### Propiedades
- `Server`: Servidor de base de datos
- `Database`: Nombre de la base de datos
- `User`: Usuario de conexión
- `Password`: Contraseña

#### Métodos
- `ToMsqlConnectionString()`: Genera cadena de conexión para SQL Server

#### Ejemplo de Uso

```csharp
// Crear cadena de conexión
var connectionString = new ConnectionString(
    "localhost", 
    "MiBaseDatos", 
    "usuario", 
    "contraseña"
);

// Obtener cadena de conexión formateada
string connStr = connectionString.ToMsqlConnectionString();
// Resultado: "data source = localhost; initial catalog = MiBaseDatos; user id = usuario; password = contraseña"

// Usar con Entity Framework o ADO.NET
using var connection = new SqlConnection(connStr);
```

---

### ONotifyPropertyChanged

**Namespace:** `HotPack.DataModel`

**Propósito:** Clase base abstracta que implementa INotifyPropertyChanged para data binding en aplicaciones WPF/WinForms.

#### Métodos
- `OnPropertyChanged(string propertyName)`: Notifica cambio por nombre de propiedad
- `OnPropertyChanged(Expression<Func<T, object>> property)`: Notifica cambio usando expresión lambda

#### Ejemplo de Uso

```csharp
public class PersonaViewModel : ONotifyPropertyChanged<PersonaViewModel>
{
    private string _nombre;
    private int _edad;

    public string Nombre
    {
        get => _nombre;
        set
        {
            _nombre = value;
            OnPropertyChanged(nameof(Nombre)); // Por string
        }
    }

    public int Edad
    {
        get => _edad;
        set
        {
            _edad = value;
            OnPropertyChanged(p => p.Edad); // Por expresión lambda (type-safe)
        }
    }
}

// En WPF XAML
// <TextBox Text="{Binding Nombre}" />
// <TextBox Text="{Binding Edad}" />
```

---

## Extensiones

### StringExtensions

**Namespace:** `System` (extiende string)

**Propósito:** Extensiones para la clase string que proporcionan funcionalidades adicionales de manipulación de texto.

#### Métodos Principales

**Transformaciones:**
- `ToCamelCase()`: Convierte a camelCase
- `ToUTF8Encoding()`: Convierte a codificación UTF-8
- `RemoveAccent()`: Elimina acentos del texto
- `ToUrlSlug()`: Convierte a formato URL slug

**Validaciones:**
- `IsNumeric()`: Verifica si la cadena es numérica
- `IsNullOrWhiteSpace()`: Verifica si es nula o solo espacios en blanco
- `Contains(params string[] containsAny)`: Verifica si contiene alguno de los valores

**Utilidades:**
- `ForEach(Action<char> action)`: Ejecuta acción para cada caracter
- `CountChar(char character)`: Cuenta ocurrencias de un caracter
- `RemoveIlegalCharactersXML()`: Elimina caracteres ilegales para XML

#### Ejemplo de Uso

```csharp
// Conversiones
string texto = "Hola_Mundo";
string camelCase = texto.ToCamelCase(); // "holaMundo"

string conAcentos = "Café con leche";
string sinAcentos = conAcentos.RemoveAccent(); // "Cafe con leche"

string titulo = "Mi Artículo Nuevo";
string slug = titulo.ToUrlSlug(); // "mi-articulo-nuevo"

// Validaciones
string numero = "123.45";
bool esNumerico = numero.IsNumeric(); // true

string buscar = "error crítico";
bool contiene = buscar.Contains("error", "crítico"); // true

// Utilidades
string palabra = "Hola";
palabra.ForEach(c => Console.Write($"{c}-")); // H-o-l-a-

long cantidadA = "banana".CountChar('a'); // 3
```

---

### IEnumerableExtensions

**Namespace:** `System.Collections.Generic`

**Propósito:** Extensiones para IEnumerable que proporcionan funcionalidades adicionales para trabajar con colecciones.

#### Métodos
- `ToObservableCollection<T>()`: Convierte a ObservableCollection
- `ToDataTable<T>(string tableName, params Expression<Func<T, object>>[] includeOnly)`: Convierte a DataTable
- `ToDataTable<T>(string tableName)`: Convierte a DataTable con todas las propiedades

#### Ejemplo de Uso

```csharp
public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    public DateTime FechaCreacion { get; set; }
}

var productos = new List<Producto>
{
    new Producto { Id = 1, Nombre = "Laptop", Precio = 999.99m, FechaCreacion = DateTime.Now },
    new Producto { Id = 2, Nombre = "Mouse", Precio = 25.50m, FechaCreacion = DateTime.Now }
};

// Convertir a ObservableCollection para WPF binding
var observable = productos.ToObservableCollection();

// Convertir a DataTable con todas las propiedades
DataTable tabla = productos.ToDataTable("Productos");

// Convertir a DataTable solo con propiedades específicas
DataTable tablaReducida = productos.ToDataTable("ProductosReducida", 
    p => p.Nombre, 
    p => p.Precio
);

// Usar DataTable con ADO.NET o para export
// tabla puede ser usado con SqlBulkCopy, exportar a Excel, etc.
```

---

### DateTimeExtensions

**Namespace:** `System`

**Propósito:** Extensiones para la clase DateTime.

#### Métodos
- `Default()`: Retorna fecha por defecto (1900-01-01)

#### Ejemplo de Uso

```csharp
DateTime fecha = DateTime.Now;
DateTime fechaDefault = fecha.Default(); // 1900-01-01
```

---

## Helpers

### ExpressionsHelper

**Namespace:** `HotPack.Helpers`

**Propósito:** Utilidades para trabajar con expresiones lambda y obtener información de propiedades de forma tipo-segura.

#### Métodos
- `GetPropertyName<T>(Expression<Func<T, object>> expression)`: Obtiene el nombre de una propiedad desde una expresión lambda

#### Ejemplo de Uso

```csharp
public class Usuario
{
    public string Nombre { get; set; }
    public int Edad { get; set; }
}

// Obtener nombre de propiedad de forma tipo-segura
string nombrePropiedad = ExpressionsHelper.GetPropertyName<Usuario>(u => u.Nombre);
Console.WriteLine(nombrePropiedad); // "Nombre"

// Útil para validaciones, mapeos, reflection, etc.
string edadPropiedad = ExpressionsHelper.GetPropertyName<Usuario>(u => u.Edad);

// Ejemplo de uso en validación
public void ValidateProperty<T>(Expression<Func<T, object>> property, object value)
{
    string propName = ExpressionsHelper.GetPropertyName(property);
    // Realizar validación usando propName...
}

ValidateProperty<Usuario>(u => u.Nombre, "Juan");
```

---

### FileHelper

**Namespace:** `HotPack.Helpers`

**Propósito:** Utilidades para trabajar con archivos, incluyendo un extenso mapeo de tipos MIME y operaciones de archivo.

#### Características
- Mapeo completo de extensiones de archivo a tipos MIME
- Soporte para archivos comunes (.pdf, .docx, .jpg, .mp4, etc.)
- Más de 500 tipos MIME predefinidos

#### Ejemplo de Uso

```csharp
// El FileHelper contiene un diccionario extenso de tipos MIME
// Útil para aplicaciones web que necesitan determinar el Content-Type

// Ejemplos de tipos MIME incluidos:
// .pdf -> application/pdf
// .jpg -> image/jpeg  
// .docx -> application/vnd.openxmlformats-officedocument.wordprocessingml.document
// .mp4 -> video/mp4
// .zip -> application/zip
```

---

## Utilidades

### Encrypter

**Namespace:** `HotPack.Utilities`

**Propósito:** Clase para encriptar y desencriptar texto usando una clave personalizada con algoritmo simétrico.

#### Constructores
- `Encrypter(string userKey)`: Crea encriptador con clave personalizada

#### Métodos
- `Encrypt(string? text)`: Encripta el texto
- `Decrypt(string? text)`: Desencripta el texto
- `EncryptString(string userKey, string? text, Mode mode)`: Método estático para encriptar/desencriptar

#### Enumeración Mode
- `Encrypt`: Modo de encriptación
- `Decrypt`: Modo de desencriptación

#### Ejemplo de Uso

```csharp
// Crear encriptador con clave personalizada
var encrypter = new Encrypter("MiClaveSecreta123");

// Encriptar texto sensible
string textoOriginal = "Información confidencial";
string textoEncriptado = encrypter.Encrypt(textoOriginal);

Console.WriteLine($"Encriptado: {textoEncriptado}");

// Desencriptar
string textoDesencriptado = encrypter.Decrypt(textoEncriptado);
Console.WriteLine($"Desencriptado: {textoDesencriptado}"); // "Información confidencial"

// Uso estático
string encriptadoEstatico = Encrypter.EncryptString(
    "OtraClaveSecreta", 
    "Datos importantes", 
    Encrypter.Mode.Encrypt
);

// Ejemplo con configuración
var config = new AppConfiguration();
config.Create(AppConfigurationType.Parameter, "ApiKey", "mi-api-key", true); // true = encriptar

// La clase AppConfiguration usa Encrypter internamente
```

---

### Log

**Namespace:** `HotPack.Utilities`

**Propósito:** Sistema de logging que organiza automáticamente los logs por fecha y aplicación con estructura de carpetas jerárquica.

#### Características
- Logs organizados por año y mes: `C:\Logs\[AppName]\[YYYY]\[MM]\`
- Nombres de archivo con fecha: `[YYYY-MM-DD] AppName.log`
- Mensajes con timestamp automático
- Creación automática de directorios

#### Constructores
- `Log()`: Constructor por defecto (usa ruta automática)
- `Log(string path)`: Constructor con ruta personalizada

#### Métodos
- `WriteLog(string message)`: Escribe al log de instancia
- `Write(string message)`: Método estático para escritura rápida
- `Write(string message, string saveDirectory)`: Escribe en directorio específico

#### Ejemplo de Uso

```csharp
// Logging estático (más común)
Log.Write("Aplicación iniciada correctamente");
Log.Write("Error al procesar usuario ID: 123");

// Con directorio personalizado
Log.Write("Backup completado", @"C:\Logs\Backups");

// Logging con instancia personalizada
var customLog = new Log(@"C:\MiApp\logs\custom.log");
customLog.WriteLog("Evento personalizado registrado");

// Ejemplo en aplicación real
public class ServicioUsuarios
{
    public Result<Usuario> CrearUsuario(Usuario usuario)
    {
        try
        {
            Log.Write($"Iniciando creación de usuario: {usuario.Email}");
            
            // Lógica de creación...
            
            Log.Write($"Usuario creado exitosamente: ID {usuario.Id}");
            return Result<Usuario>.Create(true, "Usuario creado", usuario);
        }
        catch (Exception ex)
        {
            Log.Write($"Error al crear usuario {usuario.Email}: {ex.Message}");
            return Result<Usuario>.Create(ex);
        }
    }
}

// Estructura de archivos generada:
// C:\Logs\MiApp\2023\12\[2023-12-01] MiApp.log
// C:\Logs\MiApp\2023\12\[2023-12-02] MiApp.log
```

---

## Conclusión

HotPack proporciona un conjunto completo de herramientas que cubren las necesidades más comunes en el desarrollo de aplicaciones .NET:

- **Configuración**: Manejo seguro de configuraciones con encriptación
- **Conversiones**: Sistema robusto de conversión de tipos
- **Resultados**: Patrón Result para manejo de operaciones
- **Extensiones**: Amplia gama de extensiones para tipos básicos
- **Logging**: Sistema de logs organizados automáticamente
- **Utilidades**: Encriptación, helpers de expresiones, y más

Esta librería está diseñada para mejorar la productividad del desarrollador proporcionando implementaciones reutilizables y bien probadas de funcionalidades comunes.