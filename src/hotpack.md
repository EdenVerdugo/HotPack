# Documentación HotPack

## Índice

1. [Result](#result)
   - [Propiedades](#propiedades)
   - [Constructores](#constructores)
   - [Métodos](#métodos)
   - [Casos de Uso](#casos-de-uso-result)
2. [Result\<T\>](#resultt)
   - [Propiedades](#propiedades-1)
   - [Constructores](#constructores-1)
   - [Métodos](#métodos-1)
   - [Casos de Uso](#casos-de-uso-resultt)
3. [ResultList\<T\>](#resultlistt)
   - [Descripción](#descripción-resultlist)
   - [Casos de Uso](#casos-de-uso-resultlistt)
4. [ResultBagItem](#resultbagitem)
   - [Propiedades](#propiedades-2)
   - [Constructores](#constructores-2)
   - [Casos de Uso](#casos-de-uso-resultbagitem)
5. [Conexion](#conexion)
   - [Constructor](#constructor-conexion)
   - [Métodos Principales](#métodos-principales)
   - [Casos de Uso](#casos-de-uso-conexion)
6. [Clases Auxiliares](#clases-auxiliares)
   - [ConexionParameters](#conexionparameters)
   - [ConexionCastableRow](#conexioncastablerow)
   - [ConexionMultipleReader](#conexionmultiplereader)
   - [ConexionColumnAttribute](#conexioncolumnattribute)

---

## Result

**Namespace:** `HotPack.Classes`  
**Archivo:** [HotPack/Classes/Result.cs](HotPack/Classes/Result.cs)

### Descripción
La clase [`Result`](HotPack/Classes/Result.cs) es una estructura de respuesta estándar para operaciones que encapsula el resultado de ejecución (éxito/fallo), un mensaje descriptivo, datos opcionales y metadatos adicionales. Es ideal para crear APIs consistentes y manejar respuestas uniformes en toda la aplicación.

### Propiedades

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `Value` | `bool` | Indica si la operación fue exitosa (`true`) o falló (`false`) |
| `Message` | `string` | Mensaje descriptivo del resultado de la operación |
| `Data` | `object?` | Datos adicionales de cualquier tipo asociados al resultado |
| `Code` | `int` | Código numérico del resultado (útil para códigos HTTP o de error) |
| `Bag` | `Dictionary<string, object>` | Diccionario para almacenar metadatos adicionales clave-valor |

### Constructores

```csharp
// Constructor vacío
public Result()

// Constructor con valor booleano y mensaje
public Result(bool value, string msg)

// Constructor con valor, mensaje y datos
public Result(bool value, string msg, object data)

// Constructor desde una excepción
public Result(Exception ex)
```

### Métodos

#### Métodos Estáticos

##### `Create(bool value, string msg)`
Crea una nueva instancia de Result de forma estática.

**Parámetros:**
- `value` (bool): Indica si la operación fue exitosa
- `msg` (string): Mensaje descriptivo

**Retorna:** Nueva instancia de [`Result`](HotPack/Classes/Result.cs)

**Ejemplo:**
```csharp
var result = Result.Create(true, "Operación exitosa");
```

##### `Create(bool value, string msg, object data)`
Crea una nueva instancia de Result con datos.

**Parámetros:**
- `value` (bool): Indica si fue exitoso
- `msg` (string): Mensaje descriptivo
- `data` (object): Datos a incluir

**Retorna:** Nueva instancia de [`Result`](HotPack/Classes/Result.cs)

##### `Create(Exception ex)`
Crea una instancia de Result desde una excepción.

**Parámetros:**
- `ex` (Exception): Excepción capturada

**Retorna:** [`Result`](HotPack/Classes/Result.cs) con `Value = false` y el mensaje de la excepción

#### Métodos de Instancia

##### `Cast<M>()`
Convierte el Result actual a un Result genérico de tipo M, preservando el estado y metadatos.

**Retorna:** [`Result<M>`](HotPack/Classes/Result.cs) con los mismos valores de `Value`, `Message`, `Code` y `Bag`

**Ejemplo:**
```csharp
Result result = new Result(true, "OK");
Result<string> typedResult = result.Cast<string>();
```

##### `Cast<M>(M? data)`
Convierte el Result actual a un Result genérico de tipo M con datos específicos.

**Parámetros:**
- `data` (M?): Datos a incluir en el Result convertido

**Retorna:** [`Result<M>`](HotPack/Classes/Result.cs)

**Ejemplo:**
```csharp
Result result = new Result(true, "Usuario encontrado");
Result<Usuario> userResult = result.Cast<Usuario>(usuario);
```

##### `InfoMessage()` y `InfoMessage(string infoMessageValue)`
Gestiona un mensaje de información adicional interno.

**Retorna:** string con el mensaje informativo

### Casos de Uso Result

#### Caso 1: Validación de Datos

```csharp
using HotPack.Classes;

public class UsuarioService
{
    public Result ValidarUsuario(string email, string password)
    {
        if (string.IsNullOrEmpty(email))
        {
            return new Result(false, "El email es requerido");
        }

        if (password.Length < 8)
        {
            return new Result(false, "La contraseña debe tener al menos 8 caracteres");
        }

        return new Result(true, "Usuario válido");
    }
}

// Uso
var service = new UsuarioService();
var result = service.ValidarUsuario("user@email.com", "12345");

if (!result.Value)
{
    Console.WriteLine($"Error de validación: {result.Message}");
}
```

#### Caso 2: Manejo de Excepciones

```csharp
public Result GuardarArchivo(string ruta, byte[] contenido)
{
    try
    {
        File.WriteAllBytes(ruta, contenido);
        return Result.Create(true, "Archivo guardado exitosamente");
    }
    catch (UnauthorizedAccessException ex)
    {
        var result = new Result(ex);
        result.Code = 403;
        return result;
    }
    catch (Exception ex)
    {
        var result = new Result(ex);
        result.Code = 500;
        return result;
    }
}

// Uso
var resultado = GuardarArchivo("C:/temp/file.dat", data);
if (!resultado.Value)
{
    Console.WriteLine($"Error {resultado.Code}: {resultado.Message}");
}
```

#### Caso 3: Respuesta de API con Metadatos

```csharp
public Result ObtenerDatos()
{
    var result = new Result(true, "Datos obtenidos correctamente");
    result.Code = 200;
    result.Data = new { Nombre = "Juan", Edad = 30 };
    
    // Agregar metadatos al Bag
    result.Bag.Add("TotalRegistros", 1500);
    result.Bag.Add("PaginaActual", 1);
    result.Bag.Add("Timestamp", DateTime.Now);
    result.Bag.Add("Version", "1.0.0");
    
    return result;
}

// Uso
var resultado = ObtenerDatos();
Console.WriteLine($"Total de registros: {resultado.Bag["TotalRegistros"]}");
Console.WriteLine($"Versión: {resultado.Bag["Version"]}");
```

#### Caso 4: Conversión entre Tipos de Result

```csharp
public Result ProcesarDatos()
{
    var result = new Result(true, "Proceso completado");
    result.Code = 200;
    result.Bag.Add("DuracionMs", 150);
    
    return result;
}

// Convertir a Result genérico cuando necesitamos tipar los datos
var resultado = ProcesarDatos();
var resultadoTipado = resultado.Cast<List<string>>(new List<string> { "Item1", "Item2" });

Console.WriteLine($"Elementos: {resultadoTipado.Data.Count}");
Console.WriteLine($"Duración: {resultadoTipado.Bag["DuracionMs"]} ms");
```

---

## Result\<T\>

**Namespace:** `HotPack.Classes`  
**Archivo:** [HotPack/Classes/Result.cs](HotPack/Classes/Result.cs)

### Descripción
La clase [`Result<T>`](HotPack/Classes/Result.cs) es la versión genérica de Result que proporciona tipado fuerte para la propiedad `Data`. Esto permite trabajar con datos específicos de forma segura en tiempo de compilación y aprovecha IntelliSense en el IDE.

### Propiedades

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `Value` | `bool` | Indica si la operación fue exitosa |
| `Message` | `string` | Mensaje descriptivo del resultado |
| `Data` | `T?` | Datos tipados asociados al resultado |
| `Code` | `int` | Código numérico del resultado |
| `Bag` | `Dictionary<string, object>` | Diccionario para metadatos adicionales |

### Constructores

```csharp
// Constructor vacío
public Result()

// Constructor con valor y mensaje
public Result(bool value, string msg)

// Constructor con valor, mensaje y datos tipados
public Result(bool value, string msg, T data)

// Constructor desde excepción
public Result(Exception ex)

// Constructor desde Result no genérico
public Result(Result origin)
```

### Métodos

#### Métodos Estáticos

##### `Create(bool value, string msg)`
Crea una nueva instancia de Result\<T\>.

**Retorna:** Nueva instancia de [`Result`](HotPack/Classes/Result.cs) (no genérico)

##### `Create(bool value, string msg, T data)`
Crea una instancia con datos tipados.

**Parámetros:**
- `value` (bool): Éxito o fallo
- `msg` (string): Mensaje
- `data` (T): Datos tipados

**Retorna:** Nueva instancia de [`Result`](HotPack/Classes/Result.cs)

##### `Create(Exception ex)`
Crea una instancia desde una excepción.

**Retorna:** [`Result`](HotPack/Classes/Result.cs) con el error

#### Métodos de Instancia

##### `Cast<M>()`
Convierte el Result\<T\> actual a un Result\<M\>.

**Retorna:** [`Result<M>`](HotPack/Classes/Result.cs) sin datos, solo con estado y metadatos

**Ejemplo:**
```csharp
Result<Usuario> userResult = GetUser();
Result<UsuarioDto> dtoResult = userResult.Cast<UsuarioDto>();
```

##### `Cast<M>(M? data)`
Convierte a Result\<M\> incluyendo datos específicos.

**Parámetros:**
- `data` (M?): Datos del nuevo tipo

**Retorna:** [`Result<M>`](HotPack/Classes/Result.cs) con datos

**Ejemplo:**
```csharp
Result<Usuario> userResult = GetUser();
var dto = mapper.Map<UsuarioDto>(userResult.Data);
Result<UsuarioDto> dtoResult = userResult.Cast<UsuarioDto>(dto);
```

##### `Map<M>(Func<T?, M> func)`
Transforma los datos del Result usando una función de mapeo. Ideal para integración con AutoMapper u otras librerías.

**Parámetros:**
- `func` (Func<T?, M>): Función que transforma los datos de tipo T a tipo M

**Retorna:** [`Result<M>`](HotPack/Classes/Result.cs) con los datos transformados

**Ejemplo:**
```csharp
Result<Usuario> userResult = GetUser();

// Transformación manual
var dtoResult = userResult.Map(user => new UsuarioDto
{
    Nombre = user.Nombre,
    Email = user.Email
});

// Con AutoMapper
var mappedResult = userResult.Map(user => _mapper.Map<UsuarioDto>(user));
```

### Casos de Uso Result\<T\>

#### Caso 1: Servicio de Datos Tipado

```csharp
using HotPack.Classes;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Email { get; set; }
    public DateTime FechaRegistro { get; set; }
}

public class UsuarioRepository
{
    public async Task<Result<Usuario>> ObtenerPorIdAsync(int id)
    {
        try
        {
            var usuario = await _dbContext.Usuarios.FindAsync(id);
            
            if (usuario == null)
            {
                return new Result<Usuario>(false, "Usuario no encontrado");
            }
            
            return new Result<Usuario>(true, "Usuario obtenido correctamente", usuario);
        }
        catch (Exception ex)
        {
            return new Result<Usuario>(ex);
        }
    }
}

// Uso
var repository = new UsuarioRepository();
var resultado = await repository.ObtenerPorIdAsync(123);

if (resultado.Value && resultado.Data != null)
{
    Console.WriteLine($"Usuario: {resultado.Data.Nombre}");
    Console.WriteLine($"Email: {resultado.Data.Email}");
}
else
{
    Console.WriteLine($"Error: {resultado.Message}");
}
```

#### Caso 2: API Controller con Result\<T\>

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _productoService;

    public ProductosController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerProducto(int id)
    {
        var resultado = await _productoService.ObtenerPorIdAsync(id);
        
        if (!resultado.Value)
        {
            return StatusCode(resultado.Code, new 
            { 
                mensaje = resultado.Message,
                codigo = resultado.Code
            });
        }
        
        return Ok(new
        {
            success = true,
            data = resultado.Data,
            mensaje = resultado.Message
        });
    }

    [HttpPost]
    public async Task<IActionResult> CrearProducto([FromBody] ProductoDto dto)
    {
        var resultado = await _productoService.CrearAsync(dto);
        
        resultado.Bag.Add("FechaCreacion", DateTime.Now);
        
        if (!resultado.Value)
        {
            return BadRequest(new
            {
                success = false,
                mensaje = resultado.Message,
                errores = resultado.Bag
            });
        }
        
        return CreatedAtAction(
            nameof(ObtenerProducto), 
            new { id = resultado.Data.Id }, 
            resultado.Data
        );
    }
}
```

#### Caso 3: Transformación de Datos con Map

```csharp
public class UsuarioDto
{
    public string NombreCompleto { get; set; }
    public string CorreoElectronico { get; set; }
}

public class UsuarioService
{
    private readonly UsuarioRepository _repository;

    public async Task<Result<UsuarioDto>> ObtenerUsuarioDtoAsync(int id)
    {
        var userResult = await _repository.ObtenerPorIdAsync(id);
        
        if (!userResult.Value)
        {
            return userResult.Cast<UsuarioDto>();
        }
        
        // Transformar Usuario a UsuarioDto
        var dtoResult = userResult.Map(usuario => new UsuarioDto
        {
            NombreCompleto = usuario.Nombre,
            CorreoElectronico = usuario.Email
        });
        
        return dtoResult;
    }
}

// Uso
var service = new UsuarioService();
var resultado = await service.ObtenerUsuarioDtoAsync(100);

if (resultado.Value && resultado.Data != null)
{
    Console.WriteLine($"DTO: {resultado.Data.NombreCompleto}");
}
```

#### Caso 4: Encadenamiento de Operaciones

```csharp
public class PedidoService
{
    public async Task<Result<Pedido>> ProcesarPedidoAsync(int pedidoId)
    {
        // Obtener pedido
        var pedidoResult = await ObtenerPedidoAsync(pedidoId);
        if (!pedidoResult.Value)
        {
            return pedidoResult;
        }
        
        // Validar stock
        var stockResult = await ValidarStockAsync(pedidoResult.Data);
        if (!stockResult.Value)
        {
            return new Result<Pedido>(false, stockResult.Message);
        }
        
        // Procesar pago
        var pagoResult = await ProcesarPagoAsync(pedidoResult.Data);
        if (!pagoResult.Value)
        {
            return new Result<Pedido>(false, pagoResult.Message)
            {
                Code = 402,
                Bag = pagoResult.Bag
            };
        }
        
        // Actualizar estado
        pedidoResult.Data.Estado = "Procesado";
        await GuardarPedidoAsync(pedidoResult.Data);
        
        var result = new Result<Pedido>(true, "Pedido procesado exitosamente", pedidoResult.Data);
        result.Code = 200;
        result.Bag.Add("TransaccionId", pagoResult.Bag["TransaccionId"]);
        
        return result;
    }
}
```

#### Caso 5: Constructor desde Result No Genérico

```csharp
public Result ValidarDatos(object datos)
{
    // Validaciones varias...
    var result = new Result(true, "Validación exitosa");
    result.Code = 200;
    result.Bag.Add("CamposValidados", 15);
    
    return result;
}

public Result<Producto> CrearProducto(ProductoDto dto)
{
    // Validar primero
    var validacion = ValidarDatos(dto);
    
    if (!validacion.Value)
    {
        // Crear Result<Producto> desde Result
        return new Result<Producto>(validacion);
    }
    
    var producto = new Producto
    {
        Nombre = dto.Nombre,
        Precio = dto.Precio
    };
    
    return new Result<Producto>(true, "Producto creado", producto);
}
```

---

## ResultList\<T\>

**Namespace:** `HotPack.Classes`  
**Archivo:** [HotPack/Classes/Result.cs](HotPack/Classes/Result.cs)

### Descripción ResultList

La clase [`ResultList<T>`](HotPack/Classes/Result.cs) es una especialización de `Result<List<T>>` diseñada específicamente para operaciones que devuelven colecciones de elementos. Hereda toda la funcionalidad de Result\<T\> pero está semánticamente optimizada para trabajar con listas.

**Herencia:** `ResultList<T>` → `Result<List<T>>` → objeto

### Propiedades
Hereda todas las propiedades de [`Result<List<T>>`](HotPack/Classes/Result.cs):
- `Value`: bool - Indica éxito o fallo
- `Message`: string - Mensaje descriptivo
- `Data`: List\<T\>? - Lista de elementos tipados
- `Code`: int - Código de resultado
- `Bag`: Dictionary<string, object> - Metadatos adicionales

### Casos de Uso ResultList\<T\>

#### Caso 1: Listado de Productos con Paginación

```csharp
using HotPack.Classes;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    public string Categoria { get; set; }
}

public class ProductoService
{
    public ResultList<Producto> ObtenerProductos(int pagina = 1, int tamano = 10)
    {
        try
        {
            var productos = _repository.GetAll()
                .Skip((pagina - 1) * tamano)
                .Take(tamano)
                .ToList();
            
            var totalRegistros = _repository.Count();
            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamano);
            
            var result = new ResultList<Producto>
            {
                Value = true,
                Message = "Productos obtenidos correctamente",
                Code = 200,
                Data = productos
            };
            
            // Metadatos de paginación
            result.Bag.Add("TotalRegistros", totalRegistros);
            result.Bag.Add("PaginaActual", pagina);
            result.Bag.Add("TamanoPagina", tamano);
            result.Bag.Add("TotalPaginas", totalPaginas);
            result.Bag.Add("TieneAnterior", pagina > 1);
            result.Bag.Add("TieneSiguiente", pagina < totalPaginas);
            
            return result;
        }
        catch (Exception ex)
        {
            return new ResultList<Producto>
            {
                Value = false,
                Message = ex.Message,
                Code = 500
            };
        }
    }
}

// Uso
var service = new ProductoService();
var resultado = service.ObtenerProductos(pagina: 2, tamano: 20);

if (resultado.Value && resultado.Data != null)
{
    Console.WriteLine($"Mostrando {resultado.Data.Count} productos");
    Console.WriteLine($"Página {resultado.Bag["PaginaActual"]} de {resultado.Bag["TotalPaginas"]}");
    
    foreach (var producto in resultado.Data)
    {
        Console.WriteLine($"{producto.Nombre}: ${producto.Precio}");
    }
    
    if ((bool)resultado.Bag["TieneSiguiente"])
    {
        Console.WriteLine("Hay más productos disponibles");
    }
}
```

#### Caso 2: Búsqueda con Filtros

```csharp
public class ClienteService
{
    public ResultList<Cliente> BuscarClientes(string? termino, string? ciudad, bool? activo)
    {
        var query = _dbContext.Clientes.AsQueryable();
        
        if (!string.IsNullOrEmpty(termino))
        {
            query = query.Where(c => c.Nombre.Contains(termino) || c.Email.Contains(termino));
        }
        
        if (!string.IsNullOrEmpty(ciudad))
        {
            query = query.Where(c => c.Ciudad == ciudad);
        }
        
        if (activo.HasValue)
        {
            query = query.Where(c => c.Activo == activo.Value);
        }
        
        var clientes = query.ToList();
        
        var result = new ResultList<Cliente>
        {
            Value = true,
            Message = clientes.Any() ? "Clientes encontrados" : "No se encontraron clientes",
            Code = 200,
            Data = clientes
        };
        
        result.Bag.Add("TotalEncontrados", clientes.Count);
        result.Bag.Add("FiltrosAplicados", new
        {
            Termino = termino,
            Ciudad = ciudad,
            Activo = activo
        });
        result.Bag.Add("FechaBusqueda", DateTime.Now);
        
        return result;
    }
}

// Uso
var resultado = service.BuscarClientes(termino: "Juan", ciudad: "Madrid", activo: true);

if (resultado.Value)
{
    Console.WriteLine($"Encontrados: {resultado.Bag["TotalEncontrados"]} clientes");
    
    foreach (var cliente in resultado.Data)
    {
        Console.WriteLine($"{cliente.Nombre} - {cliente.Ciudad}");
    }
}
```

#### Caso 3: Export/Reporte de Datos

```csharp
public class ReporteService
{
    public ResultList<VentaResumen> GenerarReporteVentas(DateTime desde, DateTime hasta)
    {
        try
        {
            var ventas = _repository.GetVentas()
                .Where(v => v.Fecha >= desde && v.Fecha <= hasta)
                .Select(v => new VentaResumen
                {
                    Fecha = v.Fecha,
                    Total = v.Total,
                    Cliente = v.Cliente.Nombre,
                    NumeroItems = v.Items.Count
                })
                .OrderByDescending(v => v.Fecha)
                .ToList();
            
            var totalVentas = ventas.Sum(v => v.Total);
            var promedioVenta = ventas.Any() ? ventas.Average(v => v.Total) : 0;
            
            var result = new ResultList<VentaResumen>
            {
                Value = true,
                Message = $"Reporte generado con {ventas.Count} ventas",
                Code = 200,
                Data = ventas
            };
            
            result.Bag.Add("FechaDesde", desde);
            result.Bag.Add("FechaHasta", hasta);
            result.Bag.Add("TotalVentas", totalVentas);
            result.Bag.Add("PromedioVenta", promedioVenta);
            result.Bag.Add("CantidadVentas", ventas.Count);
            result.Bag.Add("FechaGeneracion", DateTime.Now);
            
            return result;
        }
        catch (Exception ex)
        {
            return new ResultList<VentaResumen>
            {
                Value = false,
                Message = $"Error al generar reporte: {ex.Message}",
                Code = 500
            };
        }
    }
}

// Uso
var resultado = service.GenerarReporteVentas(
    desde: new DateTime(2024, 1, 1),
    hasta: DateTime.Now
);

if (resultado.Value)
{
    Console.WriteLine($"Reporte del {resultado.Bag["FechaDesde"]} al {resultado.Bag["FechaHasta"]}");
    Console.WriteLine($"Total vendido: ${resultado.Bag["TotalVentas"]}");
    Console.WriteLine($"Promedio por venta: ${resultado.Bag["PromedioVenta"]:F2}");
    Console.WriteLine($"\nDetalle de ventas:");
    
    foreach (var venta in resultado.Data)
    {
        Console.WriteLine($"{venta.Fecha:dd/MM/yyyy} - {venta.Cliente}: ${venta.Total}");
    }
}
```

#### Caso 4: API Response con Metadata

```csharp
[HttpGet("clientes")]
public IActionResult ObtenerClientes([FromQuery] int pagina = 1, [FromQuery] int tamano = 10)
{
    var resultado = _clienteService.ObtenerClientesPaginados(pagina, tamano);
    
    if (!resultado.Value)
    {
        return StatusCode(resultado.Code, new { mensaje = resultado.Message });
    }
    
    var response = new
    {
        success = true,
        message = resultado.Message,
        data = resultado.Data,
        paginacion = new
        {
            total = resultado.Bag["TotalRegistros"],
            paginaActual = resultado.Bag["PaginaActual"],
            tamanoPagina = resultado.Bag["TamanoPagina"],
            totalPaginas = resultado.Bag["TotalPaginas"],
            tieneAnterior = resultado.Bag["TieneAnterior"],
            tieneSiguiente = resultado.Bag["TieneSiguiente"]
        }
    };
    
    return Ok(response);
}
```

#### Caso 5: Carga de Datos con Estadísticas

```csharp
public class EmpleadoService
{
    public ResultList<Empleado> ObtenerEmpleadosPorDepartamento(string departamento)
    {
        var empleados = _repository.GetAll()
            .Where(e => e.Departamento == departamento)
            .ToList();
        
        if (!empleados.Any())
        {
            return new ResultList<Empleado>
            {
                Value = false,
                Message = $"No se encontraron empleados en el departamento {departamento}",
                Code = 404
            };
        }
        
        var result = new ResultList<Empleado>
        {
            Value = true,
            Message = "Empleados obtenidos correctamente",
            Code = 200,
            Data = empleados
        };
        
        // Estadísticas del departamento
        result.Bag.Add("Departamento", departamento);
        result.Bag.Add("TotalEmpleados", empleados.Count);
        result.Bag.Add("SalarioPromedio", empleados.Average(e => e.Salario));
        result.Bag.Add("SalarioMaximo", empleados.Max(e => e.Salario));
        result.Bag.Add("SalarioMinimo", empleados.Min(e => e.Salario));
        result.Bag.Add("AntiguedadPromedio", empleados.Average(e => 
            (DateTime.Now - e.FechaContratacion).TotalDays / 365));
        
        return result;
    }
}

// Uso
var resultado = service.ObtenerEmpleadosPorDepartamento("Desarrollo");

if (resultado.Value && resultado.Data != null)
{
    Console.WriteLine($"Departamento: {resultado.Bag["Departamento"]}");
    Console.WriteLine($"Total empleados: {resultado.Bag["TotalEmpleados"]}");
    Console.WriteLine($"Salario promedio: ${resultado.Bag["SalarioPromedio"]:F2}");
    Console.WriteLine($"\nLista de empleados:");
    
    foreach (var emp in resultado.Data)
    {
        Console.WriteLine($"- {emp.Nombre}: ${emp.Salario}");
    }
}
```

---

## ResultBagItem

**Namespace:** `HotPack.Classes`  
**Archivo:** [HotPack/Classes/Result.cs](HotPack/Classes/Result.cs)

### Descripción
La clase [`ResultBagItem`](HotPack/Classes/Result.cs) representa un elemento clave-valor que puede ser almacenado en colecciones. Aunque las clases Result usan `Dictionary<string, object>` directamente para el Bag, esta clase proporciona una estructura cuando se necesita trabajar con elementos individuales de forma más explícita.

### Propiedades

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `Key` | `string` | Clave identificadora del elemento |
| `Value` | `object` | Valor asociado a la clave |

### Constructores

```csharp
// Constructor vacío
public ResultBagItem()

// Constructor con clave y valor
public ResultBagItem(string key, object value)
```

### Casos de Uso ResultBagItem

#### Caso 1: Construcción Dinámica de Metadatos

```csharp
using HotPack.Classes;

public class MetadataBuilder
{
    private List<ResultBagItem> _items = new List<ResultBagItem>();
    
    public MetadataBuilder AddItem(string key, object value)
    {
        _items.Add(new ResultBagItem(key, value));
        return this;
    }
    
    public Dictionary<string, object> Build()
    {
        var dictionary = new Dictionary<string, object>();
        foreach (var item in _items)
        {
            dictionary[item.Key] = item.Value;
        }
        return dictionary;
    }
}

// Uso
var metadata = new MetadataBuilder()
    .AddItem("Usuario", "admin")
    .AddItem("Timestamp", DateTime.Now)
    .AddItem("IP", "192.168.1.1")
    .AddItem("Version", "1.0.0")
    .Build();

var result = new Result(true, "Operación exitosa");
foreach (var kvp in metadata)
{
    result.Bag[kvp.Key] = kvp.Value;
}
```

#### Caso 2: Serialización de Metadatos

```csharp
public class AuditoriaService
{
    public void RegistrarAuditoria(Result result)
    {
        var items = new List<ResultBagItem>();
        
        foreach (var kvp in result.Bag)
        {
            items.Add(new ResultBagItem(kvp.Key, kvp.Value));
        }
        
        var auditoria = new Auditoria
        {
            Fecha = DateTime.Now,
            Resultado = result.Value,
            Mensaje = result.Message,
            Metadatos = JsonSerializer.Serialize(items)
        };
        
        _repository.Save(auditoria);
    }
}
```

#### Caso 3: Validación de Metadatos Requeridos

```csharp
public class MetadataValidator
{
    private readonly List<string> _requiredKeys;
    
    public MetadataValidator(params string[] requiredKeys)
    {
        _requiredKeys = requiredKeys.ToList();
    }
    
    public Result ValidarMetadatos(Dictionary<string, object> bag)
    {
        var faltantes = new List<string>();
        
        foreach (var key in _requiredKeys)
        {
            if (!bag.ContainsKey(key))
            {
                faltantes.Add(key);
            }
        }
        
        if (faltantes.Any())
        {
            return new Result(false, $"Faltan metadatos requeridos: {string.Join(", ", faltantes)}");
        }
        
        return new Result(true, "Todos los metadatos requeridos están presentes");
    }
}

// Uso
var validator = new MetadataValidator("UserId", "Timestamp", "Action");
var resultado = validator.ValidarMetadatos(result.Bag);

if (!resultado.Value)
{
    Console.WriteLine(resultado.Message);
}
```

---

## Conexion

**Namespace:** `HotPack.Database`  
**Archivo:** [HotPack.Database/Conexion.cs](HotPack.Database/Conexion.cs)

### Descripción
La clase [`Conexion`](HotPack.Database/Conexion.cs) es una capa de acceso a datos optimizada para SQL Server que proporciona métodos asíncronos para ejecutar consultas, procedimientos almacenados y mapear resultados a objetos .NET. Se integra perfectamente con la clase [`Result`](HotPack/Classes/Result.cs) para proporcionar respuestas consistentes.

### Constructor Conexion

```csharp
public Conexion(string connectionString)
```

**Parámetros:**
- `connectionString` (string): Cadena de conexión a la base de datos SQL Server

**Ejemplo:**
```csharp
var connectionString = "Server=localhost;Database=MiDB;Integrated Security=true;";
var conexion = new Conexion(connectionString);
```

### Métodos Principales

#### `ExecuteAsync`
Ejecuta un comando SQL que no devuelve resultados (INSERT, UPDATE, DELETE).

**Firma:**
```csharp
public async Task<Result> ExecuteAsync(
    string query, 
    ConexionParameters? parameters = null, 
    CommandType? commandType = CommandType.StoredProcedure, 
    int? commandTimeout = 30)
```

**Parámetros:**
- `query` (string): Consulta SQL o nombre del procedimiento almacenado
- `parameters` (ConexionParameters?): Parámetros de la consulta (opcional)
- `commandType` (CommandType?): Tipo de comando (default: StoredProcedure)
- `commandTimeout` (int?): Timeout en segundos (default: 30)

**Retorna:** [`Result`](HotPack/Classes/Result.cs) con información de la ejecución

**Características:**
- Mapea automáticamente parámetros de salida a `Result.Value`, `Result.Message` y `Result.Code`
- Agrega parámetros de salida adicionales al `Result.Bag`

---

#### `ExecuteWithResultsAsync<T>`
Ejecuta una consulta y mapea los resultados a una lista de objetos del tipo especificado.

**Firma:**
```csharp
public async Task<ResultList<T>> ExecuteWithResultsAsync<T>(
    string query, 
    ConexionParameters? parameters = null, 
    CommandType? commandType = CommandType.StoredProcedure, 
    int? commandTimeout = 30)
```

**Parámetros:**
- `query` (string): Consulta SQL o nombre del procedimiento
- `parameters` (ConexionParameters?): Parámetros opcionales
- `commandType` (CommandType?): Tipo de comando
- `commandTimeout` (int?): Timeout en segundos

**Retorna:** [`ResultList<T>`](HotPack/Classes/Result.cs) con los datos mapeados

**Características:**
- Mapea automáticamente columnas a propiedades usando reflexión
- Soporta tipos primitivos y objetos complejos
- Respeta el atributo `ConexionColumnAttribute` para mapeo personalizado

---

#### `ExecuteToObjectAsync<T>`
Ejecuta una consulta esperando un único objeto como resultado (equivalente a FirstOrDefault).

**Firma:**
```csharp
public async Task<Result<T?>> ExecuteToObjectAsync<T>(
    string query, 
    ConexionParameters? parameters, 
    CommandType? commandType = CommandType.StoredProcedure, 
    int? commandTimeout = 30)
```

**Retorna:** [`Result<T?>`](HotPack/Classes/Result.cs) con el objeto mapeado o null

---

#### `ExecuteScalarAsync<T>`
Ejecuta una consulta que devuelve un valor escalar (COUNT, SUM, MAX, etc.).

**Firma:**
```csharp
public async Task<Result<T>> ExecuteScalarAsync<T>(
    string query, 
    ConexionParameters? parameters = null, 
    CommandType? commandType = CommandType.StoredProcedure, 
    int? commandTimeout = 30)
```

**Retorna:** [`Result<T>`](HotPack/Classes/Result.cs) con el valor escalar

---

#### `ExecuteWithMultipleResultsAsync`
Ejecuta una consulta que devuelve múltiples conjuntos de resultados.

**Firma:**
```csharp
public async Task<Result> ExecuteWithMultipleResultsAsync(
    string query, 
    ConexionParameters? parameters, 
    Func<ConexionMultipleReader, Task> readerFunc, 
    CommandType? commandType = CommandType.StoredProcedure, 
    int? commandTimeout = 30)
```

**Parámetros:**
- `readerFunc` (Func<ConexionMultipleReader, Task>): Función que procesa múltiples resultados

**Retorna:** [`Result`](HotPack/Classes/Result.cs)

---

#### `ExecuteWithMultipleResultsAsync<T>`
Versión genérica que devuelve datos del procesamiento de múltiples resultados.

**Firma:**
```csharp
public async Task<Result<T>> ExecuteWithMultipleResultsAsync<T>(
    string query, 
    ConexionParameters? parameters, 
    Func<ConexionMultipleReader, Task<T>> readerFunc, 
    CommandType? commandType = CommandType.StoredProcedure, 
    int? commandTimeout = 30)
```

**Retorna:** [`Result<T>`](HotPack/Classes/Result.cs) con los datos procesados

---

### Mapeo Automático de Parámetros

La clase [`Conexion`](HotPack.Database/Conexion.cs) mapea automáticamente parámetros de salida con nombres específicos:

| Nombres Reconocidos | Propiedad de Result |
|---------------------|---------------------|
| `result`, `presult`, `resultado`, `presultado` | `Result.Value` |
| `msg`, `pmsg`, `message`, `pmessage`, `mensaje`, `pmensaje` | `Result.Message` |
| `code`, `pcode`, `codigo`, `pcodigo` | `Result.Code` |

Los demás parámetros de salida se agregan automáticamente al `Result.Bag`.

### Casos de Uso Conexion

#### Caso 1: CRUD Básico - Insertar Usuario

```csharp
using HotPack.Database;
using HotPack.Classes;

public class UsuarioRepository
{
    private readonly Conexion _conexion;
    
    public UsuarioRepository(string connectionString)
    {
        _conexion = new Conexion(connectionString);
    }
    
    public async Task<Result> InsertarUsuarioAsync(string nombre, string email, string password)
    {
        var parametros = new ConexionParameters()
            .Add("Nombre", nombre)
            .Add("Email", email)
            .Add("Password", password)
            .AddOutput("UsuarioId", DbType.Int32)
            .AddOutput("Result", DbType.Boolean)
            .AddOutput("Message", DbType.String, 500);
        
        var resultado = await _conexion.ExecuteAsync("sp_InsertarUsuario", parametros);
        
        if (resultado.Value && resultado.Bag.ContainsKey("UsuarioId"))
        {
            Console.WriteLine($"Usuario creado con ID: {resultado.Bag["UsuarioId"]}");
        }
        
        return resultado;
    }
}

// Uso
var repo = new UsuarioRepository(connectionString);
var resultado = await repo.InsertarUsuarioAsync("Juan Pérez", "juan@email.com", "pass123");

if (resultado.Value)
{
    Console.WriteLine($"Éxito: {resultado.Message}");
    Console.WriteLine($"ID generado: {resultado.Bag["UsuarioId"]}");
}
else
{
    Console.WriteLine($"Error: {resultado.Message}");
}
```

**Procedimiento almacenado:**
```sql
CREATE PROCEDURE sp_InsertarUsuario
    @Nombre VARCHAR(100),
    @Email VARCHAR(100),
    @Password VARCHAR(100),
    @UsuarioId INT OUTPUT,
    @Result BIT OUTPUT,
    @Message VARCHAR(500) OUTPUT
AS
BEGIN
    BEGIN TRY
        IF EXISTS(SELECT 1 FROM Usuarios WHERE Email = @Email)
        BEGIN
            SET @Result = 0
            SET @Message = 'El email ya está registrado'
            RETURN
        END
        
        INSERT INTO Usuarios (Nombre, Email, Password, FechaCreacion)
        VALUES (@Nombre, @Email, @Password, GETDATE())
        
        SET @UsuarioId = SCOPE_IDENTITY()
        SET @Result = 1
        SET @Message = 'Usuario creado exitosamente'
    END TRY
    BEGIN CATCH
        SET @Result = 0
        SET @Message = ERROR_MESSAGE()
    END CATCH
END
```

---

#### Caso 2: Obtener Lista de Productos

```csharp
public class Producto
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string Categoria { get; set; }
}

public class ProductoRepository
{
    private readonly Conexion _conexion;
    
    public ProductoRepository(string connectionString)
    {
        _conexion = new Conexion(connectionString);
    }
    
    public async Task<ResultList<Producto>> ObtenerProductosAsync(string? categoria = null)
    {
        var parametros = new ConexionParameters();
        
        if (!string.IsNullOrEmpty(categoria))
        {
            parametros.Add("Categoria", categoria);
        }
        
        var resultado = await _conexion.ExecuteWithResultsAsync<Producto>(
            "sp_ObtenerProductos", 
            parametros
        );
        
        return resultado;
    }
    
    public async Task<ResultList<Producto>> ObtenerProductosPaginadosAsync(
        int pagina, 
        int tamanoPagina)
    {
        var parametros = new ConexionParameters()
            .Add("Pagina", pagina)
            .Add("TamanoPagina", tamanoPagina)
            .AddOutput("TotalRegistros", DbType.Int32, alias: "TotalRegistros")
            .AddOutput("TotalPaginas", DbType.Int32, alias: "TotalPaginas");
        
        var resultado = await _conexion.ExecuteWithResultsAsync<Producto>(
            "sp_ObtenerProductosPaginados", 
            parametros
        );
        
        return resultado;
    }
}

// Uso
var repo = new ProductoRepository(connectionString);

// Sin filtros
var todos = await repo.ObtenerProductosAsync();
if (todos.Value && todos.Data != null)
{
    Console.WriteLine($"Total productos: {todos.Data.Count}");
    foreach (var p in todos.Data)
    {
        Console.WriteLine($"{p.Nombre}: ${p.Precio} (Stock: {p.Stock})");
    }
}

// Con filtro
var electronicos = await repo.ObtenerProductosAsync("Electrónica");

// Paginados
var paginados = await repo.ObtenerProductosPaginadosAsync(pagina: 1, tamanoPagina: 20);
if (paginados.Value)
{
    Console.WriteLine($"Mostrando {paginados.Data.Count} de {paginados.Bag["TotalRegistros"]}");
    Console.WriteLine($"Página 1 de {paginados.Bag["TotalPaginas"]}");
}
```

**Procedimiento almacenado:**
```sql
CREATE PROCEDURE sp_ObtenerProductosPaginados
    @Pagina INT,
    @TamanoPagina INT,
    @TotalRegistros INT OUTPUT,
    @TotalPaginas INT OUTPUT
AS
BEGIN
    SELECT @TotalRegistros = COUNT(*) FROM Productos
    SET @TotalPaginas = CEILING(CAST(@TotalRegistros AS FLOAT) / @TamanoPagina)
    
    SELECT ProductoId, Nombre, Precio, Stock, Categoria
    FROM Productos
    ORDER BY Nombre
    OFFSET (@Pagina - 1) * @TamanoPagina ROWS
    FETCH NEXT @TamanoPagina ROWS ONLY
END
```

---

#### Caso 3: Obtener Un Solo Objeto

```csharp
public class Cliente
{
    public int ClienteId { get; set; }
    public string NombreCompleto { get; set; }
    public string Email { get; set; }
    public string Telefono { get; set; }
    public DateTime FechaRegistro { get; set; }
}

public class ClienteRepository
{
    private readonly Conexion _conexion;
    
    public ClienteRepository(string connectionString)
    {
        _conexion = new Conexion(connectionString);
    }
    
    public async Task<Result<Cliente?>> ObtenerPorIdAsync(int clienteId)
    {
        var parametros = new ConexionParameters()
            .Add("ClienteId", clienteId)
            .AddOutput("Result", DbType.Boolean)
            .AddOutput("Message", DbType.String, 500);
        
        var resultado = await _conexion.ExecuteToObjectAsync<Cliente>(
            "sp_ObtenerClientePorId", 
            parametros
        );
        
        return resultado;
    }
    
    public async Task<Result<Cliente?>> ObtenerPorEmailAsync(string email)
    {
        var parametros = new ConexionParameters()
            .Add("Email", email);
        
        return await _conexion.ExecuteToObjectAsync<Cliente>(
            "sp_ObtenerClientePorEmail", 
            parametros
        );
    }
}

// Uso
var repo = new ClienteRepository(connectionString);

var resultado = await repo.ObtenerPorIdAsync(123);

if (resultado.Value && resultado.Data != null)
{
    var cliente = resultado.Data;
    Console.WriteLine($"Cliente: {cliente.NombreCompleto}");
    Console.WriteLine($"Email: {cliente.Email}");
    Console.WriteLine($"Registrado: {cliente.FechaRegistro:dd/MM/yyyy}");
}
else if (resultado.Value && resultado.Data == null)
{
    Console.WriteLine("Cliente no encontrado");
}
else
{
    Console.WriteLine($"Error: {resultado.Message}");
}
```

---

#### Caso 4: ExecuteScalar - Obtener Totales y Conteos

```csharp
public class EstadisticasService
{
    private readonly Conexion _conexion;
    
    public EstadisticasService(string connectionString)
    {
        _conexion = new Conexion(connectionString);
    }
    
    public async Task<Result<int>> ContarClientesActivosAsync()
    {
        return await _conexion.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM Clientes WHERE Activo = 1",
            commandType: CommandType.Text
        );
    }
    
    public async Task<Result<decimal>> ObtenerTotalVentasDelMesAsync()
    {
        var parametros = new ConexionParameters()
            .Add("Mes", DateTime.Now.Month)
            .Add("Anio", DateTime.Now.Year);
        
        return await _conexion.ExecuteScalarAsync<decimal>(
            "sp_ObtenerTotalVentasMes",
            parametros
        );
    }
    
    public async Task<Result<DateTime>> ObtenerUltimaActualizacionAsync()
    {
        return await _conexion.ExecuteScalarAsync<DateTime>(
            "SELECT MAX(FechaActualizacion) FROM Productos",
            commandType: CommandType.Text
        );
    }
}

// Uso
var service = new EstadisticasService(connectionString);

var totalClientes = await service.ContarClientesActivosAsync();
if (totalClientes.Value)
{
    Console.WriteLine($"Clientes activos: {totalClientes.Data}");
}

var ventasMes = await service.ObtenerTotalVentasDelMesAsync();
if (ventasMes.Value)
{
    Console.WriteLine($"Ventas del mes: ${ventasMes.Data:N2}");
}

var ultimaActualizacion = await service.ObtenerUltimaActualizacionAsync();
if (ultimaActualizacion.Value)
{
    Console.WriteLine($"Última actualización: {ultimaActualizacion.Data:dd/MM/yyyy HH:mm}");
}
```

---

#### Caso 5: Múltiples Resultados - Dashboard Completo

```csharp
public class DashboardData
{
    public List<VentaReciente> VentasRecientes { get; set; }
    public List<ProductoMasVendido> ProductosTop { get; set; }
    public List<ClienteTop> ClientesTop { get; set; }
    public EstadisticasGenerales Estadisticas { get; set; }
}

public class DashboardService
{
    private readonly Conexion _conexion;
    
    public DashboardService(string connectionString)
    {
        _conexion = new Conexion(connectionString);
    }
    
    public async Task<Result<DashboardData>> ObtenerDashboardAsync()
    {
        var parametros = new ConexionParameters()
            .Add("Dias", 30);
        
        var resultado = await _conexion.ExecuteWithMultipleResultsAsync<DashboardData>(
            "sp_ObtenerDatosDashboard",
            parametros,
            async (reader) =>
            {
                var dashboard = new DashboardData();
                
                // Primer conjunto de resultados: Ventas recientes
                dashboard.VentasRecientes = await reader.ReadAsync<VentaReciente>();
                
                // Segundo conjunto: Productos más vendidos
                dashboard.ProductosTop = await reader.ReadAsync<ProductoMasVendido>();
                
                // Tercer conjunto: Mejores clientes
                dashboard.ClientesTop = await reader.ReadAsync<ClienteTop>();
                
                // Cuarto conjunto: Estadísticas generales
                var estadisticas = await reader.ReadAsync<EstadisticasGenerales>();
                dashboard.Estadisticas = estadisticas.FirstOrDefault();
                
                return dashboard;
            }
        );
        
        return resultado;
    }
}

// Uso
var service = new DashboardService(connectionString);
var resultado = await service.ObtenerDashboardAsync();

if (resultado.Value && resultado.Data != null)
{
    var dashboard = resultado.Data;
    
    Console.WriteLine("=== DASHBOARD ===");
    Console.WriteLine($"\nEstadísticas Generales:");
    Console.WriteLine($"Total Ventas: ${dashboard.Estadisticas.TotalVentas:N2}");
    Console.WriteLine($"Promedio Venta: ${dashboard.Estadisticas.PromedioVenta:N2}");
    
    Console.WriteLine($"\nVentas Recientes ({dashboard.VentasRecientes.Count}):");
    foreach (var venta in dashboard.VentasRecientes.Take(5))
    {
        Console.WriteLine($"  {venta.Fecha:dd/MM} - {venta.Cliente}: ${venta.Total}");
    }
    
    Console.WriteLine($"\nProductos Más Vendidos:");
    foreach (var producto in dashboard.ProductosTop.Take(5))
    {
        Console.WriteLine($"  {producto.Nombre}: {producto.Unidades} unidades");
    }
}
```

**Procedimiento almacenado:**
```sql
CREATE PROCEDURE sp_ObtenerDatosDashboard
    @Dias INT
AS
BEGIN
    DECLARE @FechaDesde DATE = DATEADD(DAY, -@Dias, GETDATE())
    
    -- Primer resultado: Ventas recientes
    SELECT TOP 10
        v.VentaId,
        v.Fecha,
        c.NombreCompleto AS Cliente,
        v.Total
    FROM Ventas v
    INNER JOIN Clientes c ON v.ClienteId = c.ClienteId
    WHERE v.Fecha >= @FechaDesde
    ORDER BY v.Fecha DESC
    
    -- Segundo resultado: Productos más vendidos
    SELECT TOP 5
        p.Nombre,
        SUM(vi.Cantidad) AS Unidades,
        SUM(vi.Subtotal) AS TotalVendido
    FROM VentaItems vi
    INNER JOIN Productos p ON vi.ProductoId = p.ProductoId
    INNER JOIN Ventas v ON vi.VentaId = v.VentaId
    WHERE v.Fecha >= @FechaDesde
    GROUP BY p.ProductoId, p.Nombre
    ORDER BY SUM(vi.Cantidad) DESC
    
    -- Tercer resultado: Mejores clientes
    SELECT TOP 5
        c.NombreCompleto,
        COUNT(v.VentaId) AS NumeroCompras,
        SUM(v.Total) AS TotalGastado
    FROM Clientes c
    INNER JOIN Ventas v ON c.ClienteId = v.ClienteId
    WHERE v.Fecha >= @FechaDesde
    GROUP BY c.ClienteId, c.NombreCompleto
    ORDER BY SUM(v.Total) DESC
    
    -- Cuarto resultado: Estadísticas generales
    SELECT 
        COUNT(*) AS TotalVentas,
        SUM(Total) AS TotalVentas,
        AVG(Total) AS PromedioVenta,
        MAX(Total) AS VentaMaxima,
        MIN(Total) AS VentaMinima
    FROM Ventas
    WHERE Fecha >= @FechaDesde
END
```

---

#### Caso 6: Mapeo con Atributos Personalizados

```csharp
public class ProductoDto
{
    [ConexionColumn("producto_id")]
    public int Id { get; set; }
    
    [ConexionColumn("nombre_producto")]
    public string Nombre { get; set; }
    
    [ConexionColumn("precio_unitario")]
    public decimal Precio { get; set; }
    
    [ConexionColumn("cantidad_stock")]
    public int Stock { get; set; }
    
    [ConexionColumn("categoria_nombre")]
    public string Categoria { get; set; }
    
    [ConexionColumn("fecha_creacion")]
    public DateTime FechaCreacion { get; set; }
}

public class ProductoRepository
{
    private readonly Conexion _conexion;
    
    public ProductoRepository(string connectionString)
    {
        _conexion = new Conexion(connectionString);
    }
    
    public async Task<ResultList<ProductoDto>> ObtenerProductosConAtributosAsync()
    {
        // El mapeo se hará automáticamente usando los atributos ConexionColumn
        return await _conexion.ExecuteWithResultsAsync<ProductoDto>(
            "sp_ObtenerProductosConNombresPersonalizados"
        );
    }
}

// Uso
var repo = new ProductoRepository(connectionString);
var resultado = await repo.ObtenerProductosConAtributosAsync();

if (resultado.Value && resultado.Data != null)
{
    foreach (var producto in resultado.Data)
    {
        Console.WriteLine($"ID: {producto.Id}");
        Console.WriteLine($"Nombre: {producto.Nombre}");
        Console.WriteLine($"Precio: ${producto.Precio}");
        Console.WriteLine($"Stock: {producto.Stock}");
        Console.WriteLine($"Categoría: {producto.Categoria}");
        Console.WriteLine($"Creado: {producto.FechaCreacion:dd/MM/yyyy}");
        Console.WriteLine("---");
    }
}
```

---

#### Caso 7: Transacciones y Operaciones Complejas

```csharp
public class PedidoService
{
    private readonly Conexion _conexion;
    
    public PedidoService(string connectionString)
    {
        _conexion = new Conexion(connectionString);
    }
    
    public async Task<Result> ProcesarPedidoCompletoAsync(
        int clienteId, 
        List<ItemPedido> items)
    {
        var parametros = new ConexionParameters()
            .Add("ClienteId", clienteId)
            .Add("Items", JsonSerializer.Serialize(items))
            .AddOutput("PedidoId", DbType.Int32, alias: "PedidoId")
            .AddOutput("Total", DbType.Decimal, alias: "TotalPedido")
            .AddOutput("Result", DbType.Boolean)
            .AddOutput("Message", DbType.String, 500)
            .AddOutput("Code", DbType.Int32);
        
        var resultado = await _conexion.ExecuteAsync(
            "sp_ProcesarPedidoCompleto", 
            parametros
        );
        
        if (resultado.Value)
        {
            Console.WriteLine($"Pedido creado: #{resultado.Bag["PedidoId"]}");
            Console.WriteLine($"Total: ${resultado.Bag["TotalPedido"]}");
        }
        
        return resultado;
    }
}

// Uso
var service = new PedidoService(connectionString);
var items = new List<ItemPedido>
{
    new ItemPedido { ProductoId = 1, Cantidad = 2, Precio = 99.99m },
    new ItemPedido { ProductoId = 5, Cantidad = 1, Precio = 49.99m }
};

var resultado = await service.ProcesarPedidoCompletoAsync(clienteId: 10, items);

if (resultado.Value)
{
    Console.WriteLine($"✓ {resultado.Message}");
    Console.WriteLine($"Pedido: {resultado.Bag["PedidoId"]}");
    Console.WriteLine($"Total: ${resultado.Bag["TotalPedido"]:N2}");
}
else
{
    Console.WriteLine($"✗ Error ({resultado.Code}): {resultado.Message}");
}
```

---

## Clases Auxiliares

### ConexionParameters

**Namespace:** `HotPack.Database`  
**Archivo:** [HotPack.Database/ConexionParameters.cs](HotPack.Database/ConexionParameters.cs)

#### Descripción
Clase helper para construir parámetros de SQL de forma fluida usando el patrón builder.

#### Métodos Principales

##### `Add(string name, object? value)`
Agrega un parámetro de entrada.

**Ejemplo:**
```csharp
var parametros = new ConexionParameters()
    .Add("UsuarioId", 123)
    .Add("Nombre", "Juan")
    .Add("Activo", true);
```

##### `AddOutput(string name, DbType type, int? size = null, string? alias = null)`
Agrega un parámetro de salida.

**Ejemplo:**
```csharp
var parametros = new ConexionParameters()
    .Add("UsuarioId", 100)
    .AddOutput("Mensaje", DbType.String, 500)
    .AddOutput("NuevoId", DbType.Int32);
```

---

### ConexionCastableRow

**Namespace:** `HotPack.Database`  
**Archivo:** [HotPack.Database/ConexionCastableRow.cs](HotPack.Database/ConexionCastableRow.cs)

#### Descripción
Permite acceder a las columnas de una fila de base de datos usando la clase `Castable` para conversiones seguras.

#### Ejemplo de Uso

```csharp
public class ManualMapping
{
    private readonly Conexion _conexion;
    
    public async Task<List<Usuario>> ObtenerUsuariosManualAsync()
    {
        var resultado = await _conexion.ExecuteAsync("sp_ObtenerUsuarios");
        var usuarios = new List<Usuario>();
        
        // Acceso manual a las filas si es necesario
        // ConexionCastableRow permite conversiones tipo-seguras de columnas
        
        return usuarios;
    }
}
```

---

### ConexionMultipleReader

**Namespace:** `HotPack.Database`  
**Archivo:** [HotPack.Database/ConexionMultipleReader.cs](HotPack.Database/ConexionMultipleReader.cs)

#### Descripción
Permite leer múltiples conjuntos de resultados de un procedimiento almacenado de forma secuencial.

#### Métodos

##### `ReadAsync<T>()`
Lee el siguiente conjunto de resultados y lo mapea a una lista del tipo especificado.

**Retorna:** `Task<List<T>>`

#### Ejemplo de Uso

```csharp
var resultado = await _conexion.ExecuteWithMultipleResultsAsync<DatosCompletos>(
    "sp_ObtenerDatosMultiples",
    null,
    async (reader) =>
    {
        var datos = new DatosCompletos();
        
        // Leer primer conjunto de resultados
        datos.Clientes = await reader.ReadAsync<Cliente>();
        
        // Leer segundo conjunto de resultados
        datos.Productos = await reader.ReadAsync<Producto>();
        
        // Leer tercer conjunto de resultados
        datos.Ventas = await reader.ReadAsync<Venta>();
        
        return datos;
    }
);
```

---

### ConexionColumnAttribute

**Namespace:** `HotPack.Database`  
**Archivo:** [HotPack.Database/ConexionColumnAttribute.cs](HotPack.Database/ConexionColumnAttribute.cs)

#### Descripción
Atributo que permite especificar un nombre de columna personalizado para el mapeo entre la base de datos y las propiedades de C#.

#### Constructor

```csharp
[AttributeUsage(AttributeTargets.Property)]
public class ConexionColumnAttribute : Attribute
{
    public ConexionColumnAttribute(string columnName)
}
```

#### Ejemplo de Uso

```csharp
public class ProductoEntity
{
    // La columna en la BD se llama "producto_id"
    [ConexionColumn("producto_id")]
    public int Id { get; set; }
    
    // La columna en la BD se llama "nombre_producto"
    [ConexionColumn("nombre_producto")]
    public string Nombre { get; set; }
    
    // La columna en la BD se llama "precio_unitario"
    [ConexionColumn("precio_unitario")]
    public decimal Precio { get; set; }
    
    // Sin atributo, busca columna "Stock" exactamente
    public int Stock { get; set; }
}

// Uso
var productos = await _conexion.ExecuteWithResultsAsync<ProductoEntity>("sp_ObtenerProductos");
// El mapeo se hace automáticamente usando los atributos
```

---

## Castable

**Namespace:** `HotPack.Classes`  
**Archivo:** [HotPack/Classes/Castable.cs](HotPack/Classes/Castable.cs)

### Descripción
La clase `Castable` encapsula un objeto y proporciona métodos de conversión seguros a diferentes tipos de datos. Ofrece dos enfoques: conversiones que lanzan excepciones si fallan y conversiones con callbacks de error personalizables.

### Constructor

```csharp
public Castable(object obj)
```

**Parámetros:**
- `obj` (object): El objeto a encapsular y convertir

### Métodos de Conversión

#### Conversiones Seguras (Lanzan Excepción)

Estos métodos intentan convertir el valor encapsulado al tipo especificado. Si la conversión falla, lanzan una excepción con mensaje descriptivo.

| Método | Tipo de Retorno | Descripción |
|--------|----------------|-------------|
| `ToInt32()` | `int` | Convierte a entero de 32 bits |
| `ToUint32()` | `uint` | Convierte a entero sin signo de 32 bits |
| `ToInt64()` | `long` | Convierte a entero de 64 bits |
| `ToUInt64()` | `ulong` | Convierte a entero sin signo de 64 bits |
| `ToDecimal()` | `decimal` | Convierte a decimal |
| `ToSingle()` | `float` | Convierte a float |
| `ToDouble()` | `double` | Convierte a double |
| `ToBoolean()` | `bool` | Convierte a booleano |
| `ToByte()` | `byte` | Convierte a byte |
| `ToBytes()` | `byte[]` | Convierte a array de bytes |
| `ToDateTime()` | `DateTime` | Convierte a DateTime |
| `ToString()` | `string?` | Convierte a string |

#### Conversiones con Callback de Error

Estos métodos intentan convertir el valor y si fallan, ejecutan una función callback personalizada para manejar el error.

| Método | Firma | Descripción |
|--------|-------|-------------|
| `TryInt32()` | `int TryInt32(Func<Castable, int> onError)` | Intenta conversión a int |
| `TryUint32()` | `uint TryUint32(Func<Castable, uint> onError)` | Intenta conversión a uint |
| `TryInt64()` | `long TryInt64(Func<Castable, long> onError)` | Intenta conversión a long |
| `TryUInt64()` | `ulong TryUInt64(Func<Castable, ulong> onError)` | Intenta conversión a ulong |
| `TryDecimal()` | `decimal TryDecimal(Func<Castable, decimal> onError)` | Intenta conversión a decimal |
| `TrySingle()` | `float TrySingle(Func<Castable, float> onError)` | Intenta conversión a float |
| `TryDouble()` | `double TryDouble(Func<Castable, double> onError)` | Intenta conversión a double |
| `TryBoolean()` | `bool TryBoolean(Func<Castable, bool> onError)` | Intenta conversión a bool |
| `TryByte()` | `byte TryByte(Func<Castable, byte> onError)` | Intenta conversión a byte |
| `TryBytes()` | `byte[] TryBytes(Func<Castable, byte[]> onError)` | Intenta conversión a byte[] |
| `TryDateTime()` | `DateTime TryDateTime(Func<Castable, DateTime> onError)` | Intenta conversión a DateTime |
| `TryString()` | `string? TryString(Func<Castable, string> onError)` | Intenta conversión a string |

### Casos de Uso Castable

#### Caso 1: Conversión Básica con Excepciones

```csharp
using HotPack.Classes;

// Conversión exitosa
var castable1 = new Castable("123");
int numero = castable1.ToInt32(); // 123

// Conversión de decimal
var castable2 = new Castable("99.99");
decimal precio = castable2.ToDecimal(); // 99.99

// Conversión de fecha
var castable3 = new Castable("2024-12-01");
DateTime fecha = castable3.ToDateTime(); // 2024-12-01 00:00:00

// Conversión que falla (lanza excepción)
try
{
    var castableInvalido = new Castable("abc");
    int numeroInvalido = castableInvalido.ToInt32(); // Lanza excepción
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message); // "Unable to convert the value "abc" to the type "Int32""
}
```

#### Caso 2: Conversión con Manejo de Errores

```csharp
// Usar valor por defecto si falla
var castable = new Castable("abc");
int numero = castable.TryInt32(c => 0); // Retorna 0 porque "abc" no es numérico

// Logging de errores
var castable2 = new Castable("xyz");
decimal precio = castable2.TryDecimal(c => 
{
    Console.WriteLine($"Error convirtiendo '{c}' a decimal, usando valor por defecto");
    return -1m;
});

// Múltiples intentos de conversión
var castable3 = new Castable("no-es-numero");
int valor = castable3.TryInt32(c => 
{
    // Intentar con valor por defecto de configuración
    return ConfiguracionGlobal.ValorPorDefecto;
});
```

#### Caso 3: Uso con AppConfiguration

```csharp
using HotPack.App;
using HotPack.Classes;

var config = new AppConfiguration();

// Los parámetros se devuelven como Castable
Castable timeoutCastable = config.TryParameter("Timeout", false, () => new Castable("30"));

// Convertir a entero con valor por defecto
int timeout = timeoutCastable.TryInt32(c => 30);

// Convertir a diferentes tipos
Castable maxRetries = config.TryParameter("MaxRetries", false, () => new Castable("3"));
int retries = maxRetries.ToInt32();

Castable enableCache = config.TryParameter("EnableCache", false, () => new Castable("true"));
bool cacheEnabled = enableCache.ToBoolean();

Castable apiUrl = config.TryParameter("ApiUrl", false, () => new Castable("https://api.ejemplo.com"));
string url = apiUrl.ToString();
```

#### Caso 4: Procesamiento de Datos de Base de Datos

```csharp
public class DataProcessor
{
    public void ProcesarFilas(DataTable tabla)
    {
        foreach (DataRow fila in tabla.Rows)
        {
            // Usar Castable para conversiones seguras desde DataRow
            var id = new Castable(fila["Id"]).TryInt32(c => 0);
            var nombre = new Castable(fila["Nombre"]).TryString(c => "Sin nombre");
            var precio = new Castable(fila["Precio"]).TryDecimal(c => 0m);
            var activo = new Castable(fila["Activo"]).TryBoolean(c => false);
            var fechaCreacion = new Castable(fila["FechaCreacion"]).TryDateTime(c => DateTime.MinValue);
            
            Console.WriteLine($"Producto {id}: {nombre} - ${precio}");
        }
    }
}
```

#### Caso 5: Validación y Conversión de Entrada de Usuario

```csharp
public class FormularioService
{
    public Result ValidarYGuardar(Dictionary<string, string> formulario)
    {
        try
        {
            // Convertir y validar edad
            var edadCastable = new Castable(formulario["edad"]);
            int edad = edadCastable.TryInt32(c => 
            {
                return -1; // Indica error
            });
            
            if (edad < 0 || edad > 120)
            {
                return new Result(false, "Edad inválida");
            }
            
            // Convertir salario
            var salarioCastable = new Castable(formulario["salario"]);
            decimal salario = salarioCastable.TryDecimal(c => 
            {
                throw new Exception("El salario debe ser un número válido");
            });
            
            // Convertir fecha de nacimiento
            var fechaNacCastable = new Castable(formulario["fechaNacimiento"]);
            DateTime fechaNac = fechaNacCastable.TryDateTime(c => DateTime.MinValue);
            
            if (fechaNac == DateTime.MinValue)
            {
                return new Result(false, "Fecha de nacimiento inválida");
            }
            
            // Guardar en base de datos...
            return new Result(true, "Datos guardados correctamente");
        }
        catch (Exception ex)
        {
            return new Result(ex);
        }
    }
}
```

#### Caso 6: Trabajo con JSON Dinámico

```csharp
using System.Text.Json;

public class JsonProcessor
{
    public void ProcesarJson(string jsonString)
    {
        var jsonDoc = JsonDocument.Parse(jsonString);
        var root = jsonDoc.RootElement;
        
        // Usar Castable para convertir valores JSON
        var idCastable = new Castable(root.GetProperty("id").GetRawText());
        int id = idCastable.ToInt32();
        
        var precioCastable = new Castable(root.GetProperty("precio").GetRawText());
        decimal precio = precioCastable.TryDecimal(c => 0m);
        
        var activoCastable = new Castable(root.GetProperty("activo").GetRawText());
        bool activo = activoCastable.TryBoolean(c => false);
        
        Console.WriteLine($"ID: {id}, Precio: {precio}, Activo: {activo}");
    }
}
```

#### Caso 7: Conversión de Bytes

```csharp
public class FileService
{
    public void ProcesarArchivo(object archivoData)
    {
        // Convertir a bytes
        var castable = new Castable(archivoData);
        byte[] bytes = castable.TryBytes(c => new byte[0]);
        
        if (bytes.Length > 0)
        {
            // Procesar el archivo
            File.WriteAllBytes("output.dat", bytes);
            Console.WriteLine($"Archivo guardado: {bytes.Length} bytes");
        }
        else
        {
            Console.WriteLine("No se pudieron obtener los bytes del archivo");
        }
    }
}
```

#### Caso 8: Sistema de Configuración Tipado

```csharp
public class AppSettings
{
    private readonly AppConfiguration _config;
    
    public AppSettings(AppConfiguration config)
    {
        _config = config;
    }
    
    public int MaxConnections => 
        _config.TryParameter("MaxConnections", false, () => new Castable("100"))
                .TryInt32(c => 100);
    
    public decimal TaxRate => 
        _config.TryParameter("TaxRate", false, () => new Castable("0.16"))
                .TryDecimal(c => 0.16m);
    
    public bool EnableLogging => 
        _config.TryParameter("EnableLogging", false, () => new Castable("true"))
                .TryBoolean(c => true);
    
    public DateTime MaintenanceDate => 
        _config.TryParameter("MaintenanceDate", false, () => new Castable(DateTime.Now.AddDays(7).ToString()))
                .TryDateTime(c => DateTime.Now.AddDays(7));
}

// Uso
var config = new AppConfiguration();
var settings = new AppSettings(config);

Console.WriteLine($"Conexiones máximas: {settings.MaxConnections}");
Console.WriteLine($"Tasa de impuesto: {settings.TaxRate:P}");
Console.WriteLine($"Logging habilitado: {settings.EnableLogging}");
Console.WriteLine($"Mantenimiento: {settings.MaintenanceDate:dd/MM/yyyy}");
```

---

## Conclusión

La librería **HotPack** proporciona un conjunto robusto de herramientas para el desarrollo de aplicaciones .NET:

### Características Principales

1. **Patrón Result Consistente**: Todas las operaciones devuelven objetos `Result` o `Result<T>` que encapsulan éxito/fallo, mensajes y datos, facilitando el manejo uniforme de respuestas.

2. **Acceso a Datos Simplificado**: La clase `Conexion` abstrae la complejidad de ADO.NET y proporciona:
   - Mapeo automático de objetos
   - Soporte para procedimientos almacenados
   - Manejo de múltiples conjuntos de resultados
   - Parámetros de entrada/salida con mapeo automático

3. **Conversiones Seguras**: La clase `Castable` ofrece conversiones de tipos con dos enfoques:
   - Conversiones que lanzan excepciones para validación estricta
   - Conversiones con callbacks para manejo flexible de errores

4. **Integración Perfecta**: Todas las clases trabajan juntas de forma cohesiva:
   - `Conexion` devuelve `Result`/`Result<T>`/`ResultList<T>`
   - `AppConfiguration` usa `Castable` para parámetros
   - Los parámetros de salida se mapean automáticamente al `Result.Bag`

### Beneficios

✅ **Código más limpio**: Reduce boilerplate code  
✅ **Tipo-seguro**: Aprovecha el sistema de tipos de C#  
✅ **Consistente**: APIs uniformes en toda la librería  
✅ **Productivo**: Menos código para tareas comunes  
✅ **Mantenible**: Separación clara de responsabilidades  
✅ **Testeable**: Fácil de hacer unit testing  

### Casos de Uso Ideales

- **APIs REST**: Respuestas consistentes con `Result<T>`
- **Aplicaciones empresariales**: Acceso a datos con `Conexion`
- **Sistemas configurables**: Configuración con `AppConfiguration`
- **Microservicios**: Comunicación entre servicios con `Result`
- **Procesamiento de datos**: Conversiones seguras con `Castable`

---

**Versión**: 1.0  
**Fecha**: Noviembre 2025  
**Licencia**: [Especificar licencia]  
**Repositorio**: [URL del repositorio]

Para más información y ejemplos adicionales, consulte el archivo [documentacion.md](documentacion.md).// filepath: hotpack.md
# Documentación HotPack

## Índice

1. [Result](#result)
   - [Propiedades](#propiedades)
   - [Constructores](#constructores)
   - [Métodos](#métodos)
   - [Casos de Uso](#casos-de-uso-result)
2. [Result\<T\>](#resultt)
   - [Propiedades](#propiedades-1)
   - [Constructores](#constructores-1)
   - [Métodos](#métodos-1)
   - [Casos de Uso](#casos-de-uso-resultt)
3. [ResultList\<T\>](#resultlistt)
   - [Descripción](#descripción-resultlist)
   - [Casos de Uso](#casos-de-uso-resultlistt)
4. [ResultBagItem](#resultbagitem)
   - [Propiedades](#propiedades-2)
   - [Constructores](#constructores-2)
   - [Casos de Uso](#casos-de-uso-resultbagitem)
5. [Conexion](#conexion)
   - [Constructor](#constructor-conexion)
   - [Métodos Principales](#métodos-principales)
   - [Casos de Uso](#casos-de-uso-conexion)
6. [Clases Auxiliares](#clases-auxiliares)
   - [ConexionParameters](#conexionparameters)
   - [ConexionCastableRow](#conexioncastablerow)
   - [ConexionMultipleReader](#conexionmultiplereader)
   - [ConexionColumnAttribute](#conexioncolumnattribute)

---

## Result

**Namespace:** `HotPack.Classes`  
**Archivo:** [HotPack/Classes/Result.cs](HotPack/Classes/Result.cs)

### Descripción
La clase [`Result`](HotPack/Classes/Result.cs) es una estructura de respuesta estándar para operaciones que encapsula el resultado de ejecución (éxito/fallo), un mensaje descriptivo, datos opcionales y metadatos adicionales. Es ideal para crear APIs consistentes y manejar respuestas uniformes en toda la aplicación.

### Propiedades

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `Value` | `bool` | Indica si la operación fue exitosa (`true`) o falló (`false`) |
| `Message` | `string` | Mensaje descriptivo del resultado de la operación |
| `Data` | `object?` | Datos adicionales de cualquier tipo asociados al resultado |
| `Code` | `int` | Código numérico del resultado (útil para códigos HTTP o de error) |
| `Bag` | `Dictionary<string, object>` | Diccionario para almacenar metadatos adicionales clave-valor |

### Constructores

```csharp
// Constructor vacío
public Result()

// Constructor con valor booleano y mensaje
public Result(bool value, string msg)

// Constructor con valor, mensaje y datos
public Result(bool value, string msg, object data)

// Constructor desde una excepción
public Result(Exception ex)
```

### Métodos

#### Métodos Estáticos

##### `Create(bool value, string msg)`
Crea una nueva instancia de Result de forma estática.

**Parámetros:**
- `value` (bool): Indica si la operación fue exitosa
- `msg` (string): Mensaje descriptivo

**Retorna:** Nueva instancia de [`Result`](HotPack/Classes/Result.cs)

**Ejemplo:**
```csharp
var result = Result.Create(true, "Operación exitosa");
```

##### `Create(bool value, string msg, object data)`
Crea una nueva instancia de Result con datos.

**Parámetros:**
- `value` (bool): Indica si fue exitoso
- `msg` (string): Mensaje descriptivo
- `data` (object): Datos a incluir

**Retorna:** Nueva instancia de [`Result`](HotPack/Classes/Result.cs)

##### `Create(Exception ex)`
Crea una instancia de Result desde una excepción.

**Parámetros:**
- `ex` (Exception): Excepción capturada

**Retorna:** [`Result`](HotPack/Classes/Result.cs) con `Value = false` y el mensaje de la excepción

#### Métodos de Instancia

##### `Cast<M>()`
Convierte el Result actual a un Result genérico de tipo M, preservando el estado y metadatos.

**Retorna:** [`Result<M>`](HotPack/Classes/Result.cs) con los mismos valores de `Value`, `Message`, `Code` y `Bag`

**Ejemplo:**
```csharp
Result result = new Result(true, "OK");
Result<string> typedResult = result.Cast<string>();
```

##### `Cast<M>(M? data)`
Convierte el Result actual a un Result genérico de tipo M con datos específicos.

**Parámetros:**
- `data` (M?): Datos a incluir en el Result convertido

**Retorna:** [`Result<M>`](HotPack/Classes/Result.cs)

**Ejemplo:**
```csharp
Result result = new Result(true, "Usuario encontrado");
Result<Usuario> userResult = result.Cast<Usuario>(usuario);
```

##### `InfoMessage()` y `InfoMessage(string infoMessageValue)`
Gestiona un mensaje de información adicional interno.

**Retorna:** string con el mensaje informativo

### Casos de Uso Result

#### Caso 1: Validación de Datos

```csharp
using HotPack.Classes;

public class UsuarioService
{
    public Result ValidarUsuario(string email, string password)
    {
        if (string.IsNullOrEmpty(email))
        {
            return new Result(false, "El email es requerido");
        }

        if (password.Length < 8)
        {
            return new Result(false, "La contraseña debe tener al menos 8 caracteres");
        }

        return new Result(true, "Usuario válido");
    }
}

// Uso
var service = new UsuarioService();
var result = service.ValidarUsuario("user@email.com", "12345");

if (!result.Value)
{
    Console.WriteLine($"Error de validación: {result.Message}");
}
```

#### Caso 2: Manejo de Excepciones

```csharp
public Result GuardarArchivo(string ruta, byte[] contenido)
{
    try
    {
        File.WriteAllBytes(ruta, contenido);
        return Result.Create(true, "Archivo guardado exitosamente");
    }
    catch (UnauthorizedAccessException ex)
    {
        var result = new Result(ex);
        result.Code = 403;
        return result;
    }
    catch (Exception ex)
    {
        var result = new Result(ex);
        result.Code = 500;
        return result;
    }
}

// Uso
var resultado = GuardarArchivo("C:/temp/file.dat", data);
if (!resultado.Value)
{
    Console.WriteLine($"Error {resultado.Code}: {resultado.Message}");
}
```

#### Caso 3: Respuesta de API con Metadatos

```csharp
public Result ObtenerDatos()
{
    var result = new Result(true, "Datos obtenidos correctamente");
    result.Code = 200;
    result.Data = new { Nombre = "Juan", Edad = 30 };
    
    // Agregar metadatos al Bag
    result.Bag.Add("TotalRegistros", 1500);
    result.Bag.Add("PaginaActual", 1);
    result.Bag.Add("Timestamp", DateTime.Now);
    result.Bag.Add("Version", "1.0.0");
    
    return result;
}

// Uso
var resultado = ObtenerDatos();
Console.WriteLine($"Total de registros: {resultado.Bag["TotalRegistros"]}");
Console.WriteLine($"Versión: {resultado.Bag["Version"]}");
```

#### Caso 4: Conversión entre Tipos de Result

```csharp
public Result ProcesarDatos()
{
    var result = new Result(true, "Proceso completado");
    result.Code = 200;
    result.Bag.Add("DuracionMs", 150);
    
    return result;
}

// Convertir a Result genérico cuando necesitamos tipar los datos
var resultado = ProcesarDatos();
var resultadoTipado = resultado.Cast<List<string>>(new List<string> { "Item1", "Item2" });

Console.WriteLine($"Elementos: {resultadoTipado.Data.Count}");
Console.WriteLine($"Duración: {resultadoTipado.Bag["DuracionMs"]} ms");
```

---

## Result\<T\>

**Namespace:** `HotPack.Classes`  
**Archivo:** [HotPack/Classes/Result.cs](HotPack/Classes/Result.cs)

### Descripción
La clase [`Result<T>`](HotPack/Classes/Result.cs) es la versión genérica de Result que proporciona tipado fuerte para la propiedad `Data`. Esto permite trabajar con datos específicos de forma segura en tiempo de compilación y aprovecha IntelliSense en el IDE.

### Propiedades

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `Value` | `bool` | Indica si la operación fue exitosa |
| `Message` | `string` | Mensaje descriptivo del resultado |
| `Data` | `T?` | Datos tipados asociados al resultado |
| `Code` | `int` | Código numérico del resultado |
| `Bag` | `Dictionary<string, object>` | Diccionario para metadatos adicionales |

### Constructores

```csharp
// Constructor vacío
public Result()

// Constructor con valor y mensaje
public Result(bool value, string msg)

// Constructor con valor, mensaje y datos tipados
public Result(bool value, string msg, T data)

// Constructor desde excepción
public Result(Exception ex)

// Constructor desde Result no genérico
public Result(Result origin)
```

### Métodos

#### Métodos Estáticos

##### `Create(bool value, string msg)`
Crea una nueva instancia de Result\<T\>.

**Retorna:** Nueva instancia de [`Result`](HotPack/Classes/Result.cs) (no genérico)

##### `Create(bool value, string msg, T data)`
Crea una instancia con datos tipados.

**Parámetros:**
- `value` (bool): Éxito o fallo
- `msg` (string): Mensaje
- `data` (T): Datos tipados

**Retorna:** Nueva instancia de [`Result`](HotPack/Classes/Result.cs)

##### `Create(Exception ex)`
Crea una instancia desde una excepción.

**Retorna:** [`Result`](HotPack/Classes/Result.cs) con el error

#### Métodos de Instancia

##### `Cast<M>()`
Convierte el Result\<T\> actual a un Result\<M\>.

**Retorna:** [`Result<M>`](HotPack/Classes/Result.cs) sin datos, solo con estado y metadatos

**Ejemplo:**
```csharp
Result<Usuario> userResult = GetUser();
Result<UsuarioDto> dtoResult = userResult.Cast<UsuarioDto>();
```

##### `Cast<M>(M? data)`
Convierte a Result\<M\> incluyendo datos específicos.

**Parámetros:**
- `data` (M?): Datos del nuevo tipo

**Retorna:** [`Result<M>`](HotPack/Classes/Result.cs) con datos

**Ejemplo:**
```csharp
Result<Usuario> userResult = GetUser();
var dto = mapper.Map<UsuarioDto>(userResult.Data);
Result<UsuarioDto> dtoResult = userResult.Cast<UsuarioDto>(dto);
```

##### `Map<M>(Func<T?, M> func)`
Transforma los datos del Result usando una función de mapeo. Ideal para integración con AutoMapper u otras librerías.

**Parámetros:**
- `func` (Func<T?, M>): Función que transforma los datos de tipo T a tipo M

**Retorna:** [`Result<M>`](HotPack/Classes/Result.cs) con los datos transformados

**Ejemplo:**
```csharp
Result<Usuario> userResult = GetUser();

// Transformación manual
var dtoResult = userResult.Map(user => new UsuarioDto
{
    Nombre = user.Nombre,
    Email = user.Email
});

// Con AutoMapper
var mappedResult = userResult.Map(user => _mapper.Map<UsuarioDto>(user));
```

### Casos de Uso Result\<T\>

#### Caso 1: Servicio de Datos Tipado

```csharp
using HotPack.Classes;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Email { get; set; }
    public DateTime FechaRegistro { get; set; }
}

public class UsuarioRepository
{
    public async Task<Result<Usuario>> ObtenerPorIdAsync(int id)
    {
        try
        {
            var usuario = await _dbContext.Usuarios.FindAsync(id);
            
            if (usuario == null)
            {
                return new Result<Usuario>(false, "Usuario no encontrado");
            }
            
            return new Result<Usuario>(true, "Usuario obtenido correctamente", usuario);
        }
        catch (Exception ex)
        {
            return new Result<Usuario>(ex);
        }
    }
}

// Uso
var repository = new UsuarioRepository();
var resultado = await repository.ObtenerPorIdAsync(123);

if (resultado.Value && resultado.Data != null)
{
    Console.WriteLine($"Usuario: {resultado.Data.Nombre}");
    Console.WriteLine($"Email: {resultado.Data.Email}");
}
else
{
    Console.WriteLine($"Error: {resultado.Message}");
}
```

#### Caso 2: API Controller con Result\<T\>

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _productoService;

    public ProductosController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerProducto(int id)
    {
        var resultado = await _productoService.ObtenerPorIdAsync(id);
        
        if (!resultado.Value)
        {
            return StatusCode(resultado.Code, new 
            { 
                mensaje = resultado.Message,
                codigo = resultado.Code
            });
        }
        
        return Ok(new
        {
            success = true,
            data = resultado.Data,
            mensaje = resultado.Message
        });
    }

    [HttpPost]
    public async Task<IActionResult> CrearProducto([FromBody] ProductoDto dto)
    {
        var resultado = await _productoService.CrearAsync(dto);
        
        resultado.Bag.Add("FechaCreacion", DateTime.Now);
        
        if (!resultado.Value)
        {
            return BadRequest(new
            {
                success = false,
                mensaje = resultado.Message,
                errores = resultado.Bag
            });
        }
        
        return CreatedAtAction(
            nameof(ObtenerProducto), 
            new { id = resultado.Data.Id }, 
            resultado.Data
        );
    }
}
```

#### Caso 3: Transformación de Datos con Map

```csharp
public class UsuarioDto
{
    public string NombreCompleto { get; set; }
    public string CorreoElectronico { get; set; }
}

public class UsuarioService
{
    private readonly UsuarioRepository _repository;

    public async Task<Result<UsuarioDto>> ObtenerUsuarioDtoAsync(int id)
    {
        var userResult = await _repository.ObtenerPorIdAsync(id);
        
        if (!userResult.Value)
        {
            return userResult.Cast<UsuarioDto>();
        }
        
        // Transformar Usuario a UsuarioDto
        var dtoResult = userResult.Map(usuario => new UsuarioDto
        {
            NombreCompleto = usuario.Nombre,
            CorreoElectronico = usuario.Email
        });
        
        return dtoResult;
    }
}

// Uso
var service = new UsuarioService();
var resultado = await service.ObtenerUsuarioDtoAsync(100);

if (resultado.Value && resultado.Data != null)
{
    Console.WriteLine($"DTO: {resultado.Data.NombreCompleto}");
}
```

#### Caso 4: Encadenamiento de Operaciones

```csharp
public class PedidoService
{
    public async Task<Result<Pedido>> ProcesarPedidoAsync(int pedidoId)
    {
        // Obtener pedido
        var pedidoResult = await ObtenerPedidoAsync(pedidoId);
        if (!pedidoResult.Value)
        {
            return pedidoResult;
        }
        
        // Validar stock
        var stockResult = await ValidarStockAsync(pedidoResult.Data);
        if (!stockResult.Value)
        {
            return new Result<Pedido>(false, stockResult.Message);
        }
        
        // Procesar pago
        var pagoResult = await ProcesarPagoAsync(pedidoResult.Data);
        if (!pagoResult.Value)
        {
            return new Result<Pedido>(false, pagoResult.Message)
            {
                Code = 402,
                Bag = pagoResult.Bag
            };
        }
        
        // Actualizar estado
        pedidoResult.Data.Estado = "Procesado";
        await GuardarPedidoAsync(pedidoResult.Data);
        
        var result = new Result<Pedido>(true, "Pedido procesado exitosamente", pedidoResult.Data);
        result.Code = 200;
        result.Bag.Add("TransaccionId", pagoResult.Bag["TransaccionId"]);
        
        return result;
    }
}
```

#### Caso 5: Constructor desde Result No Genérico

```csharp
public Result ValidarDatos(object datos)
{
    // Validaciones varias...
    var result = new Result(true, "Validación exitosa");
    result.Code = 200;
    result.Bag.Add("CamposValidados", 15);
    
    return result;
}

public Result<Producto> CrearProducto(ProductoDto dto)
{
    // Validar primero
    var validacion = ValidarDatos(dto);
    
    if (!validacion.Value)
    {
        // Crear Result<Producto> desde Result
        return new Result<Producto>(validacion);
    }
    
    var producto = new Producto
    {
        Nombre = dto.Nombre,
        Precio = dto.Precio
    };
    
    return new Result<Producto>(true, "Producto creado", producto);
}
```

---

## ResultList\<T\>

**Namespace:** `HotPack.Classes`  
**Archivo:** [HotPack/Classes/Result.cs](HotPack/Classes/Result.cs)

### Descripción ResultList

La clase [`ResultList<T>`](HotPack/Classes/Result.cs) es una especialización de `Result<List<T>>` diseñada específicamente para operaciones que devuelven colecciones de elementos. Hereda toda la funcionalidad de Result\<T\> pero está semánticamente optimizada para trabajar con listas.

**Herencia:** `ResultList<T>` → `Result<List<T>>` → objeto

### Propiedades
Hereda todas las propiedades de [`Result<List<T>>`](HotPack/Classes/Result.cs):
- `Value`: bool - Indica éxito o fallo
- `Message`: string - Mensaje descriptivo
- `Data`: List\<T\>? - Lista de elementos tipados
- `Code`: int - Código de resultado
- `Bag`: Dictionary<string, object> - Metadatos adicionales

### Casos de Uso ResultList\<T\>

#### Caso 1: Listado de Productos con Paginación

```csharp
using HotPack.Classes;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    public string Categoria { get; set; }
}

public class ProductoService
{
    public ResultList<Producto> ObtenerProductos(int pagina = 1, int tamano = 10)
    {
        try
        {
            var productos = _repository.GetAll()
                .Skip((pagina - 1) * tamano)
                .Take(tamano)
                .ToList();
            
            var totalRegistros = _repository.Count();
            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamano);
            
            var result = new ResultList<Producto>
            {
                Value = true,
                Message = "Productos obtenidos correctamente",
                Code = 200,
                Data = productos
            };
            
            // Metadatos de paginación
            result.Bag.Add("TotalRegistros", totalRegistros);
            result.Bag.Add("PaginaActual", pagina);
            result.Bag.Add("TamanoPagina", tamano);
            result.Bag.Add("TotalPaginas", totalPaginas);
            result.Bag.Add("TieneAnterior", pagina > 1);
            result.Bag.Add("TieneSiguiente", pagina < totalPaginas);
            
            return result;
        }
        catch (Exception ex)
        {
            return new ResultList<Producto>
            {
                Value = false,
                Message = ex.Message,
                Code = 500
            };
        }
    }
}

// Uso
var service = new ProductoService();
var resultado = service.ObtenerProductos(pagina: 2, tamano: 20);

if (resultado.Value && resultado.Data != null)
{
    Console.WriteLine($"Mostrando {resultado.Data.Count} productos");
    Console.WriteLine($"Página {resultado.Bag["PaginaActual"]} de {resultado.Bag["TotalPaginas"]}");
    
    foreach (var producto in resultado.Data)
    {
        Console.WriteLine($"{producto.Nombre}: ${producto.Precio}");
    }
    
    if ((bool)resultado.Bag["TieneSiguiente"])
    {
        Console.WriteLine("Hay más productos disponibles");
    }
}
```

#### Caso 2: Búsqueda con Filtros

```csharp
public class ClienteService
{
    public ResultList<Cliente> BuscarClientes(string? termino, string? ciudad, bool? activo)
    {
        var query = _dbContext.Clientes.AsQueryable();
        
        if (!string.IsNullOrEmpty(termino))
        {
            query = query.Where(c => c.Nombre.Contains(termino) || c.Email.Contains(termino));
        }
        
        if (!string.IsNullOrEmpty(ciudad))
        {
            query = query.Where(c => c.Ciudad == ciudad);
        }
        
        if (activo.HasValue)
        {
            query = query.Where(c => c.Activo == activo.Value);
        }
        
        var clientes = query.ToList();
        
        var result = new ResultList<Cliente>
        {
            Value = true,
            Message = clientes.Any() ? "Clientes encontrados" : "No se encontraron clientes",
            Code = 200,
            Data = clientes
        };
        
        result.Bag.Add("TotalEncontrados", clientes.Count);
        result.Bag.Add("FiltrosAplicados", new
        {
            Termino = termino,
            Ciudad = ciudad,
            Activo = activo
        });
        result.Bag.Add("FechaBusqueda", DateTime.Now);
        
        return result;
    }
}

// Uso
var resultado = service.BuscarClientes(termino: "Juan", ciudad: "Madrid", activo: true);

if (resultado.Value)
{
    Console.WriteLine($"Encontrados: {resultado.Bag["TotalEncontrados"]} clientes");
    
    foreach (var cliente in resultado.Data)
    {
        Console.WriteLine($"{cliente.Nombre} - {cliente.Ciudad}");
    }
}
```

#### Caso 3: Export/Reporte de Datos

```csharp
public class ReporteService
{
    public ResultList<VentaResumen> GenerarReporteVentas(DateTime desde, DateTime hasta)
    {
        try
        {
            var ventas = _repository.GetVentas()
                .Where(v => v.Fecha >= desde && v.Fecha <= hasta)
                .Select(v => new VentaResumen
                {
                    Fecha = v.Fecha,
                    Total = v.Total,
                    Cliente = v.Cliente.Nombre,
                    NumeroItems = v.Items.Count
                })
                .OrderByDescending(v => v.Fecha)
                .ToList();
            
            var totalVentas = ventas.Sum(v => v.Total);
            var promedioVenta = ventas.Any() ? ventas.Average(v => v.Total) : 0;
            
            var result = new ResultList<VentaResumen>
            {
                Value = true,
                Message = $"Reporte generado con {ventas.Count} ventas",
                Code = 200,
                Data = ventas
            };
            
            result.Bag.Add("FechaDesde", desde);
            result.Bag.Add("FechaHasta", hasta);
            result.Bag.Add("TotalVentas", totalVentas);
            result.Bag.Add("PromedioVenta", promedioVenta);
            result.Bag.Add("CantidadVentas", ventas.Count);
            result.Bag.Add("FechaGeneracion", DateTime.Now);
            
            return result;
        }
        catch (Exception ex)
        {
            return new ResultList<VentaResumen>
            {
                Value = false,
                Message = $"Error al generar reporte: {ex.Message}",
                Code = 500
            };
        }
    }
}

// Uso
var resultado = service.GenerarReporteVentas(
    desde: new DateTime(2024, 1, 1),
    hasta: DateTime.Now
);

if (resultado.Value)
{
    Console.WriteLine($"Reporte del {resultado.Bag["FechaDesde"]} al {resultado.Bag["FechaHasta"]}");
    Console.WriteLine($"Total vendido: ${resultado.Bag["TotalVentas"]}");
    Console.WriteLine($"Promedio por venta: ${resultado.Bag["PromedioVenta"]:F2}");
    Console.WriteLine($"\nDetalle de ventas:");
    
    foreach (var venta in resultado.Data)
    {
        Console.WriteLine($"{venta.Fecha:dd/MM/yyyy} - {venta.Cliente}: ${venta.Total}");
    }
}
```

#### Caso 4: API Response con Metadata

```csharp
[HttpGet("clientes")]
public IActionResult ObtenerClientes([FromQuery] int pagina = 1, [FromQuery] int tamano = 10)
{
    var resultado = _clienteService.ObtenerClientesPaginados(pagina, tamano);
    
    if (!resultado.Value)
    {
        return StatusCode(resultado.Code, new { mensaje = resultado.Message });
    }
    
    var response = new
    {
        success = true,
        message = resultado.Message,
        data = resultado.Data,
        paginacion = new
        {
            total = resultado.Bag["TotalRegistros"],
            paginaActual = resultado.Bag["PaginaActual"],
            tamanoPagina = resultado.Bag["TamanoPagina"],
            totalPaginas = resultado.Bag["TotalPaginas"],
            tieneAnterior = resultado.Bag["TieneAnterior"],
            tieneSiguiente = resultado.Bag["TieneSiguiente"]
        }
    };
    
    return Ok(response);
}
```

#### Caso 5: Carga de Datos con Estadísticas

```csharp
public class EmpleadoService
{
    public ResultList<Empleado> ObtenerEmpleadosPorDepartamento(string departamento)
    {
        var empleados = _repository.GetAll()
            .Where(e => e.Departamento == departamento)
            .ToList();
        
        if (!empleados.Any())
        {
            return new ResultList<Empleado>
            {
                Value = false,
                Message = $"No se encontraron empleados en el departamento {departamento}",
                Code = 404
            };
        }
        
        var result = new ResultList<Empleado>
        {
            Value = true,
            Message = "Empleados obtenidos correctamente",
            Code = 200,
            Data = empleados
        };
        
        // Estadísticas del departamento
        result.Bag.Add("Departamento", departamento);
        result.Bag.Add("TotalEmpleados", empleados.Count);
        result.Bag.Add("SalarioPromedio", empleados.Average(e => e.Salario));
        result.Bag.Add("SalarioMaximo", empleados.Max(e => e.Salario));
        result.Bag.Add("SalarioMinimo", empleados.Min(e => e.Salario));
        result.Bag.Add("AntiguedadPromedio", empleados.Average(e => 
            (DateTime.Now - e.FechaContratacion).TotalDays / 365));
        
        return result;
    }
}

// Uso
var resultado = service.ObtenerEmpleadosPorDepartamento("Desarrollo");

if (resultado.Value && resultado.Data != null)
{
    Console.WriteLine($"Departamento: {resultado.Bag["Departamento"]}");
    Console.WriteLine($"Total empleados: {resultado.Bag["TotalEmpleados"]}");
    Console.WriteLine($"Salario promedio: ${resultado.Bag["SalarioPromedio"]:F2}");
    Console.WriteLine($"\nLista de empleados:");
    
    foreach (var emp in resultado.Data)
    {
        Console.WriteLine($"- {emp.Nombre}: ${emp.Salario}");
    }
}
```

---

## ResultBagItem

**Namespace:** `HotPack.Classes`  
**Archivo:** [HotPack/Classes/Result.cs](HotPack/Classes/Result.cs)

### Descripción
La clase [`ResultBagItem`](HotPack/Classes/Result.cs) representa un elemento clave-valor que puede ser almacenado en colecciones. Aunque las clases Result usan `Dictionary<string, object>` directamente para el Bag, esta clase proporciona una estructura cuando se necesita trabajar con elementos individuales de forma más explícita.

### Propiedades

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `Key` | `string` | Clave identificadora del elemento |
| `Value` | `object` | Valor asociado a la clave |

### Constructores

```csharp
// Constructor vacío
public ResultBagItem()

// Constructor con clave y valor
public ResultBagItem(string key, object value)
```

### Casos de Uso ResultBagItem

#### Caso 1: Construcción Dinámica de Metadatos

```csharp
using HotPack.Classes;

public class MetadataBuilder
{
    private List<ResultBagItem> _items = new List<ResultBagItem>();
    
    public MetadataBuilder AddItem(string key, object value)
    {
        _items.Add(new ResultBagItem(key, value));
        return this;
    }
    
    public Dictionary<string, object> Build()
    {
        var dictionary = new Dictionary<string, object>();
        foreach (var item in _items)
        {
            dictionary[item.Key] = item.Value;
        }
        return dictionary;
    }
}

// Uso
var metadata = new MetadataBuilder()
    .AddItem("Usuario", "admin")
    .AddItem("Timestamp", DateTime.Now)
    .AddItem("IP", "192.168.1.1")
    .AddItem("Version", "1.0.0")
    .Build();

var result = new Result(true, "Operación exitosa");
foreach (var kvp in metadata)
{
    result.Bag[kvp.Key] = kvp.Value;
}
```

#### Caso 2: Serialización de Metadatos

```csharp
public class AuditoriaService
{
    public void RegistrarAuditoria(Result result)
    {
        var items = new List<ResultBagItem>();
        
        foreach (var kvp in result.Bag)
        {
            items.Add(new ResultBagItem(kvp.Key, kvp.Value));
        }
        
        var auditoria = new Auditoria
        {
            Fecha = DateTime.Now,
            Resultado = result.Value,
            Mensaje = result.Message,
            Metadatos = JsonSerializer.Serialize(items)
        };
        
        _repository.Save(auditoria);
    }
}
```

#### Caso 3: Validación de Metadatos Requeridos

```csharp
public class MetadataValidator
{
    private readonly List<string> _requiredKeys;
    
    public MetadataValidator(params string[] requiredKeys)
    {
        _requiredKeys = requiredKeys.ToList();
    }
    
    public Result ValidarMetadatos(Dictionary<string, object> bag)
    {
        var faltantes = new List<string>();
        
        foreach (var key in _requiredKeys)
        {
            if (!bag.ContainsKey(key))
            {
                faltantes.Add(key);
            }
        }
        
        if (faltantes.Any())
        {
            return new Result(false, $"Faltan metadatos requeridos: {string.Join(", ", faltantes)}");
        }
        
        return new Result(true, "Todos los metadatos requeridos están presentes");
    }
}

// Uso
var validator = new MetadataValidator("UserId", "Timestamp", "Action");
var resultado = validator.ValidarMetadatos(result.Bag);

if (!resultado.Value)
{
    Console.WriteLine(resultado.Message);
}
```

---

## Conexion

**Namespace:** `HotPack.Database`  
**Archivo:** [HotPack.Database/Conexion.cs](HotPack.Database/Conexion.cs)

### Descripción
La clase [`Conexion`](HotPack.Database/Conexion.cs) es una capa de acceso a datos optimizada para SQL Server que proporciona métodos asíncronos para ejecutar consultas, procedimientos almacenados y mapear resultados a objetos .NET. Se integra perfectamente con la clase [`Result`](HotPack/Classes/Result.cs) para proporcionar respuestas consistentes.

### Constructor Conexion

```csharp
public Conexion(string connectionString)
```

**Parámetros:**
- `connectionString` (string): Cadena de conexión a la base de datos SQL Server

**Ejemplo:**
```csharp
var connectionString = "Server=localhost;Database=MiDB;Integrated Security=true;";
var conexion = new Conexion(connectionString);
```

### Métodos Principales

#### `ExecuteAsync`
Ejecuta un comando SQL que no devuelve resultados (INSERT, UPDATE, DELETE).

**Firma:**
```csharp
public async Task<Result> ExecuteAsync(
    string query, 
    ConexionParameters? parameters = null, 
    CommandType? commandType = CommandType.StoredProcedure, 
    int? commandTimeout = 30)
```

**Parámetros:**
- `query` (string): Consulta SQL o nombre del procedimiento almacenado
- `parameters` (ConexionParameters?): Parámetros de la consulta (opcional)
- `commandType` (CommandType?): Tipo de comando (default: StoredProcedure)
- `commandTimeout` (int?): Timeout en segundos (default: 30)

**Retorna:** [`Result`](HotPack/Classes/Result.cs) con información de la ejecución

**Características:**
- Mapea automáticamente parámetros de salida a `Result.Value`, `Result.Message` y `Result.Code`
- Agrega parámetros de salida adicionales al `Result.Bag`

---

#### `ExecuteWithResultsAsync<T>`
Ejecuta una consulta y mapea los resultados a una lista de objetos del tipo especificado.

**Firma:**
```csharp
public async Task<ResultList<T>> ExecuteWithResultsAsync<T>(
    string query, 
    ConexionParameters? parameters = null, 
    CommandType? commandType = CommandType.StoredProcedure, 
    int? commandTimeout = 30)
```

**Parámetros:**
- `query` (string): Consulta SQL o nombre del procedimiento
- `parameters` (ConexionParameters?): Parámetros opcionales
- `commandType` (CommandType?): Tipo de comando
- `commandTimeout` (int?): Timeout en segundos

**Retorna:** [`ResultList<T>`](HotPack/Classes/Result.cs) con los datos mapeados

**Características:**
- Mapea automáticamente columnas a propiedades usando reflexión
- Soporta tipos primitivos y objetos complejos
- Respeta el atributo `ConexionColumnAttribute` para mapeo personalizado

---

#### `ExecuteToObjectAsync<T>`
Ejecuta una consulta esperando un único objeto como resultado (equivalente a FirstOrDefault).

**Firma:**
```csharp
public async Task<Result<T?>> ExecuteToObjectAsync<T>(
    string query, 
    ConexionParameters? parameters, 
    CommandType? commandType = CommandType.StoredProcedure, 
    int? commandTimeout = 30)
```

**Retorna:** [`Result<T?>`](HotPack/Classes/Result.cs) con el objeto mapeado o null

---

#### `ExecuteScalarAsync<T>`
Ejecuta una consulta que devuelve un valor escalar (COUNT, SUM, MAX, etc.).

**Firma:**
```csharp
public async Task<Result<T>> ExecuteScalarAsync<T>(
    string query, 
    ConexionParameters? parameters = null, 
    CommandType? commandType = CommandType.StoredProcedure, 
    int? commandTimeout = 30)
```

**Retorna:** [`Result<T>`](HotPack/Classes/Result.cs) con el valor escalar

---

#### `ExecuteWithMultipleResultsAsync`
Ejecuta una consulta que devuelve múltiples conjuntos de resultados.

**Firma:**
```csharp
public async Task<Result> ExecuteWithMultipleResultsAsync(
    string query, 
    ConexionParameters? parameters, 
    Func<ConexionMultipleReader, Task> readerFunc, 
    CommandType? commandType = CommandType.StoredProcedure, 
    int? commandTimeout = 30)
```

**Parámetros:**
- `readerFunc` (Func<ConexionMultipleReader, Task>): Función que procesa múltiples resultados

**Retorna:** [`Result`](HotPack/Classes/Result.cs)

---

#### `ExecuteWithMultipleResultsAsync<T>`
Versión genérica que devuelve datos del procesamiento de múltiples resultados.

**Firma:**
```csharp
public async Task<Result<T>> ExecuteWithMultipleResultsAsync<T>(
    string query, 
    ConexionParameters? parameters, 
    Func<ConexionMultipleReader, Task<T>> readerFunc, 
    CommandType? commandType = CommandType.StoredProcedure, 
    int? commandTimeout = 30)
```

**Retorna:** [`Result<T>`](HotPack/Classes/Result.cs) con los datos procesados

---

### Mapeo Automático de Parámetros

La clase [`Conexion`](HotPack.Database/Conexion.cs) mapea automáticamente parámetros de salida con nombres específicos:

| Nombres Reconocidos | Propiedad de Result |
|---------------------|---------------------|
| `result`, `presult`, `resultado`, `presultado` | `Result.Value` |
| `msg`, `pmsg`, `message`, `pmessage`, `mensaje`, `pmensaje` | `Result.Message` |
| `code`, `pcode`, `codigo`, `pcodigo` | `Result.Code` |

Los demás parámetros de salida se agregan automáticamente al `Result.Bag`.

### Casos de Uso Conexion

#### Caso 1: CRUD Básico - Insertar Usuario

```csharp
using HotPack.Database;
using HotPack.Classes;

public class UsuarioRepository
{
    private readonly Conexion _conexion;
    
    public UsuarioRepository(string connectionString)
    {
        _conexion = new Conexion(connectionString);
    }
    
    public async Task<Result> InsertarUsuarioAsync(string nombre, string email, string password)
    {
        var parametros = new ConexionParameters()
            .Add("Nombre", nombre)
            .Add("Email", email)
            .Add("Password", password)
            .AddOutput("UsuarioId", DbType.Int32)
            .AddOutput("Result", DbType.Boolean)
            .AddOutput("Message", DbType.String, 500);
        
        var resultado = await _conexion.ExecuteAsync("sp_InsertarUsuario", parametros);
        
        if (resultado.Value && resultado.Bag.ContainsKey("UsuarioId"))
        {
            Console.WriteLine($"Usuario creado con ID: {resultado.Bag["UsuarioId"]}");
        }
        
        return resultado;
    }
}

// Uso
var repo = new UsuarioRepository(connectionString);
var resultado = await repo.InsertarUsuarioAsync("Juan Pérez", "juan@email.com", "pass123");

if (resultado.Value)
{
    Console.WriteLine($"Éxito: {resultado.Message}");
    Console.WriteLine($"ID generado: {resultado.Bag["UsuarioId"]}");
}
else
{
    Console.WriteLine($"Error: {resultado.Message}");
}
```

**Procedimiento almacenado:**
```sql
CREATE PROCEDURE sp_InsertarUsuario
    @Nombre VARCHAR(100),
    @Email VARCHAR(100),
    @Password VARCHAR(100),
    @UsuarioId INT OUTPUT,
    @Result BIT OUTPUT,
    @Message VARCHAR(500) OUTPUT
AS
BEGIN
    BEGIN TRY
        IF EXISTS(SELECT 1 FROM Usuarios WHERE Email = @Email)
        BEGIN
            SET @Result = 0
            SET @Message = 'El email ya está registrado'
            RETURN
        END
        
        INSERT INTO Usuarios (Nombre, Email, Password, FechaCreacion)
        VALUES (@Nombre, @Email, @Password, GETDATE())
        
        SET @UsuarioId = SCOPE_IDENTITY()
        SET @Result = 1
        SET @Message = 'Usuario creado exitosamente'
    END TRY
    BEGIN CATCH
        SET @Result = 0
        SET @Message = ERROR_MESSAGE()
    END CATCH
END
```

---

#### Caso 2: Obtener Lista de Productos

```csharp
public class Producto
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string Categoria { get; set; }
}

public class ProductoRepository
{
    private readonly Conexion _conexion;
    
    public ProductoRepository(string connectionString)
    {
        _conexion = new Conexion(connectionString);
    }
    
    public async Task<ResultList<Producto>> ObtenerProductosAsync(string? categoria = null)
    {
        var parametros = new ConexionParameters();
        
        if (!string.IsNullOrEmpty(categoria))
        {
            parametros.Add("Categoria", categoria);
        }
        
        var resultado = await _conexion.ExecuteWithResultsAsync<Producto>(
            "sp_ObtenerProductos", 
            parametros
        );
        
        return resultado;
    }
    
    public async Task<ResultList<Producto>> ObtenerProductosPaginadosAsync(
        int pagina, 
        int tamanoPagina)
    {
        var parametros = new ConexionParameters()
            .Add("Pagina", pagina)
            .Add("TamanoPagina", tamanoPagina)
            .AddOutput("TotalRegistros", DbType.Int32, alias: "TotalRegistros")
            .AddOutput("TotalPaginas", DbType.Int32, alias: "TotalPaginas");
        
        var resultado = await _conexion.ExecuteWithResultsAsync<Producto>(
            "sp_ObtenerProductosPaginados", 
            parametros
        );
        
        return resultado;
    }
}

// Uso
var repo = new ProductoRepository(connectionString);

// Sin filtros
var todos = await repo.ObtenerProductosAsync();
if (todos.Value && todos.Data != null)
{
    Console.WriteLine($"Total productos: {todos.Data.Count}");
    foreach (var p in todos.Data)
    {
        Console.WriteLine($"{p.Nombre}: ${p.Precio} (Stock: {p.Stock})");
    }
}

// Con filtro
var electronicos = await repo.ObtenerProductosAsync("Electrónica");

// Paginados
var paginados = await repo.ObtenerProductosPaginadosAsync(pagina: 1, tamanoPagina: 20);
if (paginados.Value)
{
    Console.WriteLine($"Mostrando {paginados.Data.Count} de {paginados.Bag["TotalRegistros"]}");
    Console.WriteLine($"Página 1 de {paginados.Bag["TotalPaginas"]}");
}
```

**Procedimiento almacenado:**
```sql
CREATE PROCEDURE sp_ObtenerProductosPaginados
    @Pagina INT,
    @TamanoPagina INT,
    @TotalRegistros INT OUTPUT,
    @TotalPaginas INT OUTPUT
AS
BEGIN
    SELECT @TotalRegistros = COUNT(*) FROM Productos
    SET @TotalPaginas = CEILING(CAST(@TotalRegistros AS FLOAT) / @TamanoPagina)
    
    SELECT ProductoId, Nombre, Precio, Stock, Categoria
    FROM Productos
    ORDER BY Nombre
    OFFSET (@Pagina - 1) * @TamanoPagina ROWS
    FETCH NEXT @TamanoPagina ROWS ONLY
END
```

---

#### Caso 3: Obtener Un Solo Objeto

```csharp
public class Cliente
{
    public int ClienteId { get; set; }
    public string NombreCompleto { get; set; }
    public string Email { get; set; }
    public string Telefono { get; set; }
    public DateTime FechaRegistro { get; set; }
}

public class ClienteRepository
{
    private readonly Conexion _conexion;
    
    public ClienteRepository(string connectionString)
    {
        _conexion = new Conexion(connectionString);
    }
    
    public async Task<Result<Cliente?>> ObtenerPorIdAsync(int clienteId)
    {
        var parametros = new ConexionParameters()
            .Add("ClienteId", clienteId)
            .AddOutput("Result", DbType.Boolean)
            .AddOutput("Message", DbType.String, 500);
        
        var resultado = await _conexion.ExecuteToObjectAsync<Cliente>(
            "sp_ObtenerClientePorId", 
            parametros
        );
        
        return resultado;
    }
    
    public async Task<Result<Cliente?>> ObtenerPorEmailAsync(string email)
    {
        var parametros = new ConexionParameters()
            .Add("Email", email);
        
        return await _conexion.ExecuteToObjectAsync<Cliente>(
            "sp_ObtenerClientePorEmail", 
            parametros
        );
    }
}

// Uso
var repo = new ClienteRepository(connectionString);

var resultado = await repo.ObtenerPorIdAsync(123);

if (resultado.Value && resultado.Data != null)
{
    var cliente = resultado.Data;
    Console.WriteLine($"Cliente: {cliente.NombreCompleto}");
    Console.WriteLine($"Email: {cliente.Email}");
    Console.WriteLine($"Registrado: {cliente.FechaRegistro:dd/MM/yyyy}");
}
else if (resultado.Value && resultado.Data == null)
{
    Console.WriteLine("Cliente no encontrado");
}
else
{
    Console.WriteLine($"Error: {resultado.Message}");
}
```

---

#### Caso 4: ExecuteScalar - Obtener Totales y Conteos

```csharp
public class EstadisticasService
{
    private readonly Conexion _conexion;
    
    public EstadisticasService(string connectionString)
    {
        _conexion = new Conexion(connectionString);
    }
    
    public async Task<Result<int>> ContarClientesActivosAsync()
    {
        return await _conexion.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM Clientes WHERE Activo = 1",
            commandType: CommandType.Text
        );
    }
    
    public async Task<Result<decimal>> ObtenerTotalVentasDelMesAsync()
    {
        var parametros = new ConexionParameters()
            .Add("Mes", DateTime.Now.Month)
            .Add("Anio", DateTime.Now.Year);
        
        return await _conexion.ExecuteScalarAsync<decimal>(
            "sp_ObtenerTotalVentasMes",
            parametros
        );
    }
    
    public async Task<Result<DateTime>> ObtenerUltimaActualizacionAsync()
    {
        return await _conexion.ExecuteScalarAsync<DateTime>(
            "SELECT MAX(FechaActualizacion) FROM Productos",
            commandType: CommandType.Text
        );
    }
}

// Uso
var service = new EstadisticasService(connectionString);

var totalClientes = await service.ContarClientesActivosAsync();
if (totalClientes.Value)
{
    Console.WriteLine($"Clientes activos: {totalClientes.Data}");
}

var ventasMes = await service.ObtenerTotalVentasDelMesAsync();
if (ventasMes.Value)
{
    Console.WriteLine($"Ventas del mes: ${ventasMes.Data:N2}");
}

var ultimaActualizacion = await service.ObtenerUltimaActualizacionAsync();
if (ultimaActualizacion.Value)
{
    Console.WriteLine($"Última actualización: {ultimaActualizacion.Data:dd/MM/yyyy HH:mm}");
}
```

---

#### Caso 5: Múltiples Resultados - Dashboard Completo

```csharp
public class DashboardData
{
    public List<VentaReciente> VentasRecientes { get; set; }
    public List<ProductoMasVendido> ProductosTop { get; set; }
    public List<ClienteTop> ClientesTop { get; set; }
    public EstadisticasGenerales Estadisticas { get; set; }
}

public class DashboardService
{
    private readonly Conexion _conexion;
    
    public DashboardService(string connectionString)
    {
        _conexion = new Conexion(connectionString);
    }
    
    public async Task<Result<DashboardData>> ObtenerDashboardAsync()
    {
        var parametros = new ConexionParameters()
            .Add("Dias", 30);
        
        var resultado = await _conexion.ExecuteWithMultipleResultsAsync<DashboardData>(
            "sp_ObtenerDatosDashboard",
            parametros,
            async (reader) =>
            {
                var dashboard = new DashboardData();
                
                // Primer conjunto de resultados: Ventas recientes
                dashboard.VentasRecientes = await reader.ReadAsync<VentaReciente>();
                
                // Segundo conjunto: Productos más vendidos
                dashboard.ProductosTop = await reader.ReadAsync<ProductoMasVendido>();
                
                // Tercer conjunto: Mejores clientes
                dashboard.ClientesTop = await reader.ReadAsync<ClienteTop>();
                
                // Cuarto conjunto: Estadísticas generales
                var estadisticas = await reader.ReadAsync<EstadisticasGenerales>();
                dashboard.Estadisticas = estadisticas.FirstOrDefault();
                
                return dashboard;
            }
        );
        
        return resultado;
    }
}

// Uso
var service = new DashboardService(connectionString);
var resultado = await service.ObtenerDashboardAsync();

if (resultado.Value && resultado.Data != null)
{
    var dashboard = resultado.Data;
    
    Console.WriteLine("=== DASHBOARD ===");
    Console.WriteLine($"\nEstadísticas Generales:");
    Console.WriteLine($"Total Ventas: ${dashboard.Estadisticas.TotalVentas:N2}");
    Console.WriteLine($"Promedio Venta: ${dashboard.Estadisticas.PromedioVenta:N2}");
    
    Console.WriteLine($"\nVentas Recientes ({dashboard.VentasRecientes.Count}):");
    foreach (var venta in dashboard.VentasRecientes.Take(5))
    {
        Console.WriteLine($"  {venta.Fecha:dd/MM} - {venta.Cliente}: ${venta.Total}");
    }
    
    Console.WriteLine($"\nProductos Más Vendidos:");
    foreach (var producto in dashboard.ProductosTop.Take(5))
    {
        Console.WriteLine($"  {producto.Nombre}: {producto.Unidades} unidades");
    }
}
```

**Procedimiento almacenado:**
```sql
CREATE PROCEDURE sp_ObtenerDatosDashboard
    @Dias INT
AS
BEGIN
    DECLARE @FechaDesde DATE = DATEADD(DAY, -@Dias, GETDATE())
    
    -- Primer resultado: Ventas recientes
    SELECT TOP 10
        v.VentaId,
        v.Fecha,
        c.NombreCompleto AS Cliente,
        v.Total
    FROM Ventas v
    INNER JOIN Clientes c ON v.ClienteId = c.ClienteId
    WHERE v.Fecha >= @FechaDesde
    ORDER BY v.Fecha DESC
    
    -- Segundo resultado: Productos más vendidos
    SELECT TOP 5
        p.Nombre,
        SUM(vi.Cantidad) AS Unidades,
        SUM(vi.Subtotal) AS TotalVendido
    FROM VentaItems vi
    INNER JOIN Productos p ON vi.ProductoId = p.ProductoId
    INNER JOIN Ventas v ON vi.VentaId = v.VentaId
    WHERE v.Fecha >= @FechaDesde
    GROUP BY p.ProductoId, p.Nombre
    ORDER BY SUM(vi.Cantidad) DESC
    
    -- Tercer resultado: Mejores clientes
    SELECT TOP 5
        c.NombreCompleto,
        COUNT(v.VentaId) AS NumeroCompras,
        SUM(v.Total) AS TotalGastado
    FROM Clientes c
    INNER JOIN Ventas v ON c.ClienteId = v.ClienteId
    WHERE v.Fecha >= @FechaDesde
    GROUP BY c.ClienteId, c.NombreCompleto
    ORDER BY SUM(v.Total) DESC
    
    -- Cuarto resultado: Estadísticas generales
    SELECT 
        COUNT(*) AS TotalVentas,
        SUM(Total) AS TotalVentas,
        AVG(Total) AS PromedioVenta,
        MAX(Total) AS VentaMaxima,
        MIN(Total) AS VentaMinima
    FROM Ventas
    WHERE Fecha >= @FechaDesde
END
```

---

#### Caso 6: Mapeo con Atributos Personalizados

```csharp
public class ProductoDto
{
    [ConexionColumn("producto_id")]
    public int Id { get; set; }
    
    [ConexionColumn("nombre_producto")]
    public string Nombre { get; set; }
    
    [ConexionColumn("precio_unitario")]
    public decimal Precio { get; set; }
    
    [ConexionColumn("cantidad_stock")]
    public int Stock { get; set; }
    
    [ConexionColumn("categoria_nombre")]
    public string Categoria { get; set; }
    
    [ConexionColumn("fecha_creacion")]
    public DateTime FechaCreacion { get; set; }
}

public class ProductoRepository
{
    private readonly Conexion _conexion;
    
    public ProductoRepository(string connectionString)
    {
        _conexion = new Conexion(connectionString);
    }
    
    public async Task<ResultList<ProductoDto>> ObtenerProductosConAtributosAsync()
    {
        // El mapeo se hará automáticamente usando los atributos ConexionColumn
        return await _conexion.ExecuteWithResultsAsync<ProductoDto>(
            "sp_ObtenerProductosConNombresPersonalizados"
        );
    }
}

// Uso
var repo = new ProductoRepository(connectionString);
var resultado = await repo.ObtenerProductosConAtributosAsync();

if (resultado.Value && resultado.Data != null)
{
    foreach (var producto in resultado.Data)
    {
        Console.WriteLine($"ID: {producto.Id}");
        Console.WriteLine($"Nombre: {producto.Nombre}");
        Console.WriteLine($"Precio: ${producto.Precio}");
        Console.WriteLine($"Stock: {producto.Stock}");
        Console.WriteLine($"Categoría: {producto.Categoria}");
        Console.WriteLine($"Creado: {producto.FechaCreacion:dd/MM/yyyy}");
        Console.WriteLine("---");
    }
}
```

---

#### Caso 7: Transacciones y Operaciones Complejas

```csharp
public class PedidoService
{
    private readonly Conexion _conexion;
    
    public PedidoService(string connectionString)
    {
        _conexion = new Conexion(connectionString);
    }
    
    public async Task<Result> ProcesarPedidoCompletoAsync(
        int clienteId, 
        List<ItemPedido> items)
    {
        var parametros = new ConexionParameters()
            .Add("ClienteId", clienteId)
            .Add("Items", JsonSerializer.Serialize(items))
            .AddOutput("PedidoId", DbType.Int32, alias: "PedidoId")
            .AddOutput("Total", DbType.Decimal, alias: "TotalPedido")
            .AddOutput("Result", DbType.Boolean)
            .AddOutput("Message", DbType.String, 500)
            .AddOutput("Code", DbType.Int32);
        
        var resultado = await _conexion.ExecuteAsync(
            "sp_ProcesarPedidoCompleto", 
            parametros
        );
        
        if (resultado.Value)
        {
            Console.WriteLine($"Pedido creado: #{resultado.Bag["PedidoId"]}");
            Console.WriteLine($"Total: ${resultado.Bag["TotalPedido"]}");
        }
        
        return resultado;
    }
}

// Uso
var service = new PedidoService(connectionString);
var items = new List<ItemPedido>
{
    new ItemPedido { ProductoId = 1, Cantidad = 2, Precio = 99.99m },
    new ItemPedido { ProductoId = 5, Cantidad = 1, Precio = 49.99m }
};

var resultado = await service.ProcesarPedidoCompletoAsync(clienteId: 10, items);

if (resultado.Value)
{
    Console.WriteLine($"✓ {resultado.Message}");
    Console.WriteLine($"Pedido: {resultado.Bag["PedidoId"]}");
    Console.WriteLine($"Total: ${resultado.Bag["TotalPedido"]:N2}");
}
else
{
    Console.WriteLine($"✗ Error ({resultado.Code}): {resultado.Message}");
}
```

---

## Castable

**Namespace:** `HotPack.Classes`  
**Archivo:** [HotPack/Classes/Castable.cs](HotPack/Classes/Castable.cs)

### Descripción
La clase `Castable` encapsula un objeto y proporciona métodos de conversión seguros a diferentes tipos de datos. Ofrece dos enfoques: conversiones que lanzan excepciones si fallan y conversiones con callbacks de error personalizables.

### Constructor

```csharp
public Castable(object obj)
```

**Parámetros:**
- `obj` (object): El objeto a encapsular y convertir

### Métodos de Conversión

#### Conversiones Seguras (Lanzan Excepción)

Estos métodos intentan convertir el valor encapsulado al tipo especificado. Si la conversión falla, lanzan una excepción con mensaje descriptivo.

| Método | Tipo de Retorno | Descripción |
|--------|----------------|-------------|
| `ToInt32()` | `int` | Convierte a entero de 32 bits |
| `ToUint32()` | `uint` | Convierte a entero sin signo de 32 bits |
| `ToInt64()` | `long` | Convierte a entero de 64 bits |
| `ToUInt64()` | `ulong` | Convierte a entero sin signo de 64 bits |
| `ToDecimal()` | `decimal` | Convierte a decimal |
| `ToSingle()` | `float` | Convierte a float |
| `ToDouble()` | `double` | Convierte a double |
| `ToBoolean()` | `bool` | Convierte a booleano |
| `ToByte()` | `byte` | Convierte a byte |
| `ToBytes()` | `byte[]` | Convierte a array de bytes |
| `ToDateTime()` | `DateTime` | Convierte a DateTime |
| `ToString()` | `string?` | Convierte a string |

#### Conversiones con Callback de Error

Estos métodos intentan convertir el valor y si fallan, ejecutan una función callback personalizada para manejar el error.

| Método | Firma | Descripción |
|--------|-------|-------------|
| `TryInt32()` | `int TryInt32(Func<Castable, int> onError)` | Intenta conversión a int |
| `TryUint32()` | `uint TryUint32(Func<Castable, uint> onError)` | Intenta conversión a uint |
| `TryInt64()` | `long TryInt64(Func<Castable, long> onError)` | Intenta conversión a long |
| `TryUInt64()` | `ulong TryUInt64(Func<Castable, ulong> onError)` | Intenta conversión a ulong |
| `TryDecimal()` | `decimal TryDecimal(Func<Castable, decimal> onError)` | Intenta conversión a decimal |
| `TrySingle()` | `float TrySingle(Func<Castable, float> onError)` | Intenta conversión a float |
| `TryDouble()` | `double TryDouble(Func<Castable, double> onError)` | Intenta conversión a double |
| `TryBoolean()` | `bool TryBoolean(Func<Castable, bool> onError)` | Intenta conversión a bool |
| `TryByte()` | `byte TryByte(Func<Castable, byte> onError)` | Intenta conversión a byte |
| `TryBytes()` | `byte[] TryBytes(Func<Castable, byte[]> onError)` | Intenta conversión a byte[] |
| `TryDateTime()` | `DateTime TryDateTime(Func<Castable, DateTime> onError)` | Intenta conversión a DateTime |
| `TryString()` | `string? TryString(Func<Castable, string> onError)` | Intenta conversión a string |

### Casos de Uso Castable

#### Caso 1: Conversión Básica con Excepciones

```csharp
using HotPack.Classes;

// Conversión exitosa
var castable1 = new Castable("123");
int numero = castable1.ToInt32(); // 123

// Conversión de decimal
var castable2 = new Castable("99.99");
decimal precio = castable2.ToDecimal(); // 99.99

// Conversión de fecha
var castable3 = new Castable("2024-12-01");
DateTime fecha = castable3.ToDateTime(); // 2024-12-01 00:00:00

// Conversión que falla (lanza excepción)
try
{
    var castableInvalido = new Castable("abc");
    int numeroInvalido = castableInvalido.ToInt32(); // Lanza excepción
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message); // "Unable to convert the value "abc" to the type "Int32""
}
```

#### Caso 2: Conversión con Manejo de Errores

```csharp
// Usar valor por defecto si falla
var castable = new Castable("abc");
int numero = castable.TryInt32(c => 0); // Retorna 0 porque "abc" no es numérico

// Logging de errores
var castable2 = new Castable("xyz");
decimal precio = castable2.TryDecimal(c => 
{
    Console.WriteLine($"Error convirtiendo '{c}' a decimal, usando valor por defecto");
    return -1m;
});

// Múltiples intentos de conversión
var castable3 = new Castable("no-es-numero");
int valor = castable3.TryInt32(c => 
{
    // Intentar con valor por defecto de configuración
    return ConfiguracionGlobal.ValorPorDefecto;
});
```

#### Caso 3: Uso con AppConfiguration

```csharp
using HotPack.App;
using HotPack.Classes;

var config = new AppConfiguration();

// Los parámetros se devuelven como Castable
Castable timeoutCastable = config.TryParameter("Timeout", false, () => new Castable("30"));

// Convertir a entero con valor por defecto
int timeout = timeoutCastable.TryInt32(c => 30);

// Convertir a diferentes tipos
Castable maxRetries = config.TryParameter("MaxRetries", false, () => new Castable("3"));
int retries = maxRetries.ToInt32();

Castable enableCache = config.TryParameter("EnableCache", false, () => new Castable("true"));
bool cacheEnabled = enableCache.ToBoolean();

Castable apiUrl = config.TryParameter("ApiUrl", false, () => new Castable("https://api.ejemplo.com"));
string url = apiUrl.ToString();
```

#### Caso 4: Procesamiento de Datos de Base de Datos

```csharp
public class DataProcessor
{
    public void ProcesarFilas(DataTable tabla)
    {
        foreach (DataRow fila in tabla.Rows)
        {
            // Usar Castable para conversiones seguras desde DataRow
            var id = new Castable(fila["Id"]).TryInt32(c => 0);
            var nombre = new Castable(fila["Nombre"]).TryString(c => "Sin nombre");
            var precio = new Castable(fila["Precio"]).TryDecimal(c => 0m);
            var activo = new Castable(fila["Activo"]).TryBoolean(c => false);
            var fechaCreacion = new Castable(fila["FechaCreacion"]).TryDateTime(c => DateTime.MinValue);
            
            Console.WriteLine($"Producto {id}: {nombre} - ${precio}");
        }
    }
}
```

#### Caso 5: Validación y Conversión de Entrada de Usuario

```csharp
public class FormularioService
{
    public Result ValidarYGuardar(Dictionary<string, string> formulario)
    {
        try
        {
            // Convertir y validar edad
            var edadCastable = new Castable(formulario["edad"]);
            int edad = edadCastable.TryInt32(c => 
            {
                return -1; // Indica error
            });
            
            if (edad < 0 || edad > 120)
            {
                return new Result(false, "Edad inválida");
            }
            
            // Convertir salario
            var salarioCastable = new Castable(formulario["salario"]);
            decimal salario = salarioCastable.TryDecimal(c => 
            {
                throw new Exception("El salario debe ser un número válido");
            });
            
            // Convertir fecha de nacimiento
            var fechaNacCastable = new Castable(formulario["fechaNacimiento"]);
            DateTime fechaNac = fechaNacCastable.TryDateTime(c => DateTime.MinValue);
            
            if (fechaNac == DateTime.MinValue)
            {
                return new Result(false, "Fecha de nacimiento inválida");
            }
            
            // Guardar en base de datos...
            return new Result(true, "Datos guardados correctamente");
        }
        catch (Exception ex)
        {
            return new Result(ex);
        }
    }
}
```

#### Caso 6: Trabajo con JSON Dinámico

```csharp
using System.Text.Json;

public class JsonProcessor
{
    public void ProcesarJson(string jsonString)
    {
        var jsonDoc = JsonDocument.Parse(jsonString);
        var root = jsonDoc.RootElement;
        
        // Usar Castable para convertir valores JSON
        var idCastable = new Castable(root.GetProperty("id").GetRawText());
        int id = idCastable.ToInt32();
        
        var precioCastable = new Castable(root.GetProperty("precio").GetRawText());
        decimal precio = precioCastable.TryDecimal(c => 0m);
        
        var activoCastable = new Castable(root.GetProperty("activo").GetRawText());
        bool activo = activoCastable.TryBoolean(c => false);
        
        Console.WriteLine($"ID: {id}, Precio: {precio}, Activo: {activo}");
    }
}
```

#### Caso 7: Conversión de Bytes

```csharp
public class FileService
{
    public void ProcesarArchivo(object archivoData)
    {
        // Convertir a bytes
        var castable = new Castable(archivoData);
        byte[] bytes = castable.TryBytes(c => new byte[0]);
        
        if (bytes.Length > 0)
        {
            // Procesar el archivo
            File.WriteAllBytes("output.dat", bytes);
            Console.WriteLine($"Archivo guardado: {bytes.Length} bytes");
        }
        else
        {
            Console.WriteLine("No se pudieron obtener los bytes del archivo");
        }
    }
}
```

#### Caso 8: Sistema de Configuración Tipado

```csharp
public class AppSettings
{
    private readonly AppConfiguration _config;
    
    public AppSettings(AppConfiguration config)
    {
        _config = config;
    }
    
    public int MaxConnections => 
        _config.TryParameter("MaxConnections", false, () => new Castable("100"))
                .TryInt32(c => 100);
    
    public decimal TaxRate => 
        _config.TryParameter("TaxRate", false, () => new Castable("0.16"))
                .TryDecimal(c => 0.16m);
    
    public bool EnableLogging => 
        _config.TryParameter("EnableLogging", false, () => new Castable("true"))
                .TryBoolean(c => true);
    
    public DateTime MaintenanceDate => 
        _config.TryParameter("MaintenanceDate", false, () => new Castable(DateTime.Now.AddDays(7).ToString()))
                .TryDateTime(c => DateTime.Now.AddDays(7));
}

// Uso
var config = new AppConfiguration();
var settings = new AppSettings(config);

Console.WriteLine($"Conexiones máximas: {settings.MaxConnections}");
Console.WriteLine($"Tasa de impuesto: {settings.TaxRate:P}");
Console.WriteLine($"Logging habilitado: {settings.EnableLogging}");
Console.WriteLine($"Mantenimiento: {settings.MaintenanceDate:dd/MM/yyyy}");
```

---

## Conclusión

La librería **HotPack** proporciona un conjunto robusto de herramientas para el desarrollo de aplicaciones .NET:

### Características Principales

1. **Patrón Result Consistente**: Todas las operaciones devuelven objetos `Result` o `Result<T>` que encapsulan éxito/fallo, mensajes y datos, facilitando el manejo uniforme de respuestas.

2. **Acceso a Datos Simplificado**: La clase `Conexion` abstrae la complejidad de ADO.NET y proporciona:
   - Mapeo automático de objetos
   - Soporte para procedimientos almacenados
   - Manejo de múltiples conjuntos de resultados
   - Parámetros de entrada/salida con mapeo automático

3. **Conversiones Seguras**: La clase `Castable` ofrece conversiones de tipos con dos enfoques:
   - Conversiones que lanzan excepciones para validación estricta
   - Conversiones con callbacks para manejo flexible de errores

4. **Integración Perfecta**: Todas las clases trabajan juntas de forma cohesiva:
   - `Conexion` devuelve `Result`/`Result<T>`/`ResultList<T>`
   - `AppConfiguration` usa `Castable` para parámetros
   - Los parámetros de salida se mapean automáticamente al `Result.Bag`

### Beneficios

✅ **Código más limpio**: Reduce boilerplate code  
✅ **Tipo-seguro**: Aprovecha el sistema de tipos de C#  
✅ **Consistente**: APIs uniformes en toda la librería  
✅ **Productivo**: Menos código para tareas comunes  
✅ **Mantenible**: Separación clara de responsabilidades  
✅ **Testeable**: Fácil de hacer unit testing  

### Casos de Uso Ideales

- **APIs REST**: Respuestas consistentes con `Result<T>`
- **Aplicaciones empresariales**: Acceso a datos con `Conexion`
- **Sistemas configurables**: Configuración con `AppConfiguration`
- **Microservicios**: Comunicación entre servicios con `Result`
- **Procesamiento de datos**: Conversiones seguras con `Castable`

---

**Versión**: 1.0  
**Fecha**: Noviembre 2025  
**Licencia**: [Especificar licencia]  
**Repositorio**: [URL del repositorio]

Para más información y ejemplos adicionales, consulte el archivo [documentacion.md](documentacion.md).