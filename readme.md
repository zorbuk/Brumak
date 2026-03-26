
# Bienvenido a "BRUMAK".

Brumak es un proyecto de **MMORPG Isométrico por turnos** con todo lo que ello  implica.

## Tecnologías
Uso de C# con WPF en el cliente, conectividad con Sockets TCP, base de datos con ORM y tests con xUnit.
System.Drawings para el renderizado.

## ¿Como abro un servidor?
(..) Todavía no está disponible. ¡Pronto!

## Configuraciones
**Brumak-Client** : *db_settings.json*

    {
	    "ConnectionStrings": {
		    "AuthServerPort": int,
		    "AuthServerIp": "ip as string",
		    "ShowLogs": bool,
		    "SaveLogs": bool
	    }
    }

**Brumak-Auth** : *db_settings.json*

    {
	    "ConnectionStrings": {
	      "BrumakDb": "connection string",
		    "AuthServerPort": int,
		    "AuthServerIp": "ip as string",
		    "ShowLogs": bool,
		    "SaveLogs": bool
	    }
    }

## Creado por
**Linkedin**: [Miquel Valero](https://www.linkedin.com/in/miquelvalero/)

> Actualmente estoy en **búsqueda activa** de trabajo en lenguajes C# o NodeJS. 😉
