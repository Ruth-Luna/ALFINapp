import csv
import pyodbc

# Cadena de conexión a la base de datos
connection_string = ("DRIVER={ODBC Driver 17 for SQL Server};"
                     "SERVER=103.195.101.157;"
                     "DATABASE=CORE_ALFIN;"
                     "UID=programacion;"
                     "PWD=2345;"
                     "TrustServerCertificate=yes;")

# Función para leer el CSV e insertar los datos en la tabla SQL
def cargar_csv_a_sql(csv_file):
    try:
        # Establecer conexión con la base de datos
        conn = pyodbc.connect(connection_string)
        cursor = conn.cursor()

        # Activar fast_executemany para mejorar la velocidad de inserción
        cursor.fast_executemany = True

        # Leer el archivo CSV con codificación ISO-8859-1
        with open(csv_file, mode='r', encoding='ISO-8859-1') as file:
            reader = csv.DictReader(file)

            # Función para convertir campos vacíos en NULL
            def convertir_a_null(valor):
                return None if valor == "" else valor

            # Crear una lista para almacenar los datos a insertar
            datos_a_insertar = []

            # Iterar sobre las filas del CSV e ir acumulando los valores
            for row in reader:
                datos_a_insertar.append((
                    convertir_a_null(row['dni']),
                    convertir_a_null(row['tasa_1']),
                    convertir_a_null(row['tasa_2']),
                    convertir_a_null(row['tasa_3']),
                    convertir_a_null(row['tasa_4']),
                    convertir_a_null(row['tasa_5']),
                    convertir_a_null(row['tasa_6']),
                    convertir_a_null(row['tasa_7']),
                    convertir_a_null(row['oferta_max']),
                    convertir_a_null(row['plazo']), 
                    convertir_a_null(row['CAPACIDAD_PAGO_MEN']),
                    convertir_a_null(row['campana_grupo']),
                    convertir_a_null(row['color_id']),
                    convertir_a_null(row['usuario']),
                    convertir_a_null(row['RANGO_DEUDA']),
                    convertir_a_null(row['NumEntidades']),
                    convertir_a_null(row['fresco'])
                ))

                # Cuando el lote alcanza un tamaño de 1000 filas, insertamos los datos
                if len(datos_a_insertar) >= 1000:
                    cursor.executemany("""
                        INSERT INTO [CORE_ALFIN].[dbo].[base_clientes_banco] 
                        ([dni], [tasa_1], [tasa_2], [tasa_3], [tasa_4], 
                         [tasa_5], [tasa_6], [tasa_7], [oferta_max], [id_plazo_banco], 
                         [CAPACIDAD_PAGO_MEN], [id_campana_grupo_banco], [id_color_banco], 
                         [id_usuario_banco], [id_rango_deuda], [num_entidades], [frescura])
                        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
                    """, datos_a_insertar)

                    print ("Insertando lote de 1000 filas...")

                    # Limpiar la lista para el siguiente lote
                    datos_a_insertar = []

            # Insertar cualquier dato restante que no haya sido insertado
            if datos_a_insertar:
                cursor.executemany("""
                    INSERT INTO [CORE_ALFIN].[dbo].[base_clientes_banco] 
                    ([dni], [tasa_1], [tasa_2], [tasa_3], [tasa_4], 
                     [tasa_5], [tasa_6], [tasa_7], [oferta_max], [id_plazo_banco], 
                     [CAPACIDAD_PAGO_MEN], [id_campana_grupo_banco], [id_color_banco], 
                     [id_usuario_banco], [id_rango_deuda], [num_entidades], [frescura])
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
                """, datos_a_insertar)
                print("Insertando cualquier dato restante...")
            # Confirmar los cambios en la base de datos
            conn.commit()

        print("Datos cargados exitosamente desde el archivo CSV.")

    except Exception as e:
        print("Error al cargar los datos:", e)

    finally:
        # Cerrar la conexión a la base de datos
        cursor.close()
        conn.close()

# Nombre del archivo CSV (debe estar en la misma carpeta que este script)
csv_file = 'HOJA2DATOS.csv'

# Llamar a la función para cargar los datos
cargar_csv_a_sql(csv_file)
