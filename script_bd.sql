CREATE DATABASE HotelZormatDB;
GO

USE HotelZormatDB;
GO

-- USUARIOS
CREATE TABLE dbo.Usuarios
(
IdUsuario INT IDENTITY(1,1) NOT NULL,
NombreUsuario VARCHAR(30) NOT NULL,
ContrasenaHash VARCHAR(200) NOT NULL,
Rol VARCHAR(20) NOT NULL,
NombreCompleto VARCHAR(80) NOT NULL,
Activo BIT DEFAULT (1) NOT NULL,

CONSTRAINT PK_Usuarios
    PRIMARY KEY (IdUsuario),

CONSTRAINT UQ_Usuarios_NombreUsuario
    UNIQUE (NombreUsuario),

CONSTRAINT CK_Usuarios_Rol
    CHECK (Rol IN ('Administrador', 'Recepcionista'))
);
GO

-- Habitaciones
CREATE TABLE dbo.Habitaciones
(
Numero INT NOT NULL,
Piso INT NOT NULL,
Tipo VARCHAR(20) NOT NULL,
TarifaBase DECIMAL(10,2) NOT NULL,
Capacidad INT NOT NULL,
Estado VARCHAR(20) NOT NULL,

CONSTRAINT PK_Habitaciones
    PRIMARY KEY (Numero),

CONSTRAINT CK_Habitaciones_Piso
    CHECK (Piso > 0),

CONSTRAINT CK_Habitaciones_Tipo
    CHECK (Tipo IN ('Sencilla', 'Doble', 'Suite')),

CONSTRAINT CK_Habitaciones_TarifaBase
    CHECK (TarifaBase > 0),

CONSTRAINT CK_Habitaciones_Capacidad
    CHECK (Capacidad > 0),

CONSTRAINT CK_Habitaciones_Estado
    CHECK (Estado IN ('Disponible', 'Ocupada', 'Reservada', 'Limpieza'))
);

GO

-- Huesped
CREATE TABLE dbo.Huespedes
(
NumeroDocumento VARCHAR(20) NOT NULL,
TipoDocumento VARCHAR(10) NOT NULL,
Nombre NVARCHAR(60) NOT NULL,
Apellido NVARCHAR(60) NOT NULL,
Telefono VARCHAR(20) NULL,
Email VARCHAR(80) NULL,

CONSTRAINT PK_Huespedes
    PRIMARY KEY (NumeroDocumento),

CONSTRAINT CK_Huespedes_TipoDocumento
    CHECK (TipoDocumento IN ('Cedula', 'Pasaporte')),

CONSTRAINT CK_Huespedes_NumeroDocumento
    CHECK
    (
        (TipoDocumento = 'Cedula'
            AND LEN(NumeroDocumento) = 11
            AND NumeroDocumento NOT LIKE '%[^0-9]%')
        OR
        (TipoDocumento = 'Pasaporte'
            AND LEN(LTRIM(RTRIM(NumeroDocumento))) > 0)
    )
);
GO

-- Reservas
CREATE TABLE dbo.Reservas
(
IdReserva INT IDENTITY(1,1) NOT NULL,
NumeroDocumentoHuesped VARCHAR(20) NOT NULL,
NumeroHabitacion INT NOT NULL,
FechaCheckIn DATETIME NOT NULL,
FechaCheckOut DATETIME NOT NULL,
Temporada VARCHAR(10) NOT NULL,
Estado VARCHAR(15) NOT NULL,
TotalNoches INT NOT NULL,
MontoEstimado DECIMAL(10,2) NOT NULL,

CONSTRAINT PK_Reservas
    PRIMARY KEY (IdReserva),

CONSTRAINT FK_Reservas_Huespedes
    FOREIGN KEY (NumeroDocumentoHuesped)
    REFERENCES dbo.Huespedes (NumeroDocumento),

CONSTRAINT FK_Reservas_Habitaciones
    FOREIGN KEY (NumeroHabitacion)
    REFERENCES dbo.Habitaciones (Numero),

CONSTRAINT CK_Reservas_Fechas
    CHECK (FechaCheckOut > FechaCheckIn),

CONSTRAINT CK_Reservas_Temporada
    CHECK (Temporada IN ('Alta', 'Media', 'Baja')),

CONSTRAINT CK_Reservas_Estado
    CHECK (Estado IN ('Pendiente', 'Confirmada', 'Cancelada')),

CONSTRAINT CK_Reservas_TotalNoches
    CHECK
    (
        TotalNoches > 0
        AND TotalNoches = DATEDIFF(DAY, FechaCheckIn, FechaCheckOut)
    ),
CONSTRAINT CK_Reservas_MontoEstimado
    CHECK (MontoEstimado > 0)
);

GO

-- Estadia
CREATE TABLE dbo.Estadias
(
IdEstadia INT IDENTITY(1,1) NOT NULL,
IdReserva INT NOT NULL,
FechaEntradaReal DATETIME NOT NULL,
FechaSalidaReal DATETIME NULL,
Estado VARCHAR(10) NOT NULL,

CONSTRAINT PK_Estadias
    PRIMARY KEY (IdEstadia),

CONSTRAINT FK_Estadias_Reservas
    FOREIGN KEY (IdReserva)
    REFERENCES dbo.Reservas (IdReserva),

CONSTRAINT CK_Estadias_Estado
    CHECK (Estado IN ('Activa', 'Cerrada')),

CONSTRAINT CK_Estadias_Fechas
    CHECK
    (
        FechaSalidaReal IS NULL
        OR FechaSalidaReal > FechaEntradaReal
    ),
CONSTRAINT CK_Estadias_Cierre
    CHECK
    (
        (Estado = 'Activa' AND FechaSalidaReal IS NULL)
        OR
        (Estado = 'Cerrada' AND FechaSalidaReal IS NOT NULL)
    )
);

GO

-- Facturas
CREATE TABLE dbo.Facturas
(
IdFactura INT IDENTITY(1,1) NOT NULL,
IdEstadia INT NOT NULL,
NumeroNCF VARCHAR(19) NOT NULL,
Subtotal DECIMAL(10,2) NOT NULL,
ITBIS DECIMAL(10,2) NOT NULL,
Propina DECIMAL(10,2) NOT NULL,
Total DECIMAL(10,2) NOT NULL,
FechaEmision DATETIME NOT NULL
    CONSTRAINT DF_Facturas_FechaEmision DEFAULT (GETDATE()),

CONSTRAINT PK_Facturas
    PRIMARY KEY (IdFactura),

CONSTRAINT FK_Facturas_Estadias
    FOREIGN KEY (IdEstadia)
    REFERENCES dbo.Estadias (IdEstadia),

CONSTRAINT UQ_Facturas_NumeroNCF
    UNIQUE (NumeroNCF),

CONSTRAINT CK_Facturas_Montos
    CHECK
    (
        Subtotal >= 0
        AND ITBIS >= 0
        AND Propina >= 0
        AND Total >= 0
    ),

CONSTRAINT CK_Facturas_Total
    CHECK (Total = Subtotal + ITBIS + Propina)

);
GO

-- Bitacora
CREATE TABLE dbo.Bitacora
(
IdBitacora INT IDENTITY(1,1) NOT NULL,
IdUsuario INT NOT NULL,
Accion VARCHAR(20) NOT NULL,
Detalle VARCHAR(200) NULL,
FechaHora DATETIME NOT NULL
    CONSTRAINT DF_Bitacora_FechaHora DEFAULT (GETDATE()),

CONSTRAINT PK_Bitacora
    PRIMARY KEY (IdBitacora),

CONSTRAINT FK_Bitacora_Usuarios
    FOREIGN KEY (IdUsuario)
    REFERENCES dbo.Usuarios (IdUsuario),

CONSTRAINT CK_Bitacora_Accion
    CHECK (Accion IN ('Login', 'CheckIn', 'CheckOut', 'Facturacion'))
);

GO

-- DATOS INICIALES DE PRUEBA **USUARIOS Y HABITACIONES**

-- INSERT: USUARIOS

INSERT INTO dbo.Usuarios
    (NombreUsuario, ContrasenaHash, Rol, NombreCompleto, Activo)
VALUES
(
    'administrador',CONVERT(VARCHAR(200),
    HASHBYTES('SHA2_256', 'HotelZormat.Admin.2026!'), 2),
    'Administrador', 'Administrador del sistema',1),

(
    'recepcionista', CONVERT(VARCHAR(200),
    HASHBYTES('SHA2_256', 'HotelZormat.Recepcion.2026!'),2),
    'Recepcionista','Recepcionista de prueba',1);
GO


-- INSERT: 9 habitaciones en 3 pisos

INSERT INTO dbo.Habitaciones
    (Numero, Piso, Tipo, TarifaBase, Capacidad, Estado)
VALUES
    (101, 1, 'Sencilla', 2500.00, 1, 'Disponible'),
    (102, 1, 'Doble',    3500.00, 2, 'Ocupada'),
    (103, 1, 'Suite',    5500.00, 4, 'Limpieza'),
    (201, 2, 'Sencilla', 2700.00, 1, 'Reservada'),
    (202, 2, 'Doble',    3800.00, 2, 'Disponible'),
    (203, 2, 'Suite',    6000.00, 4, 'Disponible'),
    (301, 3, 'Sencilla', 2900.00, 1, 'Limpieza'),
    (302, 3, 'Doble',    4100.00, 3, 'Reservada'),
    (303, 3, 'Suite',    6500.00, 4, 'Ocupada');
GO
